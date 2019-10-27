using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace IMCUI.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IMCUI/IMCForm", 50)]
    public class IMCForm : IMCUIContainer
    {
        [HideInInspector]
        public IMCCanvas canvas;
        [SerializeField]
        private bool m_showOnAwake = false;
        public bool showOnAwake
        {
            get { return m_showOnAwake; }
            set { m_showOnAwake = value; }
        }
        [SerializeField]
        private Vector3 m_showTargetPosition = Vector3.zero;
        public Vector3 showTargetPosition
        {
            get { return m_showTargetPosition; }
            set { m_showTargetPosition = value; }
        }
        [SerializeField]
        private Vector3 m_closeTargetPosition = Vector3.zero;
        public Vector3 closeTargetPosition
        {
            get { return m_closeTargetPosition; }
            set { m_closeTargetPosition = value; }
        }

        [SerializeField]
        private bool m_isBlockerToggle = false;
        public bool isBlockerToggle
        {
            get { return m_isBlockerToggle; }
            set { m_isBlockerToggle = value; }
        }

        public bool isDontDestroy = false;
        public IMCBlocker.ShowStyleEnum blockerShowStyle = IMCBlocker.ShowStyleEnum.Transparent;//拦截器显示风格枚举

        public UnityAction InitializeAction;
        public UnityAction ShowAction;
        public UnityAction CloseAction;
        public UnityAction UnInitAction;
        private int m_SiblingIndex = 0;
        /// <summary>
        /// 显示方法
        /// </summary>
        public virtual void Show()
        {
            Show(m_showTargetPosition, m_isBlockerToggle, blockerShowStyle, showCallBack, blockerCallBack);
        }
        public UnityAction blockerCallBack;
        public UnityEvent showCallBack;
        public UnityEvent closeCallBack;
        /// <summary>
        /// show方法重载
        /// </summary>
        public virtual void Show(Vector3 showTargetPosition, bool blockerToggle = false, IMCBlocker.ShowStyleEnum showStyle = IMCBlocker.ShowStyleEnum.Transparent, UnityEvent showCallBack = null, UnityAction blockerCallBack = null)
        {
            m_SiblingIndex = this.GetSiblintIndex();
            CanvasGroupSwitch(true);

            List<IMCUIBehaviour> tempList = new List<IMCUIBehaviour>();
            tempList.Add(this);
            if (blockerToggle)
            {
                blocker = IMCBlocker.Instance.Create(tempList, showStyle, blockerCallBack);
            }

            selfType = this.GetType();//2017年8月3日17:02:40 动态加载form时，并不会调用初始化方法
            System.Reflection.MethodInfo methodInfo = selfType.GetMethod("ShowEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

            if (methodInfo != null)
                methodInfo.Invoke(this, null);
            if (ShowAction != null)
                ShowAction();
            rectTransform.anchoredPosition3D = showTargetPosition;

            if (showCallBack != null)
                showCallBack.Invoke();
        }
        /// <summary>
        /// 关闭方法
        /// </summary>
        public virtual void Close()
        {
            Close(m_closeTargetPosition, closeCallBack);
        }
        /// <summary>
        /// Close方法重载
        /// </summary>
        public virtual void Close(Vector3 closeTargetPosition, UnityEvent closeCallBack = null)
        {
            CanvasGroupSwitch(false);
            selfType = this.GetType();//2017年8月3日17:02:40 动态加载form时，并不会调用初始化方法
            System.Reflection.MethodInfo methodInfo = selfType.GetMethod("CloseEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (methodInfo != null)
                methodInfo.Invoke(this, null);
            if (CloseAction != null)
                CloseAction();
            if (blocker)
            {
                IMCBlocker.Instance.RemoveStack(this);
                this.SetSiblingIndex(m_SiblingIndex);
            }
            rectTransform.anchoredPosition3D = closeTargetPosition;
            if (closeCallBack != null) closeCallBack.Invoke();
        }

        public override void UnInit(int time = 0)
        {
            selfType = this.GetType();//2017年8月3日17:02:40 动态加载form时，并不会调用初始化方法
            System.Reflection.MethodInfo methodInfo = selfType.GetMethod("UnInitEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (methodInfo != null)
                methodInfo.Invoke(this, null);

            if (UnInitAction != null)
                UnInitAction();
            //if (blocker)//2017年7月21日14:46:03 已移至imcuibehaviour脚本中
            //{
            //    IMCBlocker.RemoveBlocker();
            //    //this.SetSiblingIndex(m_SiblingIndex);
            //}
            if (canvas)
                canvas.UnRegisterForm(this);
            base.UnInit(time);
        }
        protected override void Awake()
        {
            base.Awake();            
        }
        private Type selfType;
        public override void Initialize()
        {
            m_controlType = ControlType.IMCForm;
            base.Initialize();
            if (isDontDestroy)
                DontDestroyOnLoad(this.gameObject);
            selfType = this.GetType();//2017年8月3日17:02:40 动态加载form时，并不会调用初始化方法
            System.Reflection.MethodInfo methodInfo = selfType.GetMethod("InitializeEvent",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public);
            if (methodInfo != null)
                methodInfo.Invoke(this, null);
            if (initialize)
                return;
            if (InitializeAction != null)
                InitializeAction();
        }
        protected override void OnDestroy()
        {
            if (this.canvas)
                this.canvas.UnRegisterForm(this);
        }
    }
}