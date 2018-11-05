using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(VLDebugImageBehaviour))]
public class VLDebugImageBehaviourEditor : Editor
{
    private SerializedProperty workerBehaviourProp;
    private SerializedProperty imageObjectProp;
    private SerializedProperty trackerProp;
    private SerializedProperty idProp;

    void OnEnable()
    {
        this.workerBehaviourProp = serializedObject.FindProperty("workerBehaviour");
        this.imageObjectProp = serializedObject.FindProperty("imageObject");
        this.trackerProp = serializedObject.FindProperty("tracker");
        this.idProp = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw a field for setting the worker object
        EditorGUILayout.PropertyField(this.workerBehaviourProp);

        // Draw a field for setting the image object
        EditorGUILayout.PropertyField(this.imageObjectProp);

        // Show drop down menu with the available trackers
        int previousTracker = this.trackerProp.enumValueIndex;
        EditorGUILayout.PropertyField(this.trackerProp);
        int newTracker = this.trackerProp.enumValueIndex;

        // 'Undefined' tracker selected?
        if (newTracker == (int)VLDebugImageBehaviour.Tracker.Undefined)
        {
            // Tracker not changed?
            if (previousTracker == newTracker)
            {
                // Display the previous ID as text
                this.idProp.stringValue = EditorGUILayout.TextField(
                    "Key", this.idProp.stringValue);
            }
            // Otherwise, tracker was changed
            else
            {
                // Display an empty ID, because we don't want to show the ID
                // value of the previous tracker
                this.idProp.stringValue = EditorGUILayout.TextField(
                    "Key", "");
            }
        }
        else
        {
            // Tracker not changed?
            int previousLabelIndex;
            if (previousTracker == newTracker)
            {
                // Get the index of the currently selected label
                previousLabelIndex = Array.IndexOf(
                    VLDebugImageBehaviour.labels[newTracker],
                    this.idProp.stringValue);
                if (previousLabelIndex < 0)
                {
                    previousLabelIndex = 0;
                }
            }
            // Otherwise, tracker was changed
            else
            {
                // Use the label with the index 0, because we don't want to
                // show the label of the previous tracker
                previousLabelIndex = 0;
            }

            // Show a drop down list with the available image labels
            int labelIndex = EditorGUILayout.Popup("Image", previousLabelIndex,
                VLDebugImageBehaviour.labels[newTracker]);
            this.idProp.stringValue =
                VLDebugImageBehaviour.labels[newTracker][labelIndex];
        }

        serializedObject.ApplyModifiedProperties();
    }
}