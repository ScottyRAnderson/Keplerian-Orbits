using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CelestialManager))]
public class CelestialManager_Inspector : Editor
{
    private static bool displaySystemLoader;

    private CelestialManager managerBase;

    private SerializedProperty simulated;
    private SerializedProperty systemScrub;
    private SerializedProperty systemStar;
    private SerializedProperty systemBodyData;

    private void OnEnable()
    {
        managerBase = target as CelestialManager;

        simulated = serializedObject.FindProperty("simulated");
        systemScrub = serializedObject.FindProperty("systemScrub");
        systemStar = serializedObject.FindProperty("systemStar");
        systemBodyData = serializedObject.FindProperty("systemBodyData");
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(managerBase, "Celestial Manager Updated");

        serializedObject.Update();
        DrawOverview();
        DrawSystemLoader();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOverview()
    {
        EditorGUILayout.PropertyField(simulated);
        EditorGUILayout.Slider(systemScrub, 0f, 1f);
        EditorGUILayout.PropertyField(systemStar);
    }

    private void DrawSystemLoader()
    {
        displaySystemLoader = EditorGUILayout.Foldout(displaySystemLoader, "Load System");
        if(displaySystemLoader)
        {
            EditorGUILayout.PropertyField(systemBodyData);
            if(GUILayout.Button("Load System Data"))
            {
                managerBase.LoadSystemData((TextAsset)systemBodyData.objectReferenceValue);
                EditorUtility.SetDirty(managerBase);
            }
        }
    }
}