using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(VLInitCameraBehaviour))]
[CanEditMultipleObjects]
public class VLInitCameraBehaviourEditor : Editor
{
    [Serializable]
    private class InitPose
    {
        public string type;
        public float[] t = new float[3];
        public float[] q = new float[4];

        public InitPose()
        {
            this.type = "visionlib";
        }

        public void Set(Vector4 t, Quaternion q)
        {
            this.t[0] = t.x;
            this.t[1] = t.y;
            this.t[2] = t.z;

            this.q[0] = q.x;
            this.q[1] = q.y;
            this.q[2] = q.z;
            this.q[3] = q.w;
        }
    }

    private SerializedProperty workerBehaviourProp;
    private SerializedProperty initCameraProp;
    private SerializedProperty backgroundLayerProp;
    private SerializedProperty useLastValidPoseProp;
    private SerializedProperty overwriteOnLoadProp;

    private Matrix4x4 rotCamera = VLUnityCameraHelper.rotationZ0;

    private Vector4 t;
    private Quaternion q;
    private InitPose initPose = new InitPose();
    private string initPoseLabel = "\"initPose\":";
    private string initPoseString;
    private bool prettyPrint = true;

    private GUIContent content = new GUIContent();
    private Vector2 scrollPos = new Vector2();

    private void UpdateInitPose(Camera cam)
    {
        // Get the VisionLib transformation from the Unity camera
        VLUnityCameraHelper.CameraToVLPose(
            cam, this.rotCamera, out this.t, out this.q);

        // Convert the transformation into JSON
        this.initPose.Set(t, q);
        this.initPoseString = VLJsonUtility.ToJson(this.initPose,
            this.prettyPrint);
    }

    void OnEnable()
    {
        this.workerBehaviourProp =
            serializedObject.FindProperty("workerBehaviour");
        this.initCameraProp =
            serializedObject.FindProperty("initCamera");
        this.backgroundLayerProp =
            serializedObject.FindProperty("backgroundLayer");
        this.useLastValidPoseProp =
            serializedObject.FindProperty("useLastValidPose");
        this.overwriteOnLoadProp =
            serializedObject.FindProperty("overwriteOnLoad");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw all public field using the default layout
        EditorGUILayout.PropertyField(this.workerBehaviourProp);
        EditorGUILayout.PropertyField(this.initCameraProp);
        EditorGUILayout.PropertyField(this.backgroundLayerProp);
        EditorGUILayout.PropertyField(this.useLastValidPoseProp);
        EditorGUILayout.PropertyField(this.overwriteOnLoadProp);

        // Only show the VisionLib initPose, if one object is selected
        if (this.targets.Length == 1)
        {
            VLInitCameraBehaviour behaviour =
                (VLInitCameraBehaviour)this.targets[0];
            Camera cam = (behaviour.initCamera ?
                behaviour.initCamera :
                behaviour.GetComponent<Camera>());
            if (cam)
            {
                UpdateInitPose(cam);
                ReadOnlyTextField(this.initPoseLabel, this.initPoseString,
                     EditorStyles.helpBox);
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "\"initPose\" preview does not work with multi-editing.",
                MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ReadOnlyTextField(string labelText, string text,
        GUIStyle style)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(labelText,
                GUILayout.Width(EditorGUIUtility.labelWidth));

            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos,
                GUILayout.ExpandHeight(false));
            {
                // Explicitly set the size of the SelectableLabel. Otherwise
                // the ScrollView doesn't work correctly.
                this.content.text = text;
                Vector2 size = style.CalcSize(this.content);
                EditorGUILayout.SelectableLabel(text, style,
                    GUILayout.ExpandWidth(true),
                    GUILayout.MinWidth(size.x),
                    GUILayout.Height(size.y));
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();
    }
}