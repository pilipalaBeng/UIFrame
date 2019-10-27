using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using IMCHyperlinksLabel = IMCUI.UI.IMCHyperlinksLabel;
namespace IMCUIEditor.UI
{
    [CustomEditor(typeof(IMCHyperlinksLabel), true)]
    public class IMCHyperlinksLabelEditor : IMCTextEditor
    {
        SerializedProperty m_OnHrefClick;
        SerializedProperty m_hyperlinksLaberColor;
        SerializedProperty m_hyperlinksLaberFontSize;

        private void OnEnable()
        {
            base.OnEnable();
            m_OnHrefClick = serializedObject.FindProperty("m_OnHrefClick");
            m_hyperlinksLaberColor = serializedObject.FindProperty("m_hyperlinksLaberColor");
            m_hyperlinksLaberFontSize = serializedObject.FindProperty("m_hyperlinksLaberFontSize");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_hyperlinksLaberFontSize);
            EditorGUILayout.PropertyField(m_hyperlinksLaberColor);
            EditorGUILayout.PropertyField(m_OnHrefClick);
            serializedObject.ApplyModifiedProperties();
        }
        //[Test]
        //public void EditorTest()
        //{
        //    //Arrange
        //    var gameObject = new GameObject();

        //    //Act
        //    //Try to rename the GameObject
        //    var newGameObjectName = "My game object";
        //    gameObject.name = newGameObjectName;

        //    //Assert
        //    //The object has a new name
        //    Assert.AreEqual(newGameObjectName, gameObject.name);
        //}
    }
}