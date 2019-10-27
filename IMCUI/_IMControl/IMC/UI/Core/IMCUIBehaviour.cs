using UnityEngine;
using UnityEngine.Events;
using System;
namespace IMCUI.UI
{
    public enum ControlType
    {
        None,
        IMCProgressBar,
        IMCText,
        IMCImage,
        IMCRawImage,
        IMCButton,
        IMCToggle,
        IMCSlider,
        IMCScrollbar,
        IMCDropdown,
        IMCInputField,
        IMCScrollRect,
        IMCForm,
        IMCTabView,
        IMCGroup,
        IMCMessageBox,
        IMCCascadeDropDown,
        IMCSwitchButton,
        IMCBlocker,
        IMCOrderShowButtons,
        IMCCanvas,
    }
    public enum ContainerType
    {
        Control,
        Container,

        Other,
    }

    public abstract class IMCUIBehaviour : MonoBehaviour
    {
        #region
        protected virtual void Awake()
        { }

        protected virtual void OnEnable()
        { }

        protected virtual void Start()
        { }

        protected virtual void OnDisable()
        { }

        protected virtual void OnDestroy()
        {
            if (this.m_parent != null && this.m_parent.m_containerType == ContainerType.Container)
            {
                if (this.m_parent.GetComponent<IMCUIContainer>())
                    this.m_parent.GetComponent<IMCUIContainer>().RemoveControl(this);
            }
        }

        public virtual bool IsActive()
        {
            return isActiveAndEnabled;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        { }

        protected virtual void Reset()
        { }
#endif

        protected virtual void OnRectTransformDimensionsChange()
        { }

        protected virtual void OnBeforeTransformParentChanged()
        { }

        protected virtual void OnTransformParentChanged()
        { }

        protected virtual void OnDidApplyAnimationProperties()
        { }

        protected virtual void OnCanvasGroupChanged()
        { }

        protected virtual void OnCanvasHierarchyChanged()
        { }

        public bool IsDestroyed()
        {
            // Workaround for Unity native side of the object
            // having been destroyed but accessing via interface
            // won't call the overloaded ==
            return this == null;
        }
        #endregion

        public UnityAction<IMCBlocker> blockerAction;
        #region

        //private ViewState m_viewType;

        //public ViewState viewType
        //{
        //    get { return m_viewType; }
        //    set { m_viewType = value; }
        //}

        protected ControlType m_controlType;

        public ControlType controlType
        {
            get { return m_controlType; }
        }
        protected ContainerType m_containerType;

        public ContainerType containerType
        {
            get { return m_containerType; }
        }
        private IMCUIBehaviour m_parent;

        public IMCUIBehaviour parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }
        private IMCBlocker m_blocker;

        public IMCBlocker blocker
        {
            get { return m_blocker; }
            set { m_blocker = value; }
        }

        protected bool initialize = false;

        private RectTransform m_rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (!m_rectTransform)
                    m_rectTransform = this.gameObject.GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        public string customID;//自定义ID

        public int intanceID
        {
            get { return GetInstanceID(); }
        }

        public bool active
        {
            get
            {
                return gameObject.activeSelf;
            }
            set
            {
                gameObject.SetActive(value);
            }
        }

        public virtual bool raycast//射线
        {
            get
            {
                GetGraphics();
                for (int i = 0; i < m_graphics.Length; i++)
                {
                    if (m_graphics[i].raycastTarget)
                        return true;
                }
                return false;
            }
            set
            {
                GetGraphics();
                for (int i = 0; i < m_graphics.Length; i++)
                {
                    m_graphics[i].raycastTarget = value;
                }
            }
        }

        private Graphic[] m_graphics;
        protected Graphic[] graphics
        {
            get
            {
                GetGraphics();
                return m_graphics;
            }
        }
        private void GetGraphics()
        {
            m_graphics = this.GetComponentsInChildren<Graphic>();
        }

        public virtual float alpha
        {
            get;
            set;
        }

        public virtual bool interact//交互
        {
            get;
            set;
        }

private IMCForm m_form;

        public IMCForm form
        {
            get
            {
                FindRootForm(gameObject);
                return m_form;
            }
        }
        public virtual void Initialize()
        {
            if (initialize)
            {
                return;
            }
            initialize = true;
}

        void FindRootForm(GameObject obj)
        {
            IMCForm uiobj = GetIMCUIBehaviourParent(obj);
            if (uiobj)
            {
                m_form = uiobj;
                return;
            }
            if (obj.transform.parent)
                FindRootForm(obj.transform.parent.gameObject);
        }

        IMCForm GetIMCUIBehaviourParent(GameObject obj)
        {
            return obj.GetComponent<IMCForm>();
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public virtual void UnInit(int time = 0)
        {
            if (gameObject)
            {
                if (blocker && (!blocker.Is_blockerPointerClick))//2017年9月1日10:08:12  
                    IMCBlocker.Instance.RemoveStack(this);
                time = time < 0 ? 0 : time;
                if (time == 0)
                    Destroy(gameObject);
                else
                    Destroy(gameObject, time);
            }
        }
        /// <summary>
        /// 获得容器对象
        /// </summary>
        public virtual IMCUIContainer GetContainerObject()
        {
            return null;
        }
        /// <summary>
        /// 返回组件的类型
        /// </summary>
        public virtual T GetBaseType<T>()
        {
            Type type = typeof(T);
            if (m_controlType.ToString() == type.Name)
            {
                return (T)(object)this;
            }
            return default(T);
        }

        /// <summary>
        /// 获取组件Hierarchy窗口中的层级
        /// </summary>
        public virtual int GetSiblintIndex()
        {
            return transform.GetSiblingIndex();
        }
        /// <summary>
        /// 设置组件Hierarchy窗口中的层级
        /// </summary>
        public virtual void SetSiblingIndex(int index)
        {
            if (index < 0)
                index = 0;
            transform.SetSiblingIndex(index);
        }
        /// <summary>
        /// 将组件移动至Hierarchy最上方
        /// </summary>
        public virtual void SetAsFirstSibling()
        {
            transform.SetAsFirstSibling();
        }
        /// <summary>
        /// 将组件移动至Hierarchy最下方
        /// </summary>
        public virtual void SetAsLastSibling()
        {
            transform.SetAsLastSibling();
        }
        /// <summary>
        /// 解除父物体
        /// </summary>
        public IMCUIBehaviour Unparent()
        {
            if (m_parent != null && m_parent.m_containerType == ContainerType.Container)
            {
                m_parent.GetContainerObject().RemoveControl(this);
                this.transform.parent = null;
            }
            return this;
        }

        public UnityAction TweenCallBack;

        public void MoveTo(Vector3 position, float time, UnityAction callBack = null)
        {
            StopCoroutine("MoveTo");
            StartCoroutine(FindIMCUIAnimation().MoveTo(this, position, time, callBack));
            if (TweenCallBack != null) TweenCallBack();
        }

        public void RotationTo(Vector3 angle, float time, UnityAction callBack = null)
        {
            StopCoroutine("RotationTo");
            StartCoroutine(FindIMCUIAnimation().RotationTo(this, angle, time, callBack));
            if (TweenCallBack != null) TweenCallBack();
        }

        public void ScaleTo(Vector3 size, float time, UnityAction callBack = null)
        {
            StopCoroutine("ScaleTo");
            StartCoroutine(FindIMCUIAnimation().ScaleTo(this, size, time, callBack));
            if (TweenCallBack != null) TweenCallBack();
        }

        //public virtual void ColorTo(Color targetColor, float time, UnityAction callBack = null)
        //{
        //    if (TweenCallBack != null) TweenCallBack();
        //    if (callBack != null) callBack();
        //    //need override
        //}

        public virtual void AlphaTo(float targetAlpha, float time, UnityAction callBack = null)//2017年7月4日16:03:37 新建 AlphaTo方法
        {
            StartCoroutine(FindIMCUIAnimation().AlphaTo(this, targetAlpha, time, callBack));
            if (TweenCallBack != null) TweenCallBack();
        }
        private IMCUIAnimation m_animation;
        private IMCUIAnimation FindIMCUIAnimation()
        {
            if (!m_animation)
                m_animation = this.gameObject.AddComponent<IMCUIAnimation>();
            return m_animation;
        }
        #endregion

        #region rectTransform
        ///<summary>
        ///设置自身在UI面板上的3D位置
        ///</summary>
        public Vector3 anchoredPosition3D
        {
            get { return rectTransform.anchoredPosition3D; }
            set { rectTransform.anchoredPosition3D = value; }
        }
        ///<summary>
        ///设置自身在UI面板上的2D位置
        ///</summary>
        public Vector2 anchoredPosition
        {
            get { return rectTransform.anchoredPosition; ; }
            set { rectTransform.anchoredPosition = value; }
        }

        public Vector2 anchorMax
        {
            get { return rectTransform.anchorMax; }
            set { rectTransform.anchorMax = value; }
        }

        public Vector2 anchorMin
        {
            get { return rectTransform.anchorMin; }
            set { rectTransform.anchorMin = value; }
        }

        public Vector2 offsetMax
        {
            get { return rectTransform.offsetMax; }
            set { rectTransform.offsetMax = value; }
        }

        public Vector2 offsetMin
        {
            get { return rectTransform.offsetMin; }
            set { rectTransform.offsetMin = value; }
        }
        /// <summary>
        /// 设置中心点
        /// </summary>
        public Vector2 pivot
        {
            get { return rectTransform.pivot; }
            set { rectTransform.pivot = value; }
        }
        public Rect rect
        {
            get { return rectTransform.rect; }
        }
        /// <summary>
        /// 设置大小
        /// </summary>
        public Vector2 sizeDelta
        {
            get { return rectTransform.sizeDelta; }
            set { rectTransform.sizeDelta = value; }
        }
        public void GetLocalCorners(Vector3[] fourCornersArray)
        {
            rectTransform.GetLocalCorners(fourCornersArray);
        }
        public void GetWorldCorners(Vector3[] fourCornersArray)
        {
            rectTransform.GetWorldCorners(fourCornersArray);
        }
        public void SetInsetAndSizeFromParentEdge(RectTransform.Edge edge, float inset, float size)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(edge, inset, size);
        }
        public void SetSizeWithCurrentAnchors(RectTransform.Axis axis, float size)
        {
            rectTransform.SetSizeWithCurrentAnchors(axis, size);
        }
        #endregion

        public void UIMirror(bool isMirror)
        {
            if (!isMirror)
                this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
            else
                this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }

        public UnityAction<LoadState> LoadStateChangeAction;
        public virtual void LoadComplete(IMCTaskLoader loader)
        {

        }

        public virtual void SetTexture(Texture texture)
        {

        }
    }
}