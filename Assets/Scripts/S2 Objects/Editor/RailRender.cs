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
        if (draw.rails == null || draw == null) { return; }

        if (draw.rails.Count > 1)
        {
            for (int i = 0; i < draw.rails.Count; i++)
                {
                if(draw.rails[i] == null) { return; }

                if(draw.railType[0] == 'B')
                {
                    if (i == draw.rails.Count - 1 && draw.isClosed)
                    {
                        Handles.DrawBezier(draw.rails[i].transform.position, draw.rails[0].transform.position, draw.rails[i].GetControlPoint2(), draw.rails[0].GetControlPoint1(), Color.red, null, 2f);
                    }
                    else if (i != draw.rails.Count - 1)
                    {
                        if (draw.rails[i + 1] == null) { return; }
                        Handles.DrawBezier(draw.rails[i].transform.position, draw.rails[i + 1].transform.position, draw.rails[i].GetControlPoint2(), draw.rails[i + 1].GetControlPoint1(), Color.red, null, 2f);
                    }
                }
                else if (draw.railType[0] == 'L')
                {
                    if (i == draw.rails.Count - 1 && draw.isClosed)
                    {
                        Handles.DrawLine(draw.rails[i].transform.position, draw.rails[0].transform.position);
                    }
                    else if (i != draw.rails.Count - 1)
                    {
                        if (draw.rails[i + 1] == null) { return; }
                        Handles.DrawLine(draw.rails[i].transform.position, draw.rails[i + 1].transform.position);
                    }
                }
            }
        }
    }


    void OnEnable()
    {
        //Debug.Log("OnEnable");
        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    void OnDisable()
    {
        //Debug.Log("OnEnable");
        //SceneView.duringSceneGui += OnSceneViewGUI;
    }
}
