using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using IMCUI.UI;
using UnityEditor.AnimatedValues;

namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCForm), true)]
    public class IMCFormEditor : Editor
    {
        SerializedProperty m_IsBlockerToggle;
        AnimBool m_IsBlockerToggleAnim;
        //SerializedProperty m_BlockerHierarchyStyle;
        //SerializedProperty m_BlockerShowStyle;
        void OnEnable()
        {
            m_IsBlockerToggle = serializedObject.FindProperty("m_isBlockerToggle");
            //m_BlockerHierarchyStyle = serializedObject.FindProperty("blockerHierarchyStyle");
            //m_BlockerShowStyle = serializedObject.FindProperty("blockerShowStyle");

            m_IsBlockerToggleAnim = new AnimBool(Repaint);
            SetAnimBools(true);
        }
        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_IsBlockerToggleAnim, !m_IsBlockerToggle.hasMultipleDifferentValues && m_IsBlockerToggle.boolValue == true, instant);
        }
        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }
        //public override void OnInspectorGUI()
        //{
        //    SetAnimBools(false);
        //    serializedObject.Update();
        //    IMCForm form = (IMCForm)target;
        //    form.customID = EditorGUILayout.TextField("customID", form.customID);
        //    form.ShowOnAwake = EditorGUILayout.Toggle("ShowOnAwake", form.ShowOnAwake);
        //    form.showTargetPosition = EditorGUILayout.Vector3Field("ShowTargetPosition", form.showTargetPosition);
        //    form.closeTargetPosition = EditorGUILayout.Vector3Field("CloseTargetPosition", form.closeTargetPosition);

        //    EditorGUILayout.PropertyField(m_IsBlockerToggle);
        //    if (EditorGUILayout.BeginFadeGroup(m_IsBlockerToggleAnim.faded))
        //    {
        //        EditorGUI.indentLevel++;
        //        EditorGUILayout.PropertyField(m_BlockerHierarchyStyle);
        //        EditorGUILayout.PropertyField(m_BlockerShowStyle);
        //        EditorGUI.indentLevel--;
        //    }
        //    EditorGUILayout.EndFadeGroup();
            

        //    serializedObject.ApplyModifiedProperties();

        //}
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}