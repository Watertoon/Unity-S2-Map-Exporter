using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailBase))]
public class DrawRailPoint : Editor
{
    // Start is called before the first frame update
    void OnSceneViewGUI(SceneView sv)
    {
        RailBase draw = target as RailBase;
        if (draw == null) { return; }

        if (draw.rails.Length > 1)
        {
                
            for (int i = 0; i < draw.rails.Length; i++)
                {
                if(draw.railType == "Bezier")
                {
                    if (i == draw.rails.Length - 1 && draw.isClosed)
                    {
                        Handles.DrawBezier(draw.rails[i].transform.position, draw.rails[0].transform.position, draw.rails[i].GetControlPoint2(), draw.rails[0].GetControlPoint1(), Color.red, null, 2f);
                    }
                    else if (i != draw.rails.Length - 1)
                    {
                        Handles.DrawBezier(draw.rails[i].transform.position, draw.rails[i + 1].transform.position, draw.rails[i].GetControlPoint2(), draw.rails[i + 1].GetControlPoint1(), Color.red, null, 2f);
                    }
                }
                else if (draw.railType == "Linear")
                {
                    if (i == draw.rails.Length - 1 && draw.isClosed)
                    {
                        Handles.DrawLine(draw.rails[i].transform.position, draw.rails[0].transform.position);
                    }
                    else if (i != draw.rails.Length - 1)
                    {
                        Handles.DrawLine(draw.rails[i].transform.position, draw.rails[i + 1].transform.position);
                    }
                }
            }
        }
    }


    void OnEnable()
    {
        Debug.Log("OnEnable");
        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    void OnDisable()
    {
        Debug.Log("OnEnable");
        SceneView.duringSceneGui += OnSceneViewGUI;
    }
}
