using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    [CustomPropertyDrawer(typeof(ColorBlock), true)]
    public class ColorBlockDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect drawRect = rect;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty normalColor = prop.FindPropertyRelative("m_NormalColor");
            SerializedProperty highlighted = prop.FindPropertyRelative("m_HighlightedColor");
            SerializedProperty pressedColor = prop.FindPropertyRelative("m_PressedColor");
            SerializedProperty disabledColor = prop.FindPropertyRelative("m_DisabledColor");
            SerializedProperty colorMultiplier = prop.FindPropertyRelative("m_ColorMultiplier");
            SerializedProperty fadeDuration = prop.FindPropertyRelative("m_FadeDuration");

            EditorGUI.PropertyField(drawRect, normalColor);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, highlighted);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, pressedColor);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, disabledColor);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, colorMultiplier);
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(drawRect, fadeDuration);
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 6 * EditorGUIUtility.singleLineHeight + 5 * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
