using UnityEngine;
using System.IO;

public class RailBase : MonoBehaviour
{
    //Object Headers&footers
    private const string header = "    - ";
    private const string six_white_space = "      ";
    private const string four_white_space = "    ";

    //These are common to every object
    [Header("Variables")]
    [Tooltip("The Id of the object, needed for links.")]
    public string id = "rail";
    [Tooltip("True if an rails end and start are connected.")]
    public bool isClosed = false;
    [Tooltip("???")]
    public bool isLadder = false;
    [Tooltip("True if an object is linked to this object.")]
    public bool isLinkDest = false;
    [Tooltip("Determines what modes the object appears in.")]
    public string layerConfigName = "Cmn";
    [Tooltip("???")]
    public int priority = 100;
    [Tooltip("Linear(straight) or Bezier(curved)")]
    public string railType = "Linear";
    [Tooltip("The objects internal name.")]
    public string unitConfigName = "Rail";

    [Header("Rail Points")]
    [Tooltip("Add all railpoints here.")]
    public RailPoint[] rails;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 translate = transform.position;
        Vector3 rotation = UnityEditor.TransformUtils.GetInspectorRotation(this.transform);

        //Setup our always used values
        StringParameters rail_id = new StringParameters("Id", id);
        BoolParameters is_closed = new BoolParameters("IsClosed", isClosed);
        BoolParameters is_ladder = new BoolParameters("IsLadder", isLadder);
        BoolParameters link_dest = new BoolParameters("IsLinkDest", isLinkDest);
        StringParameters layer = new StringParameters("LayerConfigName", layerConfigName);
        IntParameters rail_priority = new IntParameters("Priority", priority);
        StringParameters rail_type = new StringParameters("RailType", railType);
        StringParameters unit_config_name = new StringParameters("UnitConfigName", unitConfigName);

        //Declare a variable size array and an index for it
        string[] xml = new string[CalculateObjXmlSize()];
        int xml_index = 0;

        //Create our XMl
        //Common
        xml[xml_index++] = header + rail_id.param_name + ": " + rail_id.value;
        xml[xml_index++] = ObjFactory(is_closed);
        xml[xml_index++] = ObjFactory(is_ladder);
        xml[xml_index++] = ObjFactory(link_dest);
        xml[xml_index++] = ObjFactory(layer);
        xml[xml_index++] = six_white_space + "Links: {}";
        //Likely not required, but it exists on all objects in the final game
        xml[xml_index++] = six_white_space + "ModelName: null";
        xml[xml_index++] = ObjFactory(rail_priority);

        xml[xml_index++] = six_white_space + "RailPoints:";
        //Add RailPoints
        for(int i = 0; i < rails.Length; i++)
        {
            rails[i].AddRailPoint(ref xml, ref xml_index);
        }

        xml[xml_index++] = ObjFactory(rail_type);

        //Add rotate, transform, and scale
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Rotate", rotation));
        translate.x = -translate.x;
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Translate", translate));
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Scale", transform.localScale));

        xml[xml_index++] = ObjFactory(unit_config_name);

        //Put XML in a file
        ExportToXmlFile(ref xml);
    }

    string ObjFactory(StringParameters inject)
    {
        string parameter = six_white_space + inject.param_name + ": " + inject.value;

        return parameter;
    }

    string ObjFactory(IntParameters inject)
    {
        string parameter = six_white_space + inject.param_name + ": !l " + inject.value;

        return parameter;
    }

    string ObjFactory(BoolParameters inject)
    {
        string parameter;
        if (inject.value)
        {
            parameter = six_white_space + inject.param_name + ": true";
        }
        else
        {
            parameter = six_white_space + inject.param_name + ": false";
        }
        return parameter;
    }

    string ObjFactory(FloatParameters inject)
    {
        string parameter = six_white_space + inject.param_name + ": " + inject.value;

        return parameter;
    }

    string ObjFactory(Vector3Parameters inject)
    {
        string parameter = six_white_space + inject.param_name + ": " + inject.value;

        return parameter;
    }

    int CalculateObjXmlSize()
    {
        //Start with rotate, translate, scale, and the 5 basic inputs
        int xml_size = 14;

        for(int i = 0; i < rails.Length; i++)
        {
            xml_size += 17;
        }

        return xml_size;
    }


    void ExportToXmlFile(ref string[] xml)
    {
        using (StreamWriter xmlOut = new StreamWriter("Assets/Scripts/MapRailExport.txt", true))
        {
            for (int i = 0; i < xml.Length; i++)
            {
                xmlOut.WriteLine(xml[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
