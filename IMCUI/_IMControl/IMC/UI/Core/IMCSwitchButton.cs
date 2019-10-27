using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace IMCUI.UI
{
    public class IMCSwitchButton : IMCSlider
    {
        private Coroutine moveAnimationCoroutine;
        private IMCImage i_backGround;
        private float f_deltaX = 0;
        private float f_dragPositionX = 0;
        //private bool b_isOnToggle = false;
        private IMCText m_content;
        public IMCText content
        {
            get
            {
                if (m_content == null && handleRect != null && handleRect.GetChild(0) != null)
                    m_content = handleRect.GetChild(0).GetComponent<IMCText>();
                return m_content;
            }
            set { m_content = value; }
        }

        [Serializable]
        public class switchButtonEvent : UnityEvent<bool>
        { }
        public switchButtonEvent onClick = new switchButtonEvent();

        [SerializeField, Range(0, 1)]
        private float m_moveSpeed = 0.1f;
        public float moveSpeed
        {
            get { return m_moveSpeed; }
            set { m_moveSpeed = value; }
        }
        [SerializeField]
        private bool m_isOn;
        public bool isOn
        {
            get { return m_isOn; }
            set
            {
                m_isOn = value;
                //IsOnToggle(value);
                ShowBackGroundColor();
                ShowContentText();
            }
        }

        [SerializeField]
        private string m_onContent;
        public string onContent
        {
            get
            {
                if (isContent)
                    return m_onContent;
                return "";
            }
            set
            {
                m_onContent = value;
                ShowContentText();
            }
        }
        [SerializeField]
        private string m_offContent;
        public string offContent
        {
            get
            {
                if (isContent)
                    return m_offContent;
                return "";
            }
            set
            {
                m_offContent = value;
                ShowContentText();
            }
        }
        [SerializeField]
        private bool m_isContent;
        public bool isContent
        {
            get { return m_isContent; }
            set
            {
                m_isContent = value;
                ShowContentText();
            }
        }

        [SerializeField]
        private bool m_isColor = false;
        public bool isColor
        {
            get { return m_isColor; }
            set
            {
                m_isColor = value;
                ShowBackGroundColor();
            }
        }
        [SerializeField]
        private Color m_onColor = Color.white;
        public Color onColor
        {
            get
            {
                if (isColor)
                    return m_onColor;
                return Color.white;
            }
            set
            {
                m_onColor = value;
                ShowBackGroundColor();
            }
        }
        [SerializeField]
        private Color m_offColor = Color.gray;
        public Color offColor
        {
            get
            {
                if (isColor)
                    return m_offColor;
                return Color.gray;
            }
            set
            {
                m_offColor = value;
                ShowBackGroundColor();
            }
        }
        private CanvasGroup m_canvasGroup;
        public CanvasGroup canvasGroup //2018年1月5日11:43:03
        {
            get
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = this.GetComponent<CanvasGroup>();
                    if (m_canvasGroup == null)
                        m_canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
                }
                return m_canvasGroup;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCSwitchButton;
            m_containerType = ContainerType.Control;

            i_backGround = this.transform.GetChild(0).GetComponent<IMCImage>();
            if (m_isOn)
                value = 1;
            else
                value = 0;
            Refresh();
        }
        public override float alpha
        {
            get
            {
                return canvasGroup.alpha;
            }

            set
            {
                canvasGroup.alpha = value;
            }
        }
        public override bool interact
        {
            get
            {
                return canvasGroup.interactable;
            }

            set
            {
                canvasGroup.interactable = value;
            }
        }
        public override bool raycast
        {
            get
            {
                return canvasGroup.blocksRaycasts;
            }

            set
            {
                canvasGroup.blocksRaycasts = value;
            }
        }
        private bool recordIsOn = false;
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (moveAnimationCoroutine != null)
            {
                StopCoroutine(moveAnimationCoroutine);
            }
            recordIsOn = m_isOn;
            if (isMoveAnimation == false)
            {
                isMoveAnimation = null;
                Refresh();
                //if (!b_isOnToggle)
                //    b_isOnToggle = !b_isOnToggle;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(handleRect.parent.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out loacalPointerPosition);
                originalLocalPointerPosition = loacalPointerPosition;

                poniterDownPosition = Input.mousePosition;
            }
        }
        public void IsOnToggle(bool ison)
        {
            recordIsOn = m_isOn;
            m_isOn = ison;
            if (moveAnimationCoroutine == null)
                moveAnimationCoroutine = StartCoroutine(MoveAnimation());
            else
                StartCoroutine(MoveAnimation());
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isMoveAnimation == null)
            {
                isMoveAnimation = true;

                poniterUpPosition = Input.mousePosition;

                //Debug.Log("poniterUpPosition   " + poniterUpPosition);
                //Debug.Log("poniterDownPosition " + poniterDownPosition);
                if (poniterUpPosition.x == poniterDownPosition.x)
                {

                    m_isOn = !m_isOn;
                    if (moveAnimationCoroutine == null)
                        moveAnimationCoroutine = StartCoroutine(MoveAnimation());
                    else
                        StartCoroutine(MoveAnimation());
                }
                else
                {
                    if (m_isOn)
                    {
                        if (value <= 0.5f)
                            m_isOn = !m_isOn;
                    }
                    else
                    {
                        if (value >= 0.5f)
                            m_isOn = !m_isOn;
                    }
                    //Debug.Log("m_isOn" + m_isOn);
                    //Debug.Log("value" + value);
                    //Debug.Log(value);
                    if ((m_isOn && value <= 1) || (!m_isOn && value >= 0))
                    {
                        //Debug.Log("true");
                        if (moveAnimationCoroutine == null)
                            moveAnimationCoroutine = StartCoroutine(MoveAnimation());
                        else
                            StartCoroutine(MoveAnimation());
                    }
                    else
                    {
                        //Debug.Log("false");
                        //if (onClick != null)
                        //    onClick.Invoke(m_isOn);
                        Invoke("InvokeStartToggle", 0.1f);
                    }
                }
            }
        }
        Vector2 poniterDownPosition;
        Vector2 poniterUpPosition;

        Vector2 originalLocalPointerPosition;
        Vector2 loacalPointerPosition;
        public override void OnDrag(PointerEventData eventData)
        {
            if (isMoveAnimation == null)
            {
                RectTransform click = handleRect.parent.GetComponent<RectTransform>();
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(handleRect.parent.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out loacalPointerPosition))
                {
                    Vector3 offsetToOriginal = loacalPointerPosition - originalLocalPointerPosition;
                    float tempFloat = offsetToOriginal.x / click.rect.width;
                    value += tempFloat;
                }
                RectTransformUtility.ScreenPointToLocalPointInRectangle(handleRect.parent.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            }
        }

        public void Refresh()
        {
            //b_isOnToggle = false;

            if (moveAnimationCoroutine != null)
                StopCoroutine(moveAnimationCoroutine);

            ShowBackGroundColor();
            ShowContentText();
        }
        bool? isMoveAnimation = false;
        IEnumerator MoveAnimation()
        {
            //isMoveAnimation = true;

            if (onClick != null && recordIsOn != m_isOn)
                onClick.Invoke(m_isOn);
            if (m_isOn)
            {
                while (value < 1)
                {
                    if (value < 1)
                        value += m_moveSpeed;
                    i_backGround.color = Color.Lerp(i_backGround.color, m_onColor, 0.1f);
                    yield return null;
                }
                ShowContentText();
                Invoke("InvokeStartToggle", 0.1f);
            }
            else
            {
                while (value > 0)
                {
                    if (value > 0)
                        value -= m_moveSpeed;
                    i_backGround.color = Color.Lerp(i_backGround.color, m_offColor, 0.1f);
                    yield return null;
                }
                ShowContentText();
                Invoke("InvokeStartToggle", 0.1f);
            }
        }
        void InvokeStartToggle()
        {
            isMoveAnimation = false;
        }
        void ShowContentText()
        {
            if (m_isContent)
            {
                if (content)
                {
                    if (m_isOn)
                        content.text = m_onContent;
                    else
                        content.text = m_offContent;
                }
            }
            //else if (content != null && content.text != "")
            //{
            //    content.text = "";
            //}
        }

        void ShowBackGroundColor()
        {
            if (i_backGround == null)
                i_backGround = this.transform.GetChild(0).GetComponent<IMCImage>();

            if (m_isOn)
                i_backGround.color = m_onColor;
            else
                i_backGround.color = m_offColor;

        }
    }
}