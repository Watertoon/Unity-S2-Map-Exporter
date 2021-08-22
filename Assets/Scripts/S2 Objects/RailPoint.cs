using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailPoint : MonoBehaviour
{
    //Object Headers&footers
    private const string header = "    - ";
    private const string six_white_space = "      ";
    private const string four_white_space = "    ";

    //These are common to every RailPoint
    [Header("Variables")]
    public int checkPointIndex = 0;
    [Tooltip("The Id of the object, needed for links.")]
    public string id = "rail";
    [Tooltip("True if an object is linked to this object.")]
    public bool isLinkDest = false;
    [Tooltip("Determines what modes the object appears in.")]
    public string layerConfigName = "Cmn";
    [Tooltip("Offsets X")]
    public float offsetX;
    [Tooltip("Offsets Y")]
    public float offsetY;
    [Tooltip("Offsets Z")]
    public float offsetZ;
    [Tooltip("The objects internal name.")]
    public string unitConfigName = "RailPoint";
    [Tooltip("Determines whether offsets are used")]
    public bool useOffset = false;

    [Header("Control Points")]
    public GameObject firstPoint;
    public GameObject secondPoint;

    [Header("VRail only parameters")]
    [Tooltip("Hp for checkpoint.")]
    public int checkPointHp = 0;
    public int slopeFillUp = 0;
    public bool useRotateDir = false;

    public void AddRailPoint(ref string[] xml, ref int xml_index, bool isVrail)
    {
        Vector3 translate = transform.position;
        Vector3 rotation = UnityEditor.TransformUtils.GetInspectorRotation(this.transform);

        //Setup our always used values
        IntParameters check_point_index = new IntParameters("CheckPointIndex", checkPointIndex);
        StringParameters obj_id = new StringParameters("Id", id);
        BoolParameters link_dest = new BoolParameters("IsLinkDest", isLinkDest);
        StringParameters layer = new StringParameters("LayerConfigName", layerConfigName);
        FloatParameters offset_x = new FloatParameters("OffsetX", offsetX);
        FloatParameters offset_y = new FloatParameters("OffsetY", offsetY);
        FloatParameters offset_z = new FloatParameters("OffsetZ", offsetZ);
        StringParameters unit_config_name = new StringParameters("UnitConfigName", unitConfigName);
        BoolParameters use_offset = new BoolParameters("UseOffset", useOffset);

        Vector3 control_one = GetControlPoint1();
        control_one.x = -control_one.x;
        Vector3 control_two = GetControlPoint2();
        control_two.x = -control_two.x;
        Vector3Parameters control_point1 = new Vector3Parameters("", control_one);
        Vector3Parameters control_point2 = new Vector3Parameters("", control_two);


        //Vrail stuff
        IntParameters check_point_hp = new IntParameters("CheckPointHP", checkPointHp);
        IntParameters slope_fill_up = new IntParameters("SlopeFillUp", slopeFillUp);
        BoolParameters use_rotate_dir = new BoolParameters("UseRotateDir", useRotateDir);

        //Common
        if(isVrail)
        {
            xml[xml_index++] = four_white_space + header + check_point_hp.param_name + ": " + check_point_hp.value;
            xml[xml_index++] = ObjFactory(check_point_index);
        }
        else
        {
            xml[xml_index++] = four_white_space + header + check_point_index.param_name + ": " + check_point_index.value;
        }

        xml[xml_index++] = six_white_space + four_white_space + "ControlPoints:";
        xml[xml_index++] = four_white_space + four_white_space + header + control_point1.value;
        xml[xml_index++] = four_white_space + four_white_space + header + control_point2.value;

        xml[xml_index++] = ObjFactory(obj_id);
        xml[xml_index++] = ObjFactory(link_dest);
        xml[xml_index++] = ObjFactory(layer);
        xml[xml_index++] = four_white_space + six_white_space + "Links: {}";
        //Likely not required, but it exists on all objects in the final game
        if (isVrail)
        {
            xml[xml_index++] = four_white_space + six_white_space + "ModelName: RailPoint";
        }
        else
        {
            xml[xml_index++] = four_white_space + six_white_space + "ModelName:";
        }

        xml[xml_index++] = ObjFactory(offset_x);
        xml[xml_index++] = ObjFactory(offset_y);
        xml[xml_index++] = ObjFactory(offset_z);

        //Add rotate, transform, and scale
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Rotate", rotation));
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Scale", transform.localScale));
        if (isVrail)
            xml[xml_index++] = ObjFactory(slope_fill_up);
        translate.x = -translate.x;
        xml[xml_index++] = ObjFactory(new Vector3Parameters("Translate", translate));

        xml[xml_index++] = ObjFactory(unit_config_name);
        xml[xml_index++] = ObjFactory(use_offset);
        if (isVrail)
            xml[xml_index++] = ObjFactory(use_rotate_dir);
    }

    string ObjFactory(StringParameters inject)
    {
        string parameter = four_white_space + six_white_space + inject.param_name + ": " + inject.value;

        return parameter;
    }

    string ObjFactory(IntParameters inject)
    {
        string parameter = four_white_space + six_white_space + inject.param_name + ": !l " + inject.value;

        return parameter;
    }

    string ObjFactory(BoolParameters inject)
    {
        string parameter;
        if (inject.value)
        {
            parameter = four_white_space + six_white_space + inject.param_name + ": true";
        }
        else
        {
            parameter = four_white_space + six_white_space + inject.param_name + ": false";
        }
        return parameter;
    }

    string ObjFactory(FloatParameters inject)
    {
        string parameter = four_white_space + six_white_space + inject.param_name + ": " + inject.value;

        return parameter;
    }

    string ObjFactory(Vector3Parameters inject)
    {
        string parameter = four_white_space + six_white_space + inject.param_name + ": " + inject.value;

        return parameter;
    }

    public Vector3 GetControlPoint1()
    {
        if (firstPoint == null)
        {
            return transform.position;
        }
        Vector3 position = firstPoint.transform.position;
        //position.x = -position.x;

        return position;
    }

    public Vector3 GetControlPoint2()
    {
        if (secondPoint == null)
        {
            return transform.position;
        }

        Vector3 position = secondPoint.transform.position;
        //position.x = -position.x;

        return position;
    }
}
