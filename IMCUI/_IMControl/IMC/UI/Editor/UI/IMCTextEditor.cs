using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    // TODO REVIEW
    // Have material live under text
    // move stencil mask into effects *make an efects top level element like there is
    // paragraph and character

    /// <summary>
    /// Editor class used to edit UI Labels.
    /// </summary>

    [CustomEditor(typeof(IMCText), true)]
    [CanEditMultipleObjects]
    public class IMCTextEditor : GraphicEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            IMCText text = (IMCText)target;
            //EditorGUI.BeginDisabledGroup(true);
            //text.ControlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", text.ControlType);
            //text.ContainerType = (ContainerType)EditorGUILayout.EnumPopup("ContainerType", text.ContainerType);
            //EditorGUI.EndDisabledGroup();
            text.customID = EditorGUILayout.TextField("CustomID", text.customID);
            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
