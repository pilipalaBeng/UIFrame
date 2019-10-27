using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Images.
    /// </summary>
    [CustomEditor(typeof(IMCRawImage), true)]
    [CanEditMultipleObjects]
    public class IMCRawImageEditor : GraphicEditor
    {
        SerializedProperty m_Texture;
        SerializedProperty m_UVRect;
        GUIContent m_UVRectContent;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Note we have precedence for calling rectangle for just rect, even in the Inspector.
            // For example in the Camera component's Viewport Rect.
            // Hence sticking with Rect here to be consistent with corresponding property in the API.
            m_UVRectContent     = new GUIContent("UV Rect");

            m_Texture           = serializedObject.FindProperty("m_Texture");
            m_UVRect            = serializedObject.FindProperty("m_UVRect");

            SetShowNativeSize(true);
        }

        public override void OnInspectorGUI()
        {
            IMCRawImage rawImage = (IMCRawImage)target;
            //EditorGUI.BeginDisabledGroup(true);
            //rawImage.ControlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", rawImage.ControlType);
            //rawImage.ContainerType = (ContainerType)EditorGUILayout.EnumPopup("ContainerType", rawImage.ContainerType);
            //EditorGUI.EndDisabledGroup();
            rawImage.customID = EditorGUILayout.TextField("CustomID", rawImage.customID);
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Texture);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            EditorGUILayout.PropertyField(m_UVRect, m_UVRectContent);
            SetShowNativeSize(false);
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }

        void SetShowNativeSize(bool instant)
        {
            base.SetShowNativeSize(m_Texture.objectReferenceValue != null, instant);
        }

        /// <summary>
        /// Allow the texture to be previewed.
        /// </summary>

        public override bool HasPreviewGUI()
        {
            IMCRawImage rawImage = target as IMCRawImage;
            return rawImage != null;
        }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            IMCRawImage rawImage = target as IMCRawImage;
            Texture tex = rawImage.mainTexture;

            if (tex == null)
                return;

            Rect outer = rawImage.uvRect;
            outer.xMin *= rawImage.rectTransform.rect.width;
            outer.xMax *= rawImage.rectTransform.rect.width;
            outer.yMin *= rawImage.rectTransform.rect.height;
            outer.yMax *= rawImage.rectTransform.rect.height;

            SpriteDrawUtility.DrawSprite(tex, rect, outer, rawImage.uvRect, rawImage.canvasRenderer.GetColor());
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>

        public override string GetInfoString()
        {
            IMCRawImage rawImage = target as IMCRawImage;

            // Image size Text
            string text = string.Format("RawImage Size: {0}x{1}",
                    Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)),
                    Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));

            return text;
        }
    }
}
