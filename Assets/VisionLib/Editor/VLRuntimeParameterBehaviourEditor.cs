using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(VLRuntimeParameterBehaviour))]
public class VLRuntimeParameterBehaviourEditor : Editor
{
    private SerializedProperty workerBehaviourProp;
    private SerializedProperty parameterNameProp;
    private SerializedProperty parameterTypeProp;
    private SerializedProperty changingProp;
    private SerializedProperty stringValueChangedEventProp;
    private SerializedProperty intValueChangedEventProp;
    private SerializedProperty floatValueChangedEventProp;
    private SerializedProperty boolValueChangedEventProp;

    void OnEnable()
    {
        this.workerBehaviourProp =
            serializedObject.FindProperty("workerBehaviour");
        this.parameterNameProp =
            serializedObject.FindProperty("parameterName");
        this.parameterTypeProp =
            serializedObject.FindProperty("parameterType");
        this.changingProp =
            serializedObject.FindProperty("changing");

        this.stringValueChangedEventProp =
            serializedObject.FindProperty("stringValueChanged");
        this.intValueChangedEventProp =
            serializedObject.FindProperty("intValueChangedEvent");
        this.floatValueChangedEventProp =
            serializedObject.FindProperty("floatValueChangedEvent");
        this.boolValueChangedEventProp =
            serializedObject.FindProperty("boolValueChangedEvent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(this.workerBehaviourProp);
        EditorGUILayout.PropertyField(this.parameterNameProp);
        EditorGUILayout.PropertyField(this.parameterTypeProp);
        EditorGUILayout.PropertyField(this.changingProp);

        VLRuntimeParameterBehaviour.ParameterType parameterType =
            (VLRuntimeParameterBehaviour.ParameterType)
            this.parameterTypeProp.enumValueIndex;
        switch (parameterType)
        {
        case VLRuntimeParameterBehaviour.ParameterType.String:
            EditorGUILayout.PropertyField(this.stringValueChangedEventProp);
            break;
        case VLRuntimeParameterBehaviour.ParameterType.Int:
            EditorGUILayout.PropertyField(this.intValueChangedEventProp);
            EditorGUILayout.PropertyField(this.floatValueChangedEventProp);
            break;
        case VLRuntimeParameterBehaviour.ParameterType.Float:
            EditorGUILayout.PropertyField(this.floatValueChangedEventProp);
            break;
        case VLRuntimeParameterBehaviour.ParameterType.Bool:
            EditorGUILayout.PropertyField(this.boolValueChangedEventProp);
            break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}