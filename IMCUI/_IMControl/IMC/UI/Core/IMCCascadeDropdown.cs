using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace IMCUI.UI
{
    //2017年6月28日11:56:40
    //级联·下拉组件
    [DisallowMultipleComponent]
    [AddComponentMenu("IMCUI/IMCCascadeDropdown", 50)]
    public class IMCCascadeDropdown : IMCUIBehaviour
    {
        private List<IMCDropdown> m_dropDowns;

        public List<IMCDropdown> dropDowns
        {
            get { return m_dropDowns; }
            set { m_dropDowns = value; }
        }
        private CanvasGroup m_canvasGroup;
        private CanvasGroup canvasGroup
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
            m_dropDowns = new List<IMCDropdown>();
            m_controlType = ControlType.IMCCascadeDropDown;
            m_containerType = ContainerType.Control;
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
        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                if (this.transform.GetChild(i).GetComponent<IMCDropdown>())
                {
                    PrivateIMCCascadeDropDown tenmpPrivateIMCCascadeDropDown = this.transform.GetChild(i).gameObject.AddComponent<PrivateIMCCascadeDropDown>();
                    tenmpPrivateIMCCascadeDropDown.parent = this;
                    dropDowns.Add(this.transform.GetChild(i).GetComponent<IMCDropdown>());
                }
            }
            m_CurrentDropdown = dropDowns[0];
        }
        /// <summary>
        /// 通过参数dropDown返回对应m_dropDowns数组中的索引（如果没有对应的索引，则返回-1）
        /// </summary>
        public int GetDropDownsIndex(IMCDropdown dropDown)
        {
            if (dropDowns.Contains(dropDown))
            {
                for (int i = 0; i < dropDowns.Count; i++)
                {
                    if (dropDown == dropDowns[i])
                        return i;
                }
            }
            return -1;
        }
        private IMCDropdown m_CurrentDropdown;
        /// <summary>
        /// 获取当前正在工作的dropDown（默认是数组中第一个dropDown）
        /// </summary>
        public IMCDropdown currentDropdown
        {
            get { return m_CurrentDropdown; }
            set { m_CurrentDropdown = value; }
        }



        public class PrivateIMCCascadeDropDown : MonoBehaviour
        {
            [HideInInspector]
            public IMCCascadeDropdown parent;

            void Start()
            {
                this.GetComponent<IMCDropdown>().HideItemAction = CascadeDropDownFunction;
            }

            private void CascadeDropDownFunction(int a)//触发后，让parent.dropDowns中下一个Dropdown下拉列表打开
            {
                int tempIndex = parent.GetDropDownsIndex(this.GetComponent<IMCDropdown>());
                if (tempIndex + 1 <= parent.dropDowns.Count - 1)
                {
                    parent.dropDowns[tempIndex + 1].Show();
                    parent.currentDropdown = parent.dropDowns[tempIndex + 1];
                }
            }
        }
    }
}