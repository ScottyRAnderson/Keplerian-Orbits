using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static CelestialBody;

[CustomEditor(typeof(CelestialBody))][CanEditMultipleObjects]
public class CelestialBody_Inspector : Editor
{
    private static bool displayDetailData = true;

    private SerializedProperty bodyType;
    private SerializedProperty bodyName;
    private SerializedProperty radius;
    private SerializedProperty parentBody;

    private SerializedProperty orbitalPeriod;
    private SerializedProperty orbitalInclination;
    private SerializedProperty apoapsis;
    private SerializedProperty periapsis;
    private SerializedProperty argumentOfPeriapsis;
    private SerializedProperty axialTilt;
    private SerializedProperty rotationPeriod;

    private SerializedProperty orbitalProgress;
    private SerializedProperty rotationProgress;
    private SerializedProperty completeOrbits;
    private SerializedProperty rawOrbits;

    private void OnEnable()
    {
        bodyType = serializedObject.FindProperty("bodyType");
        bodyName = serializedObject.FindProperty("bodyName");
        radius = serializedObject.FindProperty("radius");
        parentBody = serializedObject.FindProperty("parentBody");

        orbitalPeriod = serializedObject.FindProperty("orbitalPeriod");
        orbitalInclination = serializedObject.FindProperty("orbitalInclination");
        apoapsis = serializedObject.FindProperty("apoapsis");
        periapsis = serializedObject.FindProperty("periapsis");
        argumentOfPeriapsis = serializedObject.FindProperty("argumentOfPeriapsis");
        axialTilt = serializedObject.FindProperty("axialTilt");
        rotationPeriod = serializedObject.FindProperty("rotationPeriod");

        orbitalProgress = serializedObject.FindProperty("orbitalProgress");
        rotationProgress = serializedObject.FindProperty("rotationProgress");
        completeOrbits = serializedObject.FindProperty("completeOrbits");
        rawOrbits = serializedObject.FindProperty("rawOrbits");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawOverview();
        if((BodyType)bodyType.enumValueIndex != BodyType.Star)
        {
            GUILayout.Space(5f);
            DrawCharacteristics();
            GUILayout.Space(5f);
            DrawDetails();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOverview()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorHelper.Header("Overview");
            using (new EditorGUI.DisabledScope(true)){
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }

            EditorGUILayout.PropertyField(bodyType);
            EditorGUILayout.PropertyField(bodyName);
            EditorGUILayout.PropertyField(radius);

            if((BodyType)bodyType.enumValueIndex != BodyType.Star){
                EditorGUILayout.PropertyField(parentBody);
            }
        }
    }

    private void DrawCharacteristics()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorHelper.Header("Characteristics");

            GUILayout.Label("Orbital", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(orbitalPeriod);
            EditorGUILayout.PropertyField(orbitalInclination);
            EditorGUILayout.PropertyField(apoapsis);
            EditorGUILayout.PropertyField(periapsis);
            EditorGUILayout.Slider(argumentOfPeriapsis, 0f, 360f);

            GUILayout.Space(5f);

            GUILayout.Label("Physical", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(axialTilt);
            if((BodyType)bodyType.enumValueIndex != BodyType.Satellite){
                EditorGUILayout.PropertyField(rotationPeriod);
            }
        }
    }

    private void DrawDetails()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorGUI.indentLevel++;
            displayDetailData = EditorHelper.Foldout(displayDetailData, "Details");
            if (displayDetailData)
            {
                if(Application.isPlaying){
                    GUI.enabled = false;
                }
                EditorGUILayout.Slider(orbitalProgress, 0f, 365f);
                if((BodyType)bodyType.enumValueIndex != BodyType.Satellite){
                    EditorGUILayout.Slider(rotationProgress, 0f, 360f);
                }

                GUI.enabled = false;
                EditorGUILayout.PropertyField(completeOrbits);
                EditorGUILayout.PropertyField(rawOrbits);
                GUI.enabled = true;
            }
            EditorGUI.indentLevel--;
        }
    }
}