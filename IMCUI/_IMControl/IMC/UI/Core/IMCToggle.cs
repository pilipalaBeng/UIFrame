using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace IMCUI.UI
{
    /// <summary>
    /// Simple toggle -- something that has an 'on' and 'off' states: checkbox, toggle button, radio button, etc.
    /// </summary>
    [AddComponentMenu("IMCUI/IMCToggle", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class IMCToggle : IMCSelectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        public enum ToggleTransition
        {
            None,
            Fade
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        { }

        /// <summary>
        /// Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        //public Graphic backGround;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        // group that this toggle can belong to
        [SerializeField]
        private ToggleGroup m_Group;

        public ToggleGroup group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(m_Group, true);
                    PlayEffect(true);
                }
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")]
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        protected IMCToggle()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Set(m_IsOn, false);
            PlayEffect(toggleTransition == ToggleTransition.None);

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(ToggleGroup newGroup, bool setMemberValue)
        {
            ToggleGroup oldGroup = m_Group;

            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (m_Group != null && IsActive())
                m_Group.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
                m_Group.NotifyToggleOn(this);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        public bool isOn
        {
            get { return m_IsOn; }
            set
            {
                Set(value);
            }
        }

        void Set(bool value)
        {
            Set(value, true);
            //if (backGround)
            //    backGround.alpha = m_IsOn ? 0 : 1;
        }

        void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);
            if (sendCallback)
                onValueChanged.Invoke(m_IsOn);
        }

        /// <summary>
        /// Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                graphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
            else
#endif
                graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            InternalToggle();
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
            if (m_isPlayAudio && m_clip != null)
                AudioSource.PlayClipAtPoint(m_clip, Vector3.zero);
        }
        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }
        #region ExpandToggle
        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCToggle;
            m_containerType = ContainerType.Control;
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
                if (label)
                    label.color = new Color(label.color.r, label.color.g, label.color.b, value);//2017年5月27日13:48:46  添加 toggle 更改 alpha颜色时  text组件的alpha也跟着变
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
        private IMCText m_Label;
        public IMCText label
        {
            get
            {
                if (!m_Label)
                {
                    m_Label = this.transform.GetComponentInChildren<IMCText>();

                }
                return m_Label;
            }
        }
        public void AddListener(UnityAction<bool> action)
        {
            onValueChanged.AddListener(action);
        }
        public void RemoveListener(UnityAction<bool> action)
        {
            onValueChanged.RemoveListener(action);
        }
        public void RemoveAllListener()
        {
            onValueChanged.RemoveAllListeners();
        }
        #endregion

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
    }
}
