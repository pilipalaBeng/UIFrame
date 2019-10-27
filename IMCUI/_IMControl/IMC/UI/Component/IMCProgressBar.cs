using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace IMCUI.UI
{
    [DisallowMultipleComponent]
    public class IMCProgressBar : IMCUIBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCProgressBar;
            m_containerType = ContainerType.Control;
        }
        public enum AnimationStyle
        {
            None,
            Rotation,
            NumbCircularProgressBar,
            NumbSquareProgressBar,
            CircularProgressBar,
            SquareProgressBar,
            //Jump,
        }
        public AnimationStyle animationStyle = AnimationStyle.None;

        private const string spritePathOrName = "";
        private const string backgroundPathOrName = "";
        [SerializeField]
        private float m_value;

        public float value
        {
            get { return m_value; }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;

                m_value = value;
                if (m_tempView)
                    m_tempView.fillAmount = m_value;
                if (m_tempNumbText)
                    m_tempNumbText.text = ((int)(m_value * 100)).ToString() + "%";
            }
        }
        [SerializeField]
        private Sprite m_sprite;
        public Sprite sprite
        {
            get
            {
                if (!m_sprite)
                    m_sprite = ResourcesImagePicture(spritePathOrName);
                return m_sprite;
            }
            set { m_sprite = value; }
        }
        [SerializeField]
        private Sprite m_background;
        public Sprite background
        {
            get
            {
                if (!m_background)
                    m_background = ResourcesImagePicture(backgroundPathOrName);
                return m_background;
            }
            set { m_background = value; }
        }
        [SerializeField]
        private IMCImage m_tempView;
        public IMCImage tempView
        {
            get
            {
                if (!m_tempView)
                    m_tempView = CreateImage("tempView");
                return m_tempView;
            }
        }
        [SerializeField]
        private IMCImage m_backgroundImage;
        public IMCImage backgroundImage
        {
            get
            {
                if (!m_backgroundImage)
                    m_backgroundImage = CreateImage("background");
                return m_backgroundImage;
            }
        }
        private IMCText m_tempNumbText;
        private IMCText tempNumbText
        {
            get
            {
                if (!m_tempNumbText)
                    if (m_tempView)
                        m_tempNumbText = CreateText("percentage");
                return m_tempNumbText;
            }
        }
        private RectTransform m_targetRT;
        private RectTransform targetRT
        {
            get
            {
                if (!m_targetRT)
                    m_targetRT = this.GetComponent<RectTransform>();
                return m_targetRT;
            }
        }
        public float rotateSpeed = -100f;
        [SerializeField]
        public bool isTempViewCustomSize = false;
        [SerializeField]
        private Vector2 m_TempViewSize;

        public Vector2 tempViewSize
        {
            get { return m_TempViewSize; }
            set
            {
                m_TempViewSize = value;
                tempView.sizeDelta = m_TempViewSize;
            }
        }
        public bool isTempViewPosition = false;
        [SerializeField]
        private Vector3 m_TempViewPosition;
        public Vector3 tempViewPosition
        {
            get { return m_TempViewPosition; }
            set
            {
                m_TempViewPosition = value;
                tempView.anchoredPosition3D = m_TempViewPosition;
            }
        }
        private IMCImage CreateImage(string name)
        {
            GameObject tempGo = new GameObject(name);
            tempGo.gameObject.AddComponent<RectTransform>();
            return tempGo.AddComponent<IMCImage>();
        }
        private IMCText CreateText(string name)
        {
            GameObject tempGo = new GameObject(name);
            tempGo.gameObject.AddComponent<RectTransform>();
            IMCText text = tempGo.AddComponent<IMCText>();
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.black;
            text.text = "0%";
            text.resizeTextForBestFit = true;
            return text;
        }
        private Vector2 targetRtAnchorMax;
        private Vector2 targetRtAnchorMin;
        private void BasicSetup()
        {
            backgroundImage.transform.SetParent(targetRT);

            targetRtAnchorMax = targetRT.anchorMax;
            targetRtAnchorMin = targetRT.anchorMin;

            targetRT.anchorMax = new Vector2(0.5f, 0.5f);
            targetRT.anchorMin = new Vector2(0.5f, 0.5f);

            backgroundImage.SetAsLastSibling();
            backgroundImage.transform.localRotation = Quaternion.identity;
            backgroundImage.transform.localScale = Vector3.one;
            backgroundImage.anchoredPosition3D = Vector3.zero;
            backgroundImage.anchorMax = targetRT.anchorMax;
            backgroundImage.anchorMin = targetRT.anchorMin;
            backgroundImage.sizeDelta = targetRT.sizeDelta;
            if (m_background != null)
                backgroundImage.sprite = m_background;
            else
                backgroundImage.color = new Color(0, 0, 0, 0);

            tempView.transform.SetParent(backgroundImage.transform);
            tempView.transform.localScale = Vector3.one;
            tempView.transform.localRotation = Quaternion.identity;
            tempView.SetAsLastSibling();

            tempView.anchoredPosition3D = isTempViewPosition ? tempViewPosition : Vector3.zero;

            if (isTempViewCustomSize)
                tempView.sizeDelta = m_TempViewSize;
            else
                if (targetRT.sizeDelta.x == targetRT.sizeDelta.y)
                tempView.sizeDelta = targetRT.sizeDelta;
            else if (targetRT.sizeDelta.x > targetRT.sizeDelta.y)
                tempView.sizeDelta = new Vector2(targetRT.sizeDelta.y, targetRT.sizeDelta.y);
            else
                tempView.sizeDelta = new Vector2(targetRT.sizeDelta.x, targetRT.sizeDelta.x);
        }
        public void Begin(Sprite tempViewSprite = null, Sprite backgroundSprite = null)
        {
            MBeign(animationStyle, tempViewSprite, background);
        }
        public void Begin(AnimationStyle style, Sprite tempViewSprite = null, Sprite backgroundSprite = null)
        {
            animationStyle = style;
            MBeign(animationStyle, tempViewSprite, background);
        }
        private void MBeign(AnimationStyle style, Sprite tempViewSprite = null, Sprite backgroundSprite = null)
        {
            if (repeatedCalls == true)
            {
                repeatedCalls = null;
                BasicSetup();
                if (tempViewSprite != null)
                    tempView.sprite = tempViewSprite;
                else
                    tempView.sprite = sprite;

                if (backgroundSprite != null)
                    backgroundImage.sprite = backgroundSprite;
                else
                    backgroundImage.sprite = background;

                StopAllCoroutines();
                switch (style)
                {
                    case AnimationStyle.Rotation:
                        if (m_tempNumbText)
                            Destroy(m_tempNumbText.gameObject);
                        m_tempView.type = IMCImage.Type.Simple;
                        StartCoroutine(RotationCoroutine());
                        break;
                    case AnimationStyle.NumbCircularProgressBar:
                        ProgressBar(IMCUI.UI.IMCImage.FillMethod.Radial360, true);
                        break;
                    case AnimationStyle.NumbSquareProgressBar:
                        if (!isTempViewCustomSize) tempView.sizeDelta = targetRT.sizeDelta;
                        ProgressBar(IMCUI.UI.IMCImage.FillMethod.Horizontal, true);
                        break;
                    case AnimationStyle.CircularProgressBar:
                        if (m_tempNumbText)
                            Destroy(m_tempNumbText.gameObject);
                        ProgressBar(IMCUI.UI.IMCImage.FillMethod.Radial360, false);
                        break;
                    case AnimationStyle.SquareProgressBar:
                        if (m_tempNumbText)
                            Destroy(m_tempNumbText.gameObject);
                        if (!isTempViewCustomSize) tempView.sizeDelta = targetRT.sizeDelta;
                        ProgressBar(IMCUI.UI.IMCImage.FillMethod.Horizontal, false);
                        break;
                }
                tempViewSize = tempView.sizeDelta;
                tempViewPosition = tempView.anchoredPosition3D;

                targetRT.anchorMax = targetRtAnchorMax;
                targetRT.anchorMin = targetRtAnchorMin;
            }
            else if (repeatedCalls == null)
            {
                repeatedCalls = true;
                value = 0;
                MBeign(style);
            }
        }
        private bool? repeatedCalls = true;
        public void Stop(bool isDestroy = false)
        {
            repeatedCalls = true;
            StopAllCoroutines();
            if (isDestroy)
                Destroy(backgroundImage.gameObject);
        }
        private IEnumerator RotationCoroutine()
        {
            while (true)
            {
                tempView.transform.Rotate(new Vector3(0, 0, rotateSpeed), 1f);
                yield return null;
            }
        }

        private void ProgressBar(IMCUI.UI.IMCImage.FillMethod fillmethod, bool textSwitch)
        {
            tempView.type = IMCImage.Type.Filled;
            tempView.fillMethod = fillmethod;
            tempView.fillAmount = 0;
            if (textSwitch)
            {
                tempNumbText.transform.SetParent(tempView.transform);
                tempNumbText.sizeDelta = tempView.sizeDelta;
                tempNumbText.anchoredPosition3D = Vector3.zero;
            }
        }
        private Sprite ResourcesImagePicture(string pathOrName)
        {
            Texture2D tempTex = Resources.Load(pathOrName) as Texture2D;
            if (tempTex == null)
                return null;
            Sprite pic = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), new Vector2(0.5f, 0.5f));
            return pic;
        }

    }
}