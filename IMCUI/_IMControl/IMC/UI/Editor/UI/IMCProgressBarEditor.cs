using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;
using IMCProgressBar = IMCUI.UI.IMCProgressBar;
namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCProgressBar), true)]
    public class IMCProgressBarEditor : Editor
    {
        GUIContent m_SpriteContent;
        GUIContent m_BackgroundContent;

        SerializedProperty m_Sprite;
        SerializedProperty m_Background;
        SerializedProperty m_IsTempViewCustomSize;
        SerializedProperty m_IsTempViewPosition;
        AnimBool m_ShowTempViewCustomSize;
        AnimBool m_ShowTempViewPosition;
        void OnEnable()
        {
            m_SpriteContent = new GUIContent("Sprite");
            m_BackgroundContent = new GUIContent("Background");

            m_Sprite = serializedObject.FindProperty("m_sprite");
            m_Background = serializedObject.FindProperty("m_background");
            m_IsTempViewCustomSize = serializedObject.FindProperty("isTempViewCustomSize");
            m_IsTempViewPosition = serializedObject.FindProperty("isTempViewPosition");
            m_ShowTempViewCustomSize = new AnimBool(Repaint);
            m_ShowTempViewPosition = new AnimBool(Repaint);
            SetAnimBools(true);
        }
        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowTempViewCustomSize, !m_IsTempViewCustomSize.hasMultipleDifferentValues && m_IsTempViewCustomSize.boolValue == true, instant);
            SetAnimBool(m_ShowTempViewPosition, !m_IsTempViewPosition.hasMultipleDifferentValues && m_IsTempViewPosition.boolValue == true, instant);
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

            IMCProgressBar progreassBar = (IMCProgressBar)target;
            progreassBar.customID = EditorGUILayout.TextField("CustomID", progreassBar.customID);
            progreassBar.animationStyle = (IMCProgressBar.AnimationStyle)EditorGUILayout.EnumPopup("Animation Style", progreassBar.animationStyle);
            EditorGUILayout.PropertyField(m_Sprite, m_SpriteContent);
            EditorGUILayout.PropertyField(m_Background, m_BackgroundContent);
            progreassBar.rotateSpeed = EditorGUILayout.FloatField("Rotate Speed", progreassBar.rotateSpeed);
            progreassBar.value = EditorGUILayout.Slider("value", progreassBar.value, 0, 1);

            EditorGUILayout.PropertyField(m_IsTempViewCustomSize);
            if (EditorGUILayout.BeginFadeGroup(m_ShowTempViewCustomSize.faded))
            {
                EditorGUI.indentLevel++;
                progreassBar.tempViewSize = EditorGUILayout.Vector2Field("Temp View Size", progreassBar.tempViewSize);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_IsTempViewPosition);
            if (EditorGUILayout.BeginFadeGroup(m_ShowTempViewPosition.faded))
            {
                EditorGUI.indentLevel++;
                progreassBar.tempViewPosition = EditorGUILayout.Vector3Field("Temp View Position", progreassBar.tempViewPosition);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            serializedObject.ApplyModifiedProperties();
        }

    }
}
