using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace IMCUI.UI
{
    [DisallowMultipleComponent]
    public class IMCTabView : IMCUIContainer
    {
        public List<IMCTabViewToggle> toggles = new List<IMCTabViewToggle>();
        public List<IMCUIBehaviour> contents = new List<IMCUIBehaviour>();
        private int currentIndex;
        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCTabView;
        }
        protected override void OnEnable()
        {
            bool tempBo = true;
            if (contents.Count > 0)
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    if (i != 0)
                        tempBo = false;

                    toggles[i].toggle.isOn = tempBo;
                    contents[i].gameObject.SetActive(tempBo);
                }
            }
        }
        public void TabViewControlShowOrClose(bool nuclearBombSwitch, IMCTabViewToggle tvt)// 内部调用
        {
            if (nuclearBombSwitch)//need judge otherwise collapse
            {
                if (toggles.Count > 0 && toggles.Count == contents.Count)
                {
                    for (int i = 0; i < toggles.Count; i++)
                    {
                        if (toggles[i] == tvt)
                        {
                            tvt.toggle.isOn = true;
                            contents[i].gameObject.SetActive(true);
                            currentIndex = i;
                        }
                        if (toggles[i] != tvt)
                        {
                            toggles[i].toggle.isOn = false;
                            contents[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        [Obsolete("this function invalid, because this is TabView, so please call AddControl reloading!But ,In order to fault-tolerant ,this function call AddControl reloading action,parameter 'index' is 0.")]
        public override void AddControl(IMCUIBehaviour control)
        {
            AddControl(0, control);
        }
        [Obsolete("this function invalid, because this is TabView, so please call AddControl reloading!But ,In order to fault-tolerant ,this function call AddControl reloading action,parameter 'index' is 0.")]
        public override void AddControl(IMCUIBehaviour control, Vector3 pos)
        {
            AddControl(0, control, pos);
        }
        /// <summary>
        /// 在使用TabView的AddControl函数需要调用这个函数
        /// </summary>
        /// <param name="index">contents数组中的索引</param>
        public void AddControl(int index, IMCUIBehaviour control)
        {
            if (index < 0)
                index = 0;
            else if (index > contents.Count - 1)
                index = contents.Count - 1;

            MAddControl(contents[index], control);
        }
        /// <summary>
        /// 在使用TabView的AddControl函数需要调用这个函数
        /// </summary>
        /// <param name="index">contents数组中的索引</param>
        public void AddControl(int index, IMCUIBehaviour control, Vector3 pos)
        {
            if (index < 0)
                index = 0;
            else if (index > contents.Count - 1)
                index = contents.Count - 1;

            MAddControl(contents[index], control);
            control.rectTransform.anchoredPosition3D = pos;
        }
        /// <summary>
        /// 在使用TabView的AddControl函数需要调用这个函数
        /// </summary>
        /// <param name="content">contents数组中对应的元素</param>
        public void AddControl(IMCUIBehaviour content, IMCUIBehaviour control)//2017年7月21日16:02:13 添加函数
        {
            if (contents.Contains(content))
            {
                MAddControl(content, control);
            }
        }
        /// <summary>
        /// 在使用TabView的AddControl函数需要调用这个函数
        /// </summary>
        /// <param name="content">contents数组中对应的元素</param>
        public void AddControl(IMCUIBehaviour content, IMCUIBehaviour control, Vector2 pos)
        {
            MAddControl(content, control);
            control.rectTransform.anchoredPosition3D = pos;
        }
        private void MAddControl(IMCUIBehaviour content, IMCUIBehaviour control)
        {
            control.Unparent();
            control.parent = this;
            control.transform.SetParent(content.transform);
            control.transform.localScale = Vector3.one;
            control.Initialize();
            base.AddControl(control);
            //GetChildControls().Add(control.gameObject);
        }
        protected override void InitializeChildControls()//2017年12月27日14:44:57 更改获取子物体初始化逻辑
        {
            //GetChildControls().Clear();
            //RemoveAllControlAndDestroy();//2017年11月27日18:12:11
            for (int i = 0; i < contents.Count; i++)
            {
                m_contentsChildParent = contents[i];
                for (int j = 0; j < contents[i].transform.childCount; j++)
                {
                    IMCUIBehaviour[] imcs = contents[i].transform.GetChild(j).GetComponents<IMCUIBehaviour>();
                    if (imcs!=null&& imcs.Length>0)
                    {
                        for (int k = 0; k < imcs.Length; k++)
                        {
                            imcs[k].parent = this;
                            imcs[k].Initialize();
                            if (k==imcs.Length-1)
                                base.AddControl(imcs[k]);
                        }
                    }
                }
            }
            //m_contentsChildParent = null;
        }
        private IMCUIBehaviour m_contentsChildParent = null;
        protected override RectTransform SetParentObject()
        {
            return m_contentsChildParent == null ? this.rectTransform : m_contentsChildParent.rectTransform;
        }
        /// <summary>
        /// 获取当前正在执行的content组件
        /// </summary>
        public int GetCurrentOperationContentGameObjectIndex()
        {
            return currentIndex;
        }
        public void SetShowContent(int index)
        {
            bool tempBo = true;
            index = index < 0 ? 0 : index;
            index = index > (contents.Count - 1) ? contents.Count - 1 : index;
            if (contents.Count > 0)
            {
                for (int i = 0; i < contents.Count; i++)
                {
                    if (i != index)
                        tempBo = false;
                    else
                        tempBo = true;
                    toggles[i].toggle.isOn = tempBo;
                    contents[i].gameObject.SetActive(tempBo);
                }
            }
            currentIndex = index;
        }
    }
}