using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using IMCUI.UI;

namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCSwitchButton), true)]
    public class IMCSwitchButtonEditor : Editor
    {
        //SerializedProperty m_isOn;
        SerializedProperty m_onClick;
        SerializedProperty m_moveSpeed;
        SerializedProperty m_onContent;
        SerializedProperty m_offContent;
        SerializedProperty m_onColor;
        SerializedProperty m_offColor;

        SerializedProperty m_isContent;
        SerializedProperty m_isColor;
        AnimBool m_ShowContent;
        AnimBool m_ShowColor;
        void OnEnable()
        {
            //m_isOn = serializedObject.FindProperty("m_isOn");
            m_onClick = serializedObject.FindProperty("onClick");
            m_moveSpeed = serializedObject.FindProperty("m_moveSpeed");
            m_onContent = serializedObject.FindProperty("m_onContent");
            m_offContent = serializedObject.FindProperty("m_offContent");
            m_onColor = serializedObject.FindProperty("m_onColor");
            m_offColor = serializedObject.FindProperty("m_offColor");

            m_isContent = serializedObject.FindProperty("m_isContent");
            m_isColor = serializedObject.FindProperty("m_isColor");
            m_ShowContent = new AnimBool(Repaint);
            m_ShowColor = new AnimBool(Repaint);
            SetAnimBools(true);
        }
        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowContent, !m_isContent.hasMultipleDifferentValues && m_isContent.boolValue == true, instant);
            SetAnimBool(m_ShowColor, !m_isColor.hasMultipleDifferentValues && m_isColor.boolValue == true, instant);
        }
        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);
            serializedObject.Update();

            IMCSwitchButton switchButton = (IMCSwitchButton)target;
            switchButton.customID = EditorGUILayout.TextField("CustomID", switchButton.customID);
            switchButton.isOn = EditorGUILayout.ToggleLeft("isOn", switchButton.isOn);
            EditorGUILayout.PropertyField(m_moveSpeed);

            EditorGUILayout.PropertyField(m_isContent);
            if (EditorGUILayout.BeginFadeGroup(m_ShowContent.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_offContent);
                EditorGUILayout.PropertyField(m_onContent);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_isColor);
            if (EditorGUILayout.BeginFadeGroup(m_ShowColor.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_offColor);
                EditorGUILayout.PropertyField(m_onColor);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_onClick);
            serializedObject.ApplyModifiedProperties();
        }

    }
}