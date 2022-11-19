using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static ToggleElement;

[CustomEditor(typeof(ToggleElement))][CanEditMultipleObjects]
public class ToggleElement_Inspector : Editor
{
    private SerializedProperty toggled;
    private SerializedProperty imageComponent;
    private SerializedProperty toggleType;

    private SerializedProperty tintColor;
    private SerializedProperty interactedSize;
    private SerializedProperty interactedSprite;

    private void OnEnable()
    {
        toggled = serializedObject.FindProperty("toggled");
        imageComponent = serializedObject.FindProperty("imageComponent");
        toggleType = serializedObject.FindProperty("toggleType");
        tintColor = serializedObject.FindProperty("tintColor");
        interactedSize = serializedObject.FindProperty("interactedSize");
        interactedSprite = serializedObject.FindProperty("interactedSprite");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(toggled);
        EditorGUILayout.PropertyField(imageComponent);
        EditorGUILayout.PropertyField(toggleType);

        switch((ToggleType)toggleType.enumValueIndex)
        {
            case ToggleType.TintSprite:
                EditorGUILayout.PropertyField(tintColor);
                break;
            case ToggleType.ResizeSprite:
                EditorGUILayout.PropertyField(interactedSize);
                break;
            case ToggleType.SwapSprite:
                EditorGUILayout.PropertyField(interactedSprite);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}