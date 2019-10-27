using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCToggle), true)]
    [CanEditMultipleObjects]
    public class IMCToggleEditor : SelectableEditor
    {
        SerializedProperty m_OnValueChangedProperty;
        SerializedProperty m_TransitionProperty;
        //SerializedProperty m_BackGroundProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;
        SerializedProperty m_isPlayAudio;
        SerializedProperty m_clip;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_TransitionProperty = serializedObject.FindProperty("toggleTransition");
            //m_BackGroundProperty = serializedObject.FindProperty("backGround");
            m_GraphicProperty = serializedObject.FindProperty("graphic");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");

            m_isPlayAudio = serializedObject.FindProperty("m_isPlayAudio");
            m_clip = serializedObject.FindProperty("m_clip");
        }

        public override void OnInspectorGUI()
        {
            IMCToggle toggle = (IMCToggle)target;
            //EditorGUI.BeginDisabledGroup(true);
            //toggle.ControlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", toggle.ControlType);
            //toggle.ContainerType = (ContainerType)EditorGUILayout.EnumPopup("ContainerType", toggle.ContainerType);
            //EditorGUI.EndDisabledGroup();
            toggle.customID = EditorGUILayout.TextField("CustomID", toggle.customID);
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            EditorGUILayout.PropertyField(m_TransitionProperty);
            //EditorGUILayout.PropertyField(m_BackGroundProperty);
            EditorGUILayout.PropertyField(m_GraphicProperty);
            EditorGUILayout.PropertyField(m_GroupProperty);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_isPlayAudio);
            EditorGUILayout.PropertyField(m_clip);
            // Draw the event notification options
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
