using System;
using System.Collections.Generic;
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

[SelectionBaseAttribute]
public class ObjectBase : MonoBehaviour
{
    //Object Headers&footers
    private const string header = "    - ";
    private const string six_white_space = "      ";

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
    public List<string> linkHeaders;
    [Tooltip("Each element represents the number of links under each header, they will be paired in chronological order andmust match the amount of elements.")]
    public List<int> linkHeaderSize;
    [Header("Links")]
    [Tooltip("Size needs to be the total number under linkHeaderSize.")]
    public List<string> destUnitId;

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

    private float ToDeg = 180f / 3.141592653f; 

    // Start is called before the first frame update
    void Start()
    {
        Quaternion rot = transform.rotation;
        //rot.x = -rot.x;
        //rot.y = -rot.y;
        //rot.z = -rot.z;
        transform.rotation = rot;
        //transform.rotation.Set(-transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        Matrix4x4 local = transform.worldToLocalMatrix;
        Quaternion q = local.rotation;
        Vector3 rotation_rad = threeaxisrot(-2 * (q.y * q.z - q.w * q.x),
                q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                2 * (q.x * q.z + q.w * q.y),
                -2 * (q.x * q.y - q.w * q.z),
                q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);
        Vector3 rotation;
        rotation.x = rotation_rad.x * 180.0f / 3.1415926f;
        rotation.y = rotation_rad.y * 180.0f / 3.1415926f;
        rotation.z = rotation_rad.z * 180.0f / 3.1415926f;
        //rotation.x = -rotation.x;
        if(rotation.x > 180)
        {
            rotation.x -= 360;
        }
        if(rotation.x < -180)
        {
            rotation.x += 360;
        }
        if (rotation.y > 180)
        {
            rotation.y -= 360;
        }
        if (rotation.y < -180)
        {
            rotation.y += 360;
        }
        if (rotation.z > 180)
        {
            rotation.z -= 360;
        }
        if (rotation.z < -180)
        {
            rotation.z += 360;
        }

        //Debug.Log(x_angle);
        //Debug.Log(y_angle);
        //Debug.Log(z_angle);
        //Stolen from https://forum.unity.com/threads/rotation-order.13469/ lol
        /*Vector3 rotation = threeaxisrot(-2 * (q.x * q.y - q.w * q.z),
                q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                2 * (q.y * q.z + q.w * q.x),
                -2 * (q.x * q.z - q.w * q.y),
                q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
        //Angle returned in radians, gotta convert to degrees
        rotation.Set(rotation.x * ToDeg, rotation.y * ToDeg, rotation.z * ToDeg);
        //transform.Rotate(0, 0, -z_angle, Space.Self);
        //transform.Rotate(0, -y_angle, 0, Space.Self);
        //transform.Rotate(x_angle, 0, 0, Space.Self);
        //Vector3 test = rot_test.eulerAngles;
        rotation.x = -rotation.x;*/

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
            link_xml_size += (destUnitId.Count);
            //1 line per header
            link_xml_size += (linkHeaderSize.Count);
        }

        return link_xml_size;
    }

    bool CheckLinkSectionLength()
    {
        int link_size = 0;
        //Add link length if valid
        if (linkHeaders.Count == 0) { return false; }

        if (linkHeaders.Count == linkHeaderSize.Count)
        {
            for (int i = 0; i < linkHeaderSize.Count; i++)
            {
                link_size += (linkHeaderSize[i]);
            }
            if (link_size == destUnitId.Count)
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
            for(int i = 0; i < linkHeaders.Count; i++)
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
            xml[xml_index++] = six_white_space + "Links: {}";
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

    //Stolen from https://forum.unity.com/threads/rotation-order.13469/ lol
    //Function to convert rotation axis

    Vector3 threeaxisrot(float r11, float r12, float r21, float r31, float r32)
    {
        Vector3 ret = new Vector3();
        ret.x = Mathf.Atan2(r31, r32);
        ret.y = Mathf.Asin(r21);
        ret.z = Mathf.Atan2(r11, r12);
        return ret;
    }

}
