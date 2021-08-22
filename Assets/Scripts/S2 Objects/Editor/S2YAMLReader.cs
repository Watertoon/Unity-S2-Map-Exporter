#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;


public class S2YAMLReader : MonoBehaviour
{

    static public Vector3 Vector3Parse(string value)
    {
        Vector3 vect = new Vector3();
        int value_index = 0;
        int end_value_index = 0;
        value_index = value.IndexOf(':', value_index + 1);
        end_value_index = value.IndexOf(',', end_value_index + 1);
        vect.x = float.Parse(value.Substring(value_index + 2, end_value_index - value_index - 2));

        value_index = value.IndexOf(':', value_index + 1);
        end_value_index = value.IndexOf(',', end_value_index + 1);
        vect.y = float.Parse(value.Substring(value_index + 2, end_value_index - value_index - 2));

        value_index = value.IndexOf(':', value_index + 1);
        end_value_index = value.IndexOf('}', end_value_index + 1);
        vect.z = float.Parse(value.Substring(value_index + 2, end_value_index - value_index - 2));

        return vect;
    }

    [MenuItem("S2/YAMLReader/ReadObj(DisableScript) %&m")]
    static public void ReadObjDisableScript()
    {
        //1: Read in YAML, dynamic size
        List<string> in_file = ReadYAMLFile("Assets/TextResources/MapImport.txt");

        //2: Parse YAML and instantiate a prefab one at a time for the objects
        ImportObjectsToEditor(in_file, false);


        //Debug.Log("reached end");
    }

    [MenuItem("S2/YAMLReader/ReadRails %&b")]
    static public void ReadRails()
    {
        //1: Read in YAML, dynamic size
        List<string> in_file = ReadYAMLFile("Assets/TextResources/MapImport.txt");

        //2: Parse YAML and instantiate a prefab one at a time for rails
        ImportRailsToEditor(in_file);


        //Debug.Log("reached end");
    }

    [MenuItem("S2/YAMLReader/ReadObj %&n")]
    static public void ReadObj()
    {
        //1: Read in YAML, dynamic size
        List<string> in_file = ReadYAMLFile("Assets/TextResources/MapImport.txt");

        //2: Parse YAML and instantiate a prefab one at a time for the objects
        ImportObjectsToEditor(in_file, true);


        //Debug.Log("reached end");
    }

    static private List<string> ReadYAMLFile(string filePath)
    {
        List<string> in_file = new List<string>();

        try
        {
            using (StreamReader yaml_in = new StreamReader(filePath))
            {
                while (!yaml_in.EndOfStream)
                {
                    in_file.Add(yaml_in.ReadLine());
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("MapImport.txt not found");
            Debug.Log(e);
        }
        return in_file;
    }

    static private void ImportRailsToEditor(List<string> in_file)
    {
        int main_index = 0;
        //Find Rail: header
        while (in_file[main_index][2] != 'R')
        {
            ++main_index;
        }
        ++main_index;

        //Mainloop
        bool end_of_rails = false;
        //Construct RailBase
        if (in_file[main_index][4] == '-')
        {
            //Calculate index for next '-' or 'R' to determine object size
            int next_obj_index = main_index;
            while (!end_of_rails)
            {
                while (!end_of_rails)
                {
                    ++next_obj_index;
                    if (next_obj_index == in_file.Count)
                    {
                        end_of_rails = true;
                        break;
                    }
                    if (in_file[next_obj_index][4] == '-')
                        break;
                    if (in_file[next_obj_index][2] == 'R')
                        end_of_rails = true;
                }
                //Find the number of railpoints 
                int numRailPoints = (next_obj_index - main_index - 14);
                //make rail base
                GameObject newRailBase;
                newRailBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                newRailBase.AddComponent<RailBase>();
                //add top params
                newRailBase.GetComponent<RailBase>().id = in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 10), in_file[main_index].Length - 10);
                ++main_index;

                newRailBase.name = newRailBase.GetComponent<RailBase>().id;

                newRailBase.GetComponent<RailBase>().isClosed = bool.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 16), in_file[main_index].Length - 16));
                ++main_index;
                newRailBase.GetComponent<RailBase>().isLadder = bool.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 16), in_file[main_index].Length - 16));
                ++main_index;
                newRailBase.GetComponent<RailBase>().isLinkDest = bool.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 18), in_file[main_index].Length - 18));
                ++main_index;
                newRailBase.GetComponent<RailBase>().layerConfigName = in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 23), in_file[main_index].Length - 23);
                ++main_index;
                ++main_index;
                ++main_index;
                newRailBase.GetComponent<RailBase>().priority = int.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 19), in_file[main_index].Length - 19));
                //add bottom params

                --next_obj_index;
                newRailBase.GetComponent<RailBase>().unitConfigName = in_file[next_obj_index].Substring(in_file[next_obj_index].Length - (in_file[next_obj_index].Length - 22), in_file[next_obj_index].Length - 22);

                //Vrail check
                if (newRailBase.GetComponent<RailBase>().unitConfigName == "Rail_VLift")
                    newRailBase.GetComponent<RailBase>().isVrail = true;

                --next_obj_index;
                //translate
                newRailBase.transform.position = Vector3Parse(in_file[next_obj_index].Substring(in_file[next_obj_index].Length - (in_file[next_obj_index].Length - 17), in_file[next_obj_index].Length - 17));
                Vector3 temp_translate = newRailBase.transform.position;
                temp_translate.x = -temp_translate.x;
                newRailBase.transform.position = temp_translate;

                --next_obj_index;
                newRailBase.transform.localScale = Vector3Parse(in_file[next_obj_index].Substring(in_file[next_obj_index].Length - (in_file[next_obj_index].Length - 13), in_file[next_obj_index].Length - 13));
                --next_obj_index;
                Vector3 in_rotation = Vector3Parse(in_file[next_obj_index].Substring(in_file[next_obj_index].Length - (in_file[next_obj_index].Length - 14), in_file[next_obj_index].Length - 14));
                newRailBase.transform.Rotate(0, 0, -in_rotation.z, Space.Self);
                newRailBase.transform.Rotate(0, -in_rotation.y, 0, Space.Self);
                newRailBase.transform.Rotate(in_rotation.x, 0, 0, Space.Self);
                --next_obj_index;
                newRailBase.GetComponent<RailBase>().railType = in_file[next_obj_index].Substring(in_file[next_obj_index].Length - (in_file[next_obj_index].Length - 16), in_file[next_obj_index].Length - 16);

                if(newRailBase.GetComponent<RailBase>().isVrail)
                {
                    numRailPoints = numRailPoints / 20;
                }
                else
                {
                    numRailPoints = numRailPoints / 17;
                }

                //make railpoints
                ++main_index;
                ++main_index;
                for(int i = 0; i < numRailPoints; ++i)
                {
                    GameObject newRailPoint;
                    newRailPoint = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    newRailPoint.AddComponent<RailPoint>();

                    if (newRailBase.GetComponent<RailBase>().isVrail)
                    {
                        newRailPoint.GetComponent<RailPoint>().checkPointHp = int.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 27), in_file[main_index].Length - 27));
                        ++main_index;
                    }

                    newRailPoint.GetComponent<RailPoint>().checkPointIndex = int.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 30), in_file[main_index].Length - 30));
                    ++main_index;
                    ++main_index;
                    GameObject newControlPoint1;
                    newControlPoint1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newControlPoint1.transform.position = Vector3Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 14), in_file[main_index].Length - 14));
                    //flip x
                    temp_translate = newControlPoint1.transform.position;
                    temp_translate.x = -temp_translate.x;
                    newControlPoint1.transform.position = temp_translate;

                    newRailPoint.GetComponent<RailPoint>().firstPoint = newControlPoint1;
                    ++main_index;
                    GameObject newControlPoint2;
                    newControlPoint2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newControlPoint2.transform.position = Vector3Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 14), in_file[main_index].Length - 14));
                    //flip x
                    temp_translate = newControlPoint2.transform.position;
                    temp_translate.x = -temp_translate.x;
                    newControlPoint2.transform.position = temp_translate;

                    newRailPoint.GetComponent<RailPoint>().secondPoint = newControlPoint2;
                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().id = in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 14), in_file[main_index].Length - 14);
                    ++main_index;
                    newRailPoint.name = newRailPoint.GetComponent<RailPoint>().id;
                    newRailPoint.GetComponent<RailPoint>().isLinkDest = bool.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 22), in_file[main_index].Length - 22));
                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().layerConfigName = in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 27), in_file[main_index].Length - 27);
                    ++main_index;
                    ++main_index;
                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().offsetX = float.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 19), in_file[main_index].Length - 19));
                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().offsetY = float.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 19), in_file[main_index].Length - 19));
                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().offsetZ = float.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 19), in_file[main_index].Length - 19));
                    ++main_index;
                    Vector3 railPointRotation = Vector3Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 18), in_file[main_index].Length - 18));
                    newRailPoint.transform.Rotate(0, 0, -railPointRotation.z, Space.Self);
                    newRailPoint.transform.Rotate(0, -railPointRotation.y, 0, Space.Self);
                    newRailPoint.transform.Rotate(railPointRotation.x, 0, 0, Space.Self);
                    ++main_index;
                    newRailPoint.transform.localScale = Vector3Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 17), in_file[main_index].Length - 17));
                    ++main_index;
                    if (newRailBase.GetComponent<RailBase>().isVrail)
                    {
                        newRailPoint.GetComponent<RailPoint>().slopeFillUp = int.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 26), in_file[main_index].Length - 26));
                        ++main_index;
                    }
                    newRailPoint.transform.position = Vector3Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 21), in_file[main_index].Length - 21));
                    //flip x
                    temp_translate = newRailPoint.transform.position;
                    temp_translate.x = -temp_translate.x;
                    newRailPoint.transform.position = temp_translate;

                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().unitConfigName = in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 26), in_file[main_index].Length - 26);
                    ++main_index;
                    newRailPoint.GetComponent<RailPoint>().useOffset = bool.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 21), in_file[main_index].Length - 21));
                    ++main_index;
                    if (newRailBase.GetComponent<RailBase>().isVrail)
                    {
                        newRailPoint.GetComponent<RailPoint>().useRotateDir = bool.Parse(in_file[main_index].Substring(in_file[main_index].Length - (in_file[main_index].Length - 24), in_file[main_index].Length - 24));
                        ++main_index;
                    }
                    //todo: find out how to properly add railpoint to railbase "rails"
                    newRailPoint.transform.parent = newRailBase.transform;
                    newControlPoint1.transform.parent = newRailPoint.transform;
                    newControlPoint2.transform.parent = newRailPoint.transform;
                    newRailBase.GetComponent<RailBase>().rails.Add(newRailPoint.GetComponent<RailPoint>());
                }
                
                if (!end_of_rails)
                {
                    next_obj_index += 5;
                    main_index = next_obj_index;
                }
            }
        }
    }

    static private void ImportObjectsToEditor(List<string> in_file, bool enableScript)
    {
        int main_index = 0;
        //Find Obj: header, assume this is copy and pasted exactly from toolbox
        while (in_file[main_index][2] != 'O')
        {
            ++main_index;
        }
        ++main_index;

        //Mainloop
        List<IntParameters> int_params = new List<IntParameters>();
        List<FloatParameters> float_params = new List<FloatParameters>();
        List<BoolParameters> bool_params = new List<BoolParameters>();
        List<StringParameters> string_params = new List<StringParameters>();
        Vector3 in_position = new Vector3();
        Vector3 in_rotation = new Vector3();
        Vector3 in_scale = new Vector3();
        List<string> link_head = new List<string>();
        List<int> link_head_size = new List<int>();
        List<string> obj_ids = new List<string>();

        bool end_of_objs = false;

        //Check if index at a nice '-'
        if (in_file[main_index][4] == '-')
        {
            //Calculate index for next '-' or 'R' to determine object size
            int next_obj_index = main_index;
            while (!end_of_objs)
            {
                while (!end_of_objs)
                {
                    ++next_obj_index;
                    if (in_file[next_obj_index][4] == '-')
                        break;
                    if (in_file[next_obj_index][2] == 'R')
                        end_of_objs = true;
                }

                //Iterate through object to find important indexs
                for(int i = 0; i < next_obj_index - main_index; ++i)
                {
                    //get name of parameter
                    int name_length = in_file[i + main_index].IndexOf(':');
                    int name_offset = 5;

                    string name = in_file[i + main_index].Substring(name_offset + 1, name_length - name_offset - 1);

                    if (name[0] == 'L')
                    {
                        if (name == "Links")
                        {
                            //find a header
                            bool finished_links = false;
                            int link_head_size_num = 0;
                            while (!finished_links)
                            {
                                ++i;
                                if (in_file[i + main_index][6] == ' ' && in_file[i + main_index][8] != ' ')
                                {
                                    int head_index = in_file[i + main_index].IndexOf(':');
                                    link_head.Add(in_file[i + main_index].Substring(8, head_index - 8));
                                    if (link_head_size_num != 0)
                                    {
                                        link_head_size.Add(link_head_size_num);
                                        link_head_size_num = 0;
                                    }
                                }
                                //count link, capture id
                                else if(in_file[i + main_index][10] == '-')
                                {
                                    ++link_head_size_num;
                                    int id_start = in_file[i + main_index].IndexOf(':');
                                    id_start = in_file[i + main_index].IndexOf(':', id_start + 1);
                                    int id_end = in_file[i + main_index].IndexOf(',', id_start);
                                    obj_ids.Add(in_file[i + main_index].Substring(id_start + 2, id_end - id_start - 2));
                                }
                                //else reset and finish
                                else
                                {
                                    finished_links = true;
                                    if(link_head_size_num != 0)
                                        link_head_size.Add(link_head_size_num);
                                    name_length = in_file[i + main_index].IndexOf(':');
                                    name = in_file[i + main_index].Substring(name_offset + 1, name_length - name_offset - 1);
                                }
                            }
                        }
                    }

                    string value = in_file[i + main_index].Substring(name_length + 2);

                    //determine value type
                    if(value.Contains("!l"))
                    {
                        int_params.Add(new IntParameters(name, int.Parse(value.Substring(3))));
                    }
                    else if (value.Contains("{X"))
                    {
                        if (name == "Rotate")
                        {
                            in_rotation = Vector3Parse(value);
                        }
                        else if (name == "Translate")
                        {
                            in_position = Vector3Parse(value);
                            in_position.x = -in_position.x;
                        }
                        else
                        {
                            in_scale = Vector3Parse(value);
                        }
                    }
                    else if (value.Contains("."))
                    {
                        try
                        {
                            float_params.Add(new FloatParameters(name, float.Parse(value)));
                        }
                        catch(Exception e)
                        {
                            string_params.Add(new StringParameters(name, value));
                        }
                    }
                    else if (value.Contains("true"))
                    {
                        bool_params.Add(new BoolParameters(name, bool.Parse(value)));
                    }
                    else if (value.Contains("false"))
                    {
                        bool_params.Add(new BoolParameters(name, bool.Parse(value)));
                    }
                    else if (!value.Contains("null"))
                    {
                        string_params.Add(new StringParameters(name, value));
                    }
                }

                int unit_config_index = 0;

                //construct object
                //find model name
                for (int i = 0; i < string_params.Count; ++i)
                {
                    if(string_params[i].param_name == "UnitConfigName")
                    {
                        unit_config_index = i;
                        break;
                    }
                }

                //Check for file
                
                //make object
                GameObject new_obj;
                bool is_area = false;
                if (File.Exists("Assets/Models/OE/Resources/" + string_params[unit_config_index].value + ".dae") || File.Exists("Assets/Prefabs/OE/Resources/" + string_params[unit_config_index].value + ".prefab") || File.Exists("Assets/Models/S2/Resources/" + string_params[unit_config_index].value + ".dae") || File.Exists("Assets/Prefabs/S2/Resources/" + string_params[unit_config_index].value + ".prefab"))
                {
                    new_obj = Instantiate(Resources.Load(string_params[unit_config_index].value, typeof(GameObject))) as GameObject;
                }
                else if (File.Exists("Assets/Models/OE/Resources/" + string_params[unit_config_index].value + "/" + string_params[unit_config_index].value + ".dae") || File.Exists("Assets/Prefabs/OE/Resources/" + string_params[unit_config_index].value + "/" + string_params[unit_config_index].value + ".prefab") || File.Exists("Assets/Models/S2/Resources/" + string_params[unit_config_index].value + "/" + string_params[unit_config_index].value + ".dae") || File.Exists("Assets/Prefabs/S2/Resources/" + string_params[unit_config_index].value + "/" + string_params[unit_config_index].value + ".prefab"))
                {
                    new_obj = Instantiate(Resources.Load(string_params[unit_config_index].value + "/" + string_params[unit_config_index].value, typeof(GameObject))) as GameObject;
                }
                else
                {
                    if (string_params[unit_config_index].value.Contains("Area") || string_params[unit_config_index].value.Contains("AirWall") || string_params[unit_config_index].value.Contains("MapPaintable"))
                    {
                        new_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        is_area = true;
                        in_scale.x = in_scale.x * 10;
                        in_scale.y = in_scale.y * 10;
                        in_scale.z = in_scale.z * 10;
                        //Find transparent mat by area type
                        if(string_params[unit_config_index].value.Contains("Stage"))
                            new_obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Transparent Mat/Red");
                        else if(string_params[unit_config_index].value.Contains("Yellow"))
                            new_obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Transparent Mat/Yellow");
                        else if (string_params[unit_config_index].value.Contains("Pink"))
                            new_obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Transparent Mat/Pink");
                        else if (string_params[unit_config_index].value.Contains("Enemy"))
                            new_obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Transparent Mat/Purple");
                        else
                            new_obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Transparent Mat/White");
                    }
                    else
                    {
                        new_obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    }
                }

                if (new_obj != null)
                {
                    new_obj.name = string_params[unit_config_index].value;
                }

                //Add params
                new_obj.AddComponent<ObjectBase>();

                if (is_area)
                    new_obj.GetComponent<ObjectBase>().isZone = true;

                new_obj.GetComponent<ObjectBase>().unitConfigName = string_params[unit_config_index].value;
                new_obj.transform.position = in_position;
                //Convert local ZYX to ZXY
                new_obj.transform.Rotate(0, 0, -in_rotation.z, Space.Self);
                new_obj.transform.Rotate(0, -in_rotation.y, 0, Space.Self);
                new_obj.transform.Rotate(in_rotation.x, 0, 0, Space.Self);

                new_obj.transform.localScale = in_scale;

                new_obj.GetComponent<ObjectBase>().destUnitId = new List<string>(obj_ids);
                new_obj.GetComponent<ObjectBase>().linkHeaders = new List<string>(link_head);
                new_obj.GetComponent<ObjectBase>().linkHeaderSize = new List<int>(link_head_size);

                int param_offset = 0;
                new_obj.GetComponent<ObjectBase>().intParams = new IntParameters[int_params.Count];
                for (int i = 0; i < int_params.Count; ++i)
                {
                    new_obj.GetComponent<ObjectBase>().intParams[i] = int_params[i];
                }
                new_obj.GetComponent<ObjectBase>().boolParams = new BoolParameters[bool_params.Count - 1];
                for (int i = 0; i < bool_params.Count; ++i)
                {
                    if (bool_params[i].param_name[0] == 'I')
                    {
                        if (bool_params[i].param_name == "IsLinkDest")
                        {
                            new_obj.GetComponent<ObjectBase>().isLinkDest = bool_params[i].value;
                            ++param_offset;
                        }
                        else
                        {
                            new_obj.GetComponent<ObjectBase>().boolParams[i - param_offset] = bool_params[i];
                        }
                    }
                    else
                    {
                        new_obj.GetComponent<ObjectBase>().boolParams[i - param_offset] = bool_params[i];
                    }
                }
                param_offset = 0;
                new_obj.GetComponent<ObjectBase>().floatParams = new FloatParameters[float_params.Count];
                for (int i = 0; i < float_params.Count; ++i)
                {
                    new_obj.GetComponent<ObjectBase>().floatParams[i] = float_params[i];
                }
                new_obj.GetComponent<ObjectBase>().stringParams = new StringParameters[string_params.Count - 3];
                for (int i = 0; i < string_params.Count; ++i)
                {
                    if(i != unit_config_index)
                    {
                        if (string_params[i].param_name[0] == 'I' || string_params[i].param_name[0] == 'L')
                        {
                            if (string_params[i].param_name == "Id")
                            {
                                new_obj.GetComponent<ObjectBase>().id = string_params[i].value;
                                ++param_offset;
                            }
                            else if (string_params[i].param_name == "LayerConfigName")
                            {
                                new_obj.GetComponent<ObjectBase>().layerConfigName = string_params[i].value;
                                ++param_offset;
                            }
                            else
                            {
                                new_obj.GetComponent<ObjectBase>().stringParams[i - param_offset] = string_params[i];
                            }
                        }
                        else
                        {
                            new_obj.GetComponent<ObjectBase>().stringParams[i - param_offset] = string_params[i];
                        }
                    }
                }

                if(!enableScript)
                {
                    new_obj.GetComponent<ObjectBase>().enabled = false;
                }

                //clear
                int_params.Clear();
                float_params.Clear();
                bool_params.Clear();
                string_params.Clear();
                link_head.Clear();
                link_head_size.Clear();
                obj_ids.Clear();

                main_index = next_obj_index;
            }
        }
    }
}



#endif