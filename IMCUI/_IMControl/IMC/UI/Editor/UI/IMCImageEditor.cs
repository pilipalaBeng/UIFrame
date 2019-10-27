using System.Linq;
using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;

namespace IMCUIEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Sprites.
    /// </summary>

    [CustomEditor(typeof(IMCImage), true)]
    [CanEditMultipleObjects]
    public class IMCImageEditor : GraphicEditor
    {
        SerializedProperty m_FillMethod;
        SerializedProperty m_FillOrigin;
        SerializedProperty m_FillAmount;
        SerializedProperty m_FillClockwise;
        SerializedProperty m_Type;
        SerializedProperty m_FillCenter;
        SerializedProperty m_Sprite;
        SerializedProperty m_PreserveAspect;
        GUIContent m_SpriteContent;
        GUIContent m_SpriteTypeContent;
        GUIContent m_ClockwiseContent;
        AnimBool m_ShowSlicedOrTiled;
        AnimBool m_ShowSliced;
        AnimBool m_ShowFilled;
        AnimBool m_ShowType;

        SerializedProperty m_isNeedShowAnimationPlan;
        AnimBool m_ShowAnimationPlan;

        SerializedProperty m_animationSpriteList;
        SerializedProperty m_animationIntervalTime;
        SerializedProperty m_playOnAwakeFrameAnimation;
        SerializedProperty m_isRepeatPlayFrameAnimation;
        SerializedProperty m_repeatPlayIntervalTime;
        protected override void OnEnable()
        {
            base.OnEnable();

            m_SpriteContent = new GUIContent("Source Image");
            m_SpriteTypeContent = new GUIContent("Image Type");
            m_ClockwiseContent = new GUIContent("Clockwise");

            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_Type = serializedObject.FindProperty("m_Type");
            m_FillCenter = serializedObject.FindProperty("m_FillCenter");
            m_FillMethod = serializedObject.FindProperty("m_FillMethod");
            m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
            m_FillClockwise = serializedObject.FindProperty("m_FillClockwise");
            m_FillAmount = serializedObject.FindProperty("m_FillAmount");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");

            m_ShowType = new AnimBool(m_Sprite.objectReferenceValue != null);
            m_ShowType.valueChanged.AddListener(Repaint);

            m_animationSpriteList = serializedObject.FindProperty("m_frameAnimationSpriteList");
            m_animationIntervalTime = serializedObject.FindProperty("m_frameAnimationIntervalTime");
            m_playOnAwakeFrameAnimation = serializedObject.FindProperty("m_isPlayOnAwakeToFrameAnimation");
            m_isRepeatPlayFrameAnimation = serializedObject.FindProperty("m_isRepeatPlayFrameAnimation");
            m_repeatPlayIntervalTime = serializedObject.FindProperty("m_repeatPlayIntervalTime");

            m_isNeedShowAnimationPlan = serializedObject.FindProperty("m_isNeedShowFrameAnimationPlan");
            m_ShowAnimationPlan = new AnimBool(Repaint);

            var typeEnum = (IMCImage.Type)m_Type.enumValueIndex;

            m_ShowSlicedOrTiled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == IMCImage.Type.Sliced);
            m_ShowSliced = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == IMCImage.Type.Sliced);
            m_ShowFilled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == IMCImage.Type.Filled);
            m_ShowSlicedOrTiled.valueChanged.AddListener(Repaint);
            m_ShowSliced.valueChanged.AddListener(Repaint);
            m_ShowFilled.valueChanged.AddListener(Repaint);

            SetShowNativeSize(true);
            SetFrameAnimationPlanAnimation(true);
        }

        protected override void OnDisable()
        {
            m_ShowType.valueChanged.RemoveListener(Repaint);
            m_ShowSlicedOrTiled.valueChanged.RemoveListener(Repaint);
            m_ShowSliced.valueChanged.RemoveListener(Repaint);
            m_ShowFilled.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            SetFrameAnimationPlanAnimation(false);
            IMCImage image = (IMCImage)target;
            //EditorGUI.BeginDisabledGroup(true);
            //image.ControlType = (ControlType)EditorGUILayout.EnumPopup("ControlType", image.ControlType);
            //image.ContainerType = (ContainerType)EditorGUILayout.EnumPopup("ContainerType", image.ContainerType);
            //EditorGUI.EndDisabledGroup();
            image.customID = EditorGUILayout.TextField("CustomID", image.customID);
            serializedObject.Update();

            SpriteGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();

            m_ShowType.target = m_Sprite.objectReferenceValue != null;
            if (EditorGUILayout.BeginFadeGroup(m_ShowType.faded))
                TypeGUI();
            EditorGUILayout.EndFadeGroup();

            SetShowNativeSize(false);
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

            EditorGUILayout.PropertyField(m_isNeedShowAnimationPlan);
            if (EditorGUILayout.BeginFadeGroup(m_ShowAnimationPlan.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_playOnAwakeFrameAnimation);
                EditorGUILayout.PropertyField(m_animationSpriteList, true);
                EditorGUILayout.PropertyField(m_animationIntervalTime);
                EditorGUILayout.PropertyField(m_isRepeatPlayFrameAnimation);
                if (m_isRepeatPlayFrameAnimation.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_repeatPlayIntervalTime);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            serializedObject.ApplyModifiedProperties();
        }
        void SetFrameAnimationPlanAnimation(bool instant)
        {
            SetFrameAnimationPlanAnimation(m_ShowAnimationPlan, !m_isNeedShowAnimationPlan.hasMultipleDifferentValues && m_isNeedShowAnimationPlan.boolValue == true, instant);
        }
        void SetFrameAnimationPlanAnimation(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }
        void SetShowNativeSize(bool instant)
        {
            IMCImage.Type type = (IMCImage.Type)m_Type.enumValueIndex;
            bool showNativeSize = (type == IMCImage.Type.Simple || type == IMCImage.Type.Filled);
            base.SetShowNativeSize(showNativeSize, instant);
        }

        /// <summary>
        /// Draw the atlas and Image selection fields.
        /// </summary>

        protected void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Sprite, m_SpriteContent);
            if (EditorGUI.EndChangeCheck())
            {
                var newSprite = m_Sprite.objectReferenceValue as Sprite;
                if (newSprite)
                {
                    IMCImage.Type oldType = (IMCImage.Type)m_Type.enumValueIndex;
                    if (newSprite.border.SqrMagnitude() > 0)
                    {
                        m_Type.enumValueIndex = (int)IMCImage.Type.Sliced;
                    }
                    else if (oldType == IMCImage.Type.Sliced)
                    {
                        m_Type.enumValueIndex = (int)IMCImage.Type.Simple;
                    }
                }
            }
        }

        /// <summary>
        /// Sprites's custom properties based on the type.
        /// </summary>

        protected void TypeGUI()
        {
            EditorGUILayout.PropertyField(m_Type, m_SpriteTypeContent);

            ++EditorGUI.indentLevel;
            {
                IMCImage.Type typeEnum = (IMCImage.Type)m_Type.enumValueIndex;

                bool showSlicedOrTiled = (!m_Type.hasMultipleDifferentValues && (typeEnum == IMCImage.Type.Sliced || typeEnum == IMCImage.Type.Tiled));
                if (showSlicedOrTiled && targets.Length > 1)
                    showSlicedOrTiled = targets.Select(obj => obj as IMCImage).All(img => img.hasBorder);

                m_ShowSlicedOrTiled.target = showSlicedOrTiled;
                m_ShowSliced.target = (showSlicedOrTiled && !m_Type.hasMultipleDifferentValues && typeEnum == IMCImage.Type.Sliced);
                m_ShowFilled.target = (!m_Type.hasMultipleDifferentValues && typeEnum == IMCImage.Type.Filled);

                IMCImage image = target as IMCImage;
                if (EditorGUILayout.BeginFadeGroup(m_ShowSlicedOrTiled.faded))
                {
                    if (image.hasBorder)
                        EditorGUILayout.PropertyField(m_FillCenter);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSliced.faded))
                {
                    if (image.sprite != null && !image.hasBorder)
                        EditorGUILayout.HelpBox("This Image doesn't have a border.", MessageType.Warning);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowFilled.faded))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_FillMethod);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_FillOrigin.intValue = 0;
                    }
                    switch ((IMCImage.FillMethod)m_FillMethod.enumValueIndex)
                    {
                        case IMCImage.FillMethod.Horizontal:
                            m_FillOrigin.intValue = (int)(IMCImage.OriginHorizontal)EditorGUILayout.EnumPopup("Fill Origin", (IMCImage.OriginHorizontal)m_FillOrigin.intValue);
                            break;
                        case IMCImage.FillMethod.Vertical:
                            m_FillOrigin.intValue = (int)(IMCImage.OriginVertical)EditorGUILayout.EnumPopup("Fill Origin", (IMCImage.OriginVertical)m_FillOrigin.intValue);
                            break;
                        case IMCImage.FillMethod.Radial90:
                            m_FillOrigin.intValue = (int)(IMCImage.Origin90)EditorGUILayout.EnumPopup("Fill Origin", (IMCImage.Origin90)m_FillOrigin.intValue);
                            break;
                        case IMCImage.FillMethod.Radial180:
                            m_FillOrigin.intValue = (int)(IMCImage.Origin180)EditorGUILayout.EnumPopup("Fill Origin", (IMCImage.Origin180)m_FillOrigin.intValue);
                            break;
                        case IMCImage.FillMethod.Radial360:
                            m_FillOrigin.intValue = (int)(IMCImage.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (IMCImage.Origin360)m_FillOrigin.intValue);
                            break;
                    }
                    EditorGUILayout.PropertyField(m_FillAmount);
                    if ((IMCImage.FillMethod)m_FillMethod.enumValueIndex > IMCImage.FillMethod.Vertical)
                    {
                        EditorGUILayout.PropertyField(m_FillClockwise, m_ClockwiseContent);
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;
        }

        /// <summary>
        /// All graphics have a preview.
        /// </summary>

        public override bool HasPreviewGUI() { return true; }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            IMCImage image = target as IMCImage;
            if (image == null) return;

            Sprite sf = image.sprite;
            if (sf == null) return;

            SpriteDrawUtility.DrawSprite(sf, rect, image.canvasRenderer.GetColor());
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>

        public override string GetInfoString()
        {
            IMCImage image = target as IMCImage;
            Sprite sprite = image.sprite;

            int x = (sprite != null) ? Mathf.RoundToInt(sprite.rect.width) : 0;
            int y = (sprite != null) ? Mathf.RoundToInt(sprite.rect.height) : 0;

            return string.Format("Image Size: {0}x{1}", x, y);
        }
    }
}
