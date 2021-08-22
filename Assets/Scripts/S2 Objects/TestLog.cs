using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 local = transform.worldToLocalMatrix;
        Quaternion rot_test = local.rotation;
        Vector3 rot_angle = rot_test.eulerAngles;

        // y_angle = Vector3.Angle(transform.up, transform.position);
        //float x_angle = Vector3.Angle(transform.right, transform.position);
        //float z_angle = Vector3.Angle(transform.forward, transform.position);
        Debug.Log(rot_angle.x);
        Debug.Log(rot_angle.y);
        Debug.Log(rot_angle.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
