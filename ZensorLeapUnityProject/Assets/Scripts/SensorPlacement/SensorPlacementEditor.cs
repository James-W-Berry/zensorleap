using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SensorRenderer))]
public class SensorPlacementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SensorRenderer myScript = (SensorRenderer)target;
        if(GUILayout.Button("Build Object"))
        {
            myScript.OnObjectPlaced();
        }
    }
}
