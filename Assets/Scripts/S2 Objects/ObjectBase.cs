using System;
using UnityEngine;
using System.IO;

//Declare some structs for object parameters
[Serializable]
public struct IntParameters
{
    public string param_name;
    public int value;

    public IntParameters(string name, int param)
    {
        param_name = name;
        value = param;
    }
}
[Serializable]
public class StringParameters
{
    public string param_name;
    public string value;

    public StringParameters(string name, string param)
    {
        param_name = name;
        value = param;
    }
}
[Serializable]
public class BoolParameters
{
    public string param_name;
    public bool value;

    public BoolParameters(string name, bool param)
    {
        param_name = name;
        value = param;
    }
}
[Serializable]
public class FloatParameters
{
    public string param_name;
    public float value;

    public FloatParameters(string name, float param)
    {
        param_name = name;
        value = param;
    }
}

public class Vector3Parameters
{
    public string param_name;
    public string value;

    public Vector3Parameters(string name, Vector3 param)
    {
        param_name = name;
        value = "{X: " + param.x.ToString() + ", Y: " + param.y.ToString() + ", Z: " + param.z.ToString() + "}";
    }
}

public class ObjectBase : MonoBehaviour
{
    //Object Headers&footers
    private const string header = "    - ";
    private const string six_white_space = "      ";
    private const string four_white_space = "    ";

    //These are common to every object
    [Header("Common Variables")]
    [Tooltip("The Id of the object, needed for links.")]
    public string id = "obj";
    [Tooltip("True if an object is linked to this object.")]
    public bool isLinkDest = false;
    [Tooltip("Determines what modes the object appears in.")]
    public string layerConfigName = "Cmn";
    [Tooltip("The objects internal name.")]
    public string unitConfigName;

    //This section is for links
    [Header("Link Headers")]
    [Tooltip("Each element represents a header.")]
    public string[] linkHeaders;
    [Tooltip("Each element represents the number of links under each header, they will be paired in chronological order andmust match the amount of elements.")]
    public int[] linkHeaderSize;
    [Header("Links")]
    private string[] definitionName;
    [Tooltip("Size needs to be the total number under linkHeaderSize.")]
    public string[] destUnitId;

    [Header("Object Specific Params")]
    [Tooltip("Parameter values featuring ints (numbers without decimals)")]
    public IntParameters[] intParams;
    [Tooltip("Parameters featuring True or False")]
    public BoolParameters[] boolParams;
    [Tooltip("Parameters featuring strings (words)")]
    public StringParameters[] stringParams;
    [Tooltip("Parameters featuring floats (numbers with decimals)")]
    public FloatParameters[] floatParams;

    [Header("Other")]
    [Tooltip("For when using unity 3d objects to represent areas in S2 (Unity 3d object scale is 1x1x1, S2 is 10x10x10)")]
    public bool isZone;
    [Tooltip("For when your object representations scale is not what it should be in S2")]
    public bool setScaleToOne;
    [Tooltip("Rotation values are sometimes flipped")]
    public bool flipRotation;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rotation = UnityEditor.TransformUtils.GetInspectorRotation(transform);

        Vector3 translate = transform.position;
        translate.x = -translate.x;

        Vector3 scale = transform.localScale;

        //Setup our always used values
        StringParameters obj_id = new StringParameters("Id", id);
        BoolParameters link_dest = new BoolParameters("IsLinkDest", isLinkDest);
        StringParameters layer = new StringParameters("LayerConfigName", layerConfigName);
        StringParameters unit_config_name = new StringParameters("UnitConfigName", unitConfigName);

        //Declare a variable size array and an index for it
        string[] xml = new string[CalculateObjXmlSize()];
        int xml_index = 0;

        //Create our XMl
        //Common
        xml[xml_index++] = header + obj_id.param_name + ": " + obj_id.value;
        xml[xml_index++] = ObjFactory(link_dest);
        xml[xml_index++] = ObjFactory(layer);
        //links
        AddLinks(ref xml, ref xml_index);
        //Likely not required, but it exists on all objects in the final game
        xml[xml_index++] = ObjFactory(new StringParameters("ModelName", "null"));

        //Add rotate, transform, and scale
        if (flipRotation)
        {
            rotation.x *= -1;
            rotation.y *= -1;
            rotation.z *= -1;
        }
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Rotate", rotation));

        xml[xml_index++] = ObjFactory(new Vector3Parameters("Translate", translate));

        if (isZone)
        {
            scale.x /= 10;
            scale.y /= 10;
            scale.z /= 10;
        }
        if (setScaleToOne)
        {
            scale.x = 1;
            scale.y = 1;
            scale.z = 1;
        }
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Scale", scale));


        xml[xml_index++] = ObjFactory(unit_config_name);
        
        //Add Unique Params
        for (int i = 0; i < intParams.Length; i++)
        {
            xml[xml_index++] = ObjFactory(intParams[i]);
        }

        for (int i = 0; i < boolParams.Length; i++)
        {
            xml[xml_index++] = ObjFactory(boolParams[i]);
        }

        for (int i = 0; i < stringParams.Length; i++)
        {
            xml[xml_index++] = ObjFactory(stringParams[i]);
        }

        for (int i = 0; i < floatParams.Length; i++)
        {
            xml[xml_index++] = ObjFactory(floatParams[i]);
        }

        //Put YAML in a file
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
        // Start with rotate, translate, scale, and the 5 basic inputs
        int xml_size = 9;

        // Add link length if valid
        if (CheckLinkSectionLength())
        {
            xml_size += GetLinkSectionXmlSize();
        }

        xml_size += intParams.Length;
        xml_size += boolParams.Length;
        xml_size += stringParams.Length;
        xml_size += floatParams.Length;

        return xml_size;
    }

    int GetLinkSectionXmlSize()
    {
        int link_xml_size = 0;

        if (CheckLinkSectionLength())
        {
            //1 line per link
            link_xml_size += (destUnitId.Length);
            //1 line per header
            link_xml_size += (linkHeaderSize.Length);
        }

        return link_xml_size;
    }

    bool CheckLinkSectionLength()
    {
        int link_size = 0;
        //Add link length if valid
        if (linkHeaders.Length == 0) { return false; }

        if (linkHeaders.Length == linkHeaderSize.Length)
        {
            for (int i = 0; i < linkHeaderSize.Length; i++)
            {
                link_size += (linkHeaderSize[i]);
            }
            if (link_size == destUnitId.Length)
            {
                return true;
            }
            else
            {
                Debug.Log("LinkHeader does not add to destUnitId length");
                return false;
            }
        }
        else
        {
            Debug.Log("linkHeader & linkHeader size not same length");
            return false;
        }
    }

    void AddLinks(ref string[] xml, ref int xml_index)
    {
        int dest_unit_id_index = 0;

        if (CheckLinkSectionLength()) 
        {
            xml[xml_index++] = six_white_space + "Links:";
            for(int i = 0; i < linkHeaders.Length; i++)
            {
                xml[xml_index++] = six_white_space + "  " + linkHeaders[i] + ":";
                for (int y = 0; y < linkHeaderSize[i]; y++)
                {
                    xml[xml_index++] = six_white_space + header + "{DefinitionName: " + linkHeaders[i] + ", DestUnitId: " + destUnitId[dest_unit_id_index++] + ", UnitFileName: ''}";
                }
            }
        }
        else
        {
            xml[xml_index++] = six_white_space + "Links: !ref0 {}";
        }
    }

    void ExportToXmlFile(ref string[] xml)
    {
        using (StreamWriter xmlOut = new StreamWriter("Assets/Scripts/MapObjectExport.txt", true))
        {
            for (int i = 0; i < xml.Length; i++)
            {
                xmlOut.WriteLine(xml[i]);
            }
        }
    }
}
