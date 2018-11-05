using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(VLHoloLensInitCameraBehaviour))]
public class VLHoloLensInitCameraBehaviourEditor : Editor
{
    private SerializedProperty workerBehaviourProp;
    private SerializedProperty holoLensTrackerBehaviourProp;
    private SerializedProperty initCameraProp;
    private SerializedProperty overwriteOnLoadProp;
    private SerializedProperty keepUprightProp;
    private SerializedProperty upAxisProp;

    void OnEnable()
    {
        this.workerBehaviourProp =
            serializedObject.FindProperty("workerBehaviour");
        this.holoLensTrackerBehaviourProp =
            serializedObject.FindProperty("holoLensTrackerBehaviour");
        this.initCameraProp =
            serializedObject.FindProperty("initCamera");
        this.overwriteOnLoadProp =
            serializedObject.FindProperty("overwriteOnLoad");
        this.keepUprightProp =
            serializedObject.FindProperty("keepUpright");
        this.upAxisProp =
            serializedObject.FindProperty("upAxis");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(this.workerBehaviourProp);
        EditorGUILayout.PropertyField(this.holoLensTrackerBehaviourProp);
        EditorGUILayout.PropertyField(this.initCameraProp);
        EditorGUILayout.PropertyField(this.overwriteOnLoadProp);
        EditorGUILayout.PropertyField(this.keepUprightProp);

        if (this.keepUprightProp.boolValue)
        {
            EditorGUILayout.PropertyField(this.upAxisProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}