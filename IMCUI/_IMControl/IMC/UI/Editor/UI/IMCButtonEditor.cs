using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCButton), true)]
    [CanEditMultipleObjects]
    public class IMCButtonEditor : SelectableEditor
    {
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_isZoomAnimation;
        SerializedProperty m_isPlayAudio;
        SerializedProperty m_clip;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_isZoomAnimation = serializedObject.FindProperty("m_isZoomAnimation");

            m_isPlayAudio = serializedObject.FindProperty("m_isPlayAudio");
            m_clip = serializedObject.FindProperty("m_clip");
            SerializedPropertyType temp = m_OnClickProperty.propertyType;
        }

        public override void OnInspectorGUI()
        {
            IMCButton button = (IMCButton)target;
            //EditorGUI.BeginDisabledGroup(true);
            //button.controlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", button.ControlType);
            //button.ContainerType = (ContainerType)EditorGUILayout.EnumPopup("ContainerType", button.ContainerType);
            //EditorGUI.EndDisabledGroup();
            button.customID = EditorGUILayout.TextField("CustomID", button.customID);
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_isZoomAnimation);
            EditorGUILayout.PropertyField(m_isPlayAudio);
            EditorGUILayout.PropertyField(m_clip);
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
