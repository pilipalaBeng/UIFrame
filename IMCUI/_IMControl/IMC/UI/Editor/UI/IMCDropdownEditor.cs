using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCDropdown), true)]
    [CanEditMultipleObjects]
    public class IMCDropdownEditor : SelectableEditor
    {
        SerializedProperty m_Template;
        SerializedProperty m_CaptionText;
        SerializedProperty m_CaptionImage;
        SerializedProperty m_ItemText;
        SerializedProperty m_ItemImage;
        SerializedProperty m_OnSelectionChanged;
        SerializedProperty m_OnSelectionHideChanged;
        SerializedProperty m_OnSelectChanged;
        SerializedProperty m_Value;
        SerializedProperty m_Options;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Template = serializedObject.FindProperty("m_Template");
            m_CaptionText = serializedObject.FindProperty("m_CaptionText");
            m_CaptionImage = serializedObject.FindProperty("m_CaptionImage");
            m_ItemText = serializedObject.FindProperty("m_ItemText");
            m_ItemImage = serializedObject.FindProperty("m_ItemImage");
            m_OnSelectionChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_OnSelectionHideChanged = serializedObject.FindProperty("m_OnHideChanged");
            m_OnSelectChanged = serializedObject.FindProperty("m_OnSelect");
            m_Value = serializedObject.FindProperty("m_Value");
            m_Options = serializedObject.FindProperty("m_Options");
        }

        public override void OnInspectorGUI()
        {
            IMCDropdown dropdown = (IMCDropdown)target;
            //EditorGUI.BeginDisabledGroup(true);
            //dropdown.ControlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", dropdown.ControlType);
            //dropdown.ContainerType = (ContainerType)EditorGUILayout.EnumPopup("ContainerType", dropdown.ContainerType);
            //EditorGUI.EndDisabledGroup();
            dropdown.customID = EditorGUILayout.TextField("CustomID", dropdown.customID);
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Template);
            EditorGUILayout.PropertyField(m_CaptionText);
            EditorGUILayout.PropertyField(m_CaptionImage);
            EditorGUILayout.PropertyField(m_ItemText);
            EditorGUILayout.PropertyField(m_ItemImage);
            EditorGUILayout.PropertyField(m_Value);
            EditorGUILayout.PropertyField(m_Options);
            EditorGUILayout.PropertyField(m_OnSelectionChanged);
            EditorGUILayout.PropertyField(m_OnSelectionHideChanged);
            EditorGUILayout.PropertyField(m_OnSelectChanged);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
