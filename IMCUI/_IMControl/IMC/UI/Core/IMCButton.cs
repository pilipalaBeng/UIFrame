using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IMCUI.UI
{
    // Button that's meant to work with mouse or touch-based devices.
    [AddComponentMenu("IMCUI/IMCButton", 30)]
    public class IMCButton : IMCSelectable, IPointerClickHandler, ISubmitHandler
    {

        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected IMCButton()
        { }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private IMCCanvas canvas;
        private IMCUIBehaviour parentUi;
        private IMCCanvas FindCanvas()
        {
            if (parentUi == null)
                parentUi = this;
            if (parentUi.GetComponent<IMCUIBehaviour>())
            {
                if (parentUi.GetComponent<IMCForm>() == null)
                {
                    if (parentUi.parent == null)
                        return null;
                    parentUi = parentUi.parent;
                    IMCCanvas canvas;
                    if ((canvas = FindCanvas()) != null)
                    {
                        return canvas;
                    }
                }
                else
                {
                    return parentUi.GetComponent<IMCForm>().canvas;
                }
            }
            return null;
        }
        private void Press()
        {
           
            if (!IsActive() || !IsInteractable())
                return;
            m_OnClick.Invoke();//m_OnClick 和  onPointerClick 执行顺序需要注意~
            if (onPointerClick != null)
            {
                onPointerClick(gameObject);
            }
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (downTime > longpressTime)
            {
                if (LongPressEnable)
                    return;
            }

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
            Debug.Log("OnSubmit");
            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        #region ExpandBtn
        private IMCText text;

        public IMCText Text//2017年7月5日09:28:49 添加对button子物体text索引
        {
            get
            {
                if (!text)
                    text = this.transform.GetComponentInChildren<IMCText>();
                return text;
            }
        }
        public override float alpha
        {
            get
            {
                if (graphics.Length > 0)
                    return graphics[0].color.a;
                return 0;
            }
            set
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = new Color(graphics[i].color.r, graphics[i].color.g, graphics[i].color.b, value);
                }
            }
        }
        public override bool interact
        {
            get
            {
                return interactable;
            }
            set
            {
                interactable = value;
            }
        }
        public override bool raycast
        {
            get
            {
                return base.raycast;
            }
            set
            {
                base.raycast = value;
            }
        }
        protected override void Awake()
        {
            base.Awake();

            m_controlType = ControlType.IMCButton;
            m_containerType = ContainerType.Control;
        }
        public override void Initialize()
        {
            if (initialize)
                return;
            initialize = true;
        }

        public void AddListener(UnityAction action)
        {
            onClick.AddListener(action);
        }
        public void RemoveListener(UnityAction action)
        {
            onClick.RemoveListener(action);
        }
        public void RemoveAllListener()
        {
            onClick.RemoveAllListeners();
        }

        private bool stipulatedTimeFunctionStartToggle = false;
        private float stipulateTime = 0;
        private UnityAction withinStipilateAction = null;
        private float startTime = 0;
        private float stopTime = 0;
        /// <summary>
        /// 按住抬起在规定时间内执行委托函数,函数参数中的UnityAction 与 onClick 委托 没有任何关系
        /// </summary>
        public void StartWithinStipulatedTimeCall(float time, UnityAction action)
        {
            stipulatedTimeFunctionStartToggle = true;
            if (time < 0)
                time = 0;
            stipulateTime = time;
            withinStipilateAction = action;
        }

        public void StopWithinStipulatedTimeCall()
        {
            stipulatedTimeFunctionStartToggle = false;
            stipulateTime = 0;
            withinStipilateAction = null;
        }

        /// <summary>
        ///  按住抬起在规定时间后执行委托函数,函数参数中的UnityAction 与 onClick 委托 没有任何关系
        /// </summary>
        public void StartOutsideStipulatedTimeCall(float time, UnityAction<GameObject> action)
        {
            longpressTime = time;
            LongPressEnable = true;
            onPointerLongPress = action;
        }
        public void StopOutsideStipulatedTimeCall()
        {
            LongPressEnable = false;
            onPointerLongPress = null;
        }
        private UnityAction delayAction = null;
        private bool delayCallToggle = false;
        private float delayTime = 0;
        private IEnumerator delayCor = null;
        /// <summary>
        /// 延迟执行,按下后延迟多少秒后执行,函数参数中的UnityAction 与 onClick 委托 没有任何关系
        /// </summary>
        public void StartDelayCall(float time, UnityAction action)
        {
            delayCallToggle = true;
            delayTime = time;
            delayAction = action;
        }

        public void StopDelayCall()
        {
            if (delayCor != null)
            {
                if (delayCor != null) StopCoroutine(delayCor);
                delayCallToggle = false;
                delayAction = null;
                delayTime = 0;
                delayCor = null;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (m_isPlayAudio)//2018年1月3日21:00:21
            {
                if (!canvas)
                    canvas = FindCanvas();
                if (canvas)
                {
                    if (m_clip)
                        canvas.PlayAudio(m_clip);
                    else
                        canvas.PlayAudio(null);
                }
                else if (canvas == null && m_clip)
                {
                    AudioSource.PlayClipAtPoint(m_clip, Vector3.zero);
                }
            }

            downTime = 0;
            pointerDown = true;

            if (stipulatedTimeFunctionStartToggle)
            {
                startTime = Time.time;
            }
            if (delayCallToggle)
            {
                delayCallToggle = !delayCallToggle;
                delayCor = DelayCoroutine(delayTime, delayAction);
                StartCoroutine(delayCor);
            }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            pointerDown = false;

            if (downTime > longpressTime)
            {
                if (LongPressEnable)
                    return;
            }

            if (stipulatedTimeFunctionStartToggle)
            {
                stopTime = Time.time - startTime;

                if (stopTime <= stipulateTime && withinStipilateAction != null)
                {
                    withinStipilateAction();
                    StopWithinStipulatedTimeCall();
                }
            }

        }

        private IEnumerator DelayCoroutine(float time, UnityAction action)
        {
            yield return new WaitForSeconds(time);
            if (action != null)
            {
                action();
            }
            StopCoroutine(delayCor);
        }
        #endregion
        public UnityAction<GameObject> onPointerClick;

        public UnityAction<GameObject> onPointerLongPress;
        private bool pointerDown = false;
        private float downTime;
        public float longpressTime;
        public bool LongPressEnable = false;
        [SerializeField]
        private bool m_isZoomAnimation = false;

        public bool isZoomAnimation
        {
            get { return m_isZoomAnimation; }
            set { m_isZoomAnimation = value; }
        }

        [SerializeField]
        private bool m_isPlayAudio = false;

        public bool isPlayAudio
        {
            get { return m_isPlayAudio; }
            set { m_isPlayAudio = value; }
        }

        [SerializeField]
        private AudioClip m_clip;
        public AudioClip clip
        {
            get { return m_clip; }
            set { m_clip = value; }
        }
        private void Update()
        {
            if (pointerDown)
            {
                downTime += Time.deltaTime;
                if (downTime >= longpressTime)
                {
                    pointerDown = false;
                    if (LongPressEnable)
                    {
                        if (onPointerLongPress != null)
                            onPointerLongPress(gameObject);
                    }
                }
            }
        }
        public void AddOnPointerLongListener(UnityAction<GameObject> action)
        {
            onPointerLongPress += action;
        }
        public void RemoveOnPointerLongListener(UnityAction<GameObject> action)
        {
            onPointerLongPress -= action;
        }


        public void Create(string buttonTitle = "Button", Sprite backgroundPicture = null)
        {
            Text.text = buttonTitle;
            if (backgroundPicture)
                image.sprite = backgroundPicture;
        }


        public void Create(UnityAction call, string buttonTitle = "Button")
        {
            AddListener(call);
            Text.text = buttonTitle;
        }


        public void Create(UnityAction call, string buttonTitle = "Button", Sprite backgroundPicture = null)
        {
            AddListener(call);
            Text.text = buttonTitle;
            if (backgroundPicture)
                image.sprite = backgroundPicture;
        }


        //internal void SetCanvasGroup(bool p)
        //{
        //    enable = p;
        //}
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (isZoomAnimation)
                transform.localScale = new Vector2(transform.localScale.x + 0.2f, transform.localScale.x + 0.2f);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (isZoomAnimation)
                transform.localScale = new Vector2(transform.localScale.x - 0.2f, transform.localScale.x - 0.2f);
        }

    }

}

