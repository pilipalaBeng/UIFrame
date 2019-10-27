using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using IMCUI.UI;

namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCTabView), true)]
    public class IMCTabViewEditor : Editor
    {
        //private SerializedProperty m_toggles;
        //private SerializedProperty m_contents;

        //public void OnEnable()
        //{
        //    m_toggles = serializedObject.FindProperty("toggles");
        //    m_contents = serializedObject.FindProperty("contents");
        //}
        public override void OnInspectorGUI()
        {
            //IMCTabView tabView = (IMCTabView)target;
            //EditorGUI.BeginDisabledGroup(true);
            //tabView.controlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", tabView.controlType);
            //tabView.containerType = (ContainerType)EditorGUILayout.EnumPopup("ContaionerType", tabView.containerType);
            //EditorGUI.EndDisabledGroup();


            //tabView.parent = (IMCUIBehaviour)EditorGUILayout.ObjectField("Parent", tabView.parent, typeof(IMCUIBehaviour), true);
            //tabView.customID = EditorGUILayout.TextField("CustomID", tabView.customID);
            base.OnInspectorGUI();
            //serializedObject.Update();
            //EditorGUILayout.PropertyField(m_toggles);
            //EditorGUILayout.PropertyField(m_contents);
            //serializedObject.ApplyModifiedProperties();
           
        }
    }
}