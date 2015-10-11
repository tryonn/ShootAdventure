using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;
        //map.MapGenerate();

        if (DrawDefaultInspector())
        {
            map.MapGenerate();
        }

        if (GUILayout.Button("Ganerator Map"))
        {
            map.MapGenerate();
        }

    }
}
