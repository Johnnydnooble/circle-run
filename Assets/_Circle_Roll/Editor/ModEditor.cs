using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Es.InkPainter;

    [CustomEditor(typeof(GameManager))]
public class ModEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Color"))
        {
            Debug.Log("We pressed!");
        }
    }
}
