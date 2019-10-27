using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace IMCUI.UI
{
    public class IMCUIContainer : IMCUIBehaviour
    {
        private List<GameObject> controls = new List<GameObject>();

        private CanvasGroup m_canvasGroup;

        public CanvasGroup canvasGroup
        {
            get
            {
                if (!m_canvasGroup)
                {
                    m_canvasGroup = this.GetComponent<CanvasGroup>();
                }
                return m_canvasGroup;
            }
        }

        private IMCImage m_image;

        public IMCImage image
        {
            get
            {
                if (!m_image)
                {
                    m_image = this.GetComponent<IMCImage>();
                    if (!m_image)
                        m_image = this.gameObject.AddComponent<IMCImage>();
                }
                return m_image;
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
        protected override void Awake()
        {
            base.Awake();

            m_containerType = ContainerType.Container;
        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        public override void Initialize()
        {
            //if (initialize)
            //    return;
            base.Initialize();
            InitializeChildControls();
            //initialize = true;
        }

        public override IMCUIContainer GetContainerObject()
        {
            return this;
        }
        /// <summary>
        /// 内部调用
        /// </summary>
        protected virtual RectTransform SetParentObject()//依靠重写特性更改父级对象
        {
            return this.rectTransform;
        }
        /// <summary>
        /// 向controls数组添加一个IMCUIBehaviour 类型的组件(在tabView容器对象中的AddControl函数与当前AddControl函数不同)
        /// </summary>
        public virtual void AddControl(IMCUIBehaviour control)
        {
            control.Unparent();
            control.parent = this;
            control.rectTransform.SetParent(SetParentObject());
            control.transform.localScale = Vector3.one;//2017年5月25日14:34:46  添加 被添加 空间强制缩放大小为  1.1.1
            control.Initialize();
            controls.Add(control.gameObject);
        }
        /// <summary>
        /// 向controls数组添加一个IMCUIBehaviour类型的组件，并指定其Vector2类型的位置(在tabView容器对象中的AddControl函数与当前AddControl函数不同)
        /// </summary>
        public virtual void AddControl(IMCUIBehaviour control, Vector3 pos)
        {
            AddControl(control);
            control.rectTransform.anchoredPosition3D = pos;
        }
        /// <summary>
        /// 将list的元素依次被调用进IMCForm中的AddControl的方法(在tabView容器对象中的AddControl函数与当前AddControl函数不同)
        /// </summary>
        public virtual void AddControls(List<IMCUIBehaviour> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                AddControl(list[i]);
            }
        }
        /// <summary>
        /// 将list的元素和poss的元素依次被调用进IMCForm中的AddControl的方法(在tabView容器对象中的AddControl函数与当前AddControl函数不同)
        /// </summary>
        public virtual void AddControls(List<IMCUIBehaviour> list, List<Vector2> posList)
        {
            for (int i = 0; i < list.Count; i++)
            {
                AddControl(list[i], posList[i]);
            }
        }
        /// <summary>
        /// 移除controls数组中对应的元素
        /// </summary>
        public virtual void RemoveControl(IMCUIBehaviour control)
        {
            if (controls.Contains(control.gameObject))
            {
                control.parent = null;
                control.rectTransform.SetParent(null);
                controls.Remove(control.gameObject);
            }
        }
        /// <summary>
        /// 移除controls数组中对应的元素，并将其销毁(无论control是否在controls数组中，函数执行完毕后，都会销毁control对象)
        /// </summary>
        public virtual void RemoveControlAndDestroy(IMCUIBehaviour control)
        {
            if (control)
            {
                RemoveControl(control);
                control.UnInit();
            }
        }
        /// <summary>
        /// 移除controls数组汇总的所有元素
        /// </summary>
        public virtual void RemoveAllControl()
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].GetComponent<IMCUIBehaviour>().parent = null;
                    controls[i].GetComponent<IMCUIBehaviour>().rectTransform.parent = null;
                    controls.Remove(controls[i]);
                }
            }
        }
        /// <summary>
        /// 移除controls数组中的所有元素，并将其销毁
        /// </summary>
        public virtual void RemoveAllControlAndDestroy()
        {
            for (int i = 0; i < controls.Count; i++)
            {
                //controls[i].parent = null;
                //controls.Remove(controls[i]);//2017年8月4日16:15:06 逻辑Bug
                controls[i].GetComponent<IMCUIBehaviour>().UnInit();
            }
            controls.Clear();
        }
        /// <summary>
        ///  通过泛型类型，返回controls数组中对应的元素（通过正向查找，返回第一个符合条件的元素）,如果没有符合条件的元素，返回null
        /// </summary>
        public virtual T FindControl<T>() where T : IMCUIBehaviour//2017年7月21日15:54:25 添加单个对象查找函数
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i].GetComponent<T>() != null)
                        return controls[i].GetComponent<T>();
                }
            }
            return default(T);
        }
        /// <summary>
        /// 通过组件的customID获取controls数组对应的元素（通过正向查找，返回第一个符合条件的元素），获取不到的话返回null
        /// </summary>
        public virtual IMCUIBehaviour FindControlByCustomID(string customID)
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID)
                            return controls[i].GetComponents<IMCUIBehaviour>()[j];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 通过泛型类型以及customID获取controls数组对应的元素（通过正向查找，返回第一个符合条件的元素），获取不到的话返回null
        /// </summary>
        public virtual T FindControlByCustomID<T>(string customID) where T : IMCUIBehaviour
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID && controls[i].GetComponents<IMCUIBehaviour>()[j].GetComponent<T>() != null)
                            return controls[i].GetComponents<IMCUIBehaviour>()[j].GetComponent<T>();
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        ///  通过组件的name获取controls数组对应的元素（通过正向查找，返回第一个符合条件的元素），获取不到的话返回null
        /// </summary>
        public virtual IMCUIBehaviour FindControlByName(string name)
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i].name == name)
                        return controls[i].GetComponent<IMCUIBehaviour>();
                }
            }
            return null;
        }
        /// <summary>
        /// 通过泛型类型以及name获取controls数组对应的元素（通过正向查找，返回第一个符合条件的元素）,获取不到的话返回null
        /// </summary>
        public virtual T FindControlByName<T>(string name) where T : IMCUIBehaviour
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i].name == name && controls[i].GetComponent<T>() != null)
                        return controls[i].GetComponent<T>();
                }
            }
            return default(T);
        }
        /// <summary>
        /// 通过name，以及customID获取controls数组中对应的元素（通过正向查找，返回第一个符合条件的元素），获取不到的话返回null
        /// </summary>
        public virtual IMCUIBehaviour FindControlByNameAndCustomID(string name, string customID)
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].name == name && controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID)
                            return controls[i].GetComponent<IMCUIBehaviour>();
                    }
                }
            }
            return null;
        }
        /// <summary>
        ///  通过泛型类型，name以及customID获取controls数组中对应的元素（通过正向查找，返回第一个符合条件的元素），获取不到的话返回null
        /// </summary>
        public virtual T FindControlByNameAndCustomID<T>(string name, string customID) where T : IMCUIBehaviour
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].name == name && controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID && controls[i].GetComponent<T>() != null)
                        {
                            return controls[i].GetComponent<T>();
                        }
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// 通过泛型类型，返回controls数组中的所有的泛型数组，如果没有符合条件的元素，返回null
        /// </summary>
        public virtual List<T> FindControls<T>() where T : IMCUIBehaviour
        {
            List<T> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<T>();
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i].GetComponent<T>() != null)
                        tempControls.Add(controls[i].GetComponent<T>());
                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过组件的customID获取controls数组对应的元素列表，获取不到的话返回null
        /// </summary>
        public virtual List<IMCUIBehaviour> FindControlsByCustomID(string customID)
        {
            List<IMCUIBehaviour> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<IMCUIBehaviour>();
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID)
                        {
                            tempControls.Add(controls[i].GetComponents<IMCUIBehaviour>()[j]);
                        }
                    }

                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过泛型类型以及customID获取controls数组对应的元素列表，获取不到的话返回null
        /// </summary>
        public virtual List<T> FindControlsByCustomID<T>(string customID) where T : IMCUIBehaviour
        {
            List<T> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<T>();
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID && controls[i].GetComponent<T>() != null)
                            tempControls.Add(controls[i].GetComponent<T>());
                    }
                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过组件的name获取controls数组对应的元素列表，获取不到的话返回null
        /// </summary>
        public virtual List<IMCUIBehaviour> FindControlsByName(string name)
        {
            List<IMCUIBehaviour> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<IMCUIBehaviour>();
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i].name == name)
                        tempControls.Add(controls[i].GetComponent<IMCUIBehaviour>());
                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过泛型类型以及name获取controls数组对应的元素列表,获取不到的话返回null
        /// </summary>
        public virtual List<T> FindControlsByName<T>(string name) where T : IMCUIBehaviour
        {
            List<T> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<T>();
                for (int i = 0; i < controls.Count; i++)
                {
                    if (controls[i].name == name && controls[i].GetComponent<T>() != null)
                        tempControls.Add(controls[i].GetComponent<T>());
                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过name，以及customID获取controls数组中对应的元素列表，获取不到的话返回null
        /// </summary>
        public virtual List<IMCUIBehaviour> FindControlsByNameAndCustomID(string name, string customID)
        {
            List<IMCUIBehaviour> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<IMCUIBehaviour>();
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].name == name && controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID)
                            tempControls.Add(controls[i].GetComponent<IMCUIBehaviour>());
                    }
                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过泛型类型，name以及customID获取controls数组中对应的元素列表，获取不到的话返回null
        /// </summary>
        public virtual List<T> FindControlsByNameAndCustomID<T>(string name, string customID) where T : IMCUIBehaviour
        {
            List<T> tempControls = null;
            if (controls.Count > 0)
            {
                tempControls = new List<T>();
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].name == name && controls[i].GetComponents<IMCUIBehaviour>()[j].customID == customID && controls[i].GetComponent<T>() != null)
                            tempControls.Add(controls[i].GetComponent<T>());
                    }
                }
            }
            if (tempControls.Count > 0)
                return tempControls;
            return null;
        }

        /// <summary>
        /// 通过组件的intanceID获取controls对应的元素，获取不到的话返回null
        /// </summary>
        public virtual IMCUIBehaviour FindControlByIntanceID(int intanceID)
        {
            if (controls.Count > 0)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    for (int j = 0; j < controls[i].GetComponents<IMCUIBehaviour>().Length; j++)
                    {
                        if (controls[i].GetComponents<IMCUIBehaviour>()[j].intanceID == intanceID)
                        {
                            return controls[i].GetComponents<IMCUIBehaviour>()[j];
                        }
                    }
                }
            }
            return null;
        }

        protected virtual void InitializeChildControls()//获取子物体
        {
            controls.Clear();
            bool tempToggle = true;
            //if (this.controlType == ControlType.IMCForm)
            //    this.form = (IMCForm)this;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                IMCUIBehaviour imc = this.transform.GetChild(i).GetComponent<IMCUIBehaviour>();
                if (imc)
                {
                    //imc.form = this.form;
                    for (int k = 0; k < imc.GetComponents<IMCUIBehaviour>().Length; k++)
                    {
                        if (tempToggle)
                        {
                            if (imc.GetComponent<IMCUIContainer>())
                            {
                                if (imc.GetComponent<IMCScrollRect>())
                                    imc.GetComponent<IMCScrollRect>().Initialize();
                                else if (imc.GetComponent<IMCTabView>())
                                    imc.GetComponent<IMCTabView>().Initialize();
                                else
                                    imc.GetComponent<IMCUIContainer>().Initialize();
                                tempToggle = false;
                            }
                            else
                            {
                                imc.GetComponents<IMCUIBehaviour>()[k].Initialize();
                            }
                        }
                        imc.GetComponents<IMCUIBehaviour>()[k].parent = this;
                    }
                    tempToggle = true;
                    controls.Add(imc.gameObject);
                }
            }
        }
        /// <summary>
        /// 获取对象容器的子物体数组controls
        /// </summary>
        public List<IMCUIBehaviour> GetChildControls()
        {
            List<IMCUIBehaviour> tempList = new List<IMCUIBehaviour>();
            for (int i = 0; i < controls.Count; i++)
            {
                tempList.Add(controls[i].GetComponent<IMCUIBehaviour>());
            }
            return tempList;
        }
        /// <summary>
        /// 设置CanvasGroup组件的raycast ；interact；alpha
        /// </summary>
        protected void CanvasGroupSwitch(bool m_switch)
        {
            raycast = m_switch;
            interact = m_switch;//2017年7月4日10:44:29 不确定是否有用
            alpha = m_switch ? 1 : 0;
        }

        /// <summary>
        /// 搬家函数，将control对象从form容器对象中移至to容器对象中（如果control不存在于form容器controls数组中，程序不会执行任何操作。如果to容器是tabView类型容器，还需要指定tabViewIndex传入具体的tabcontent索引。）
        /// </summary>
        public static void MoveHouse(IMCUIBehaviour control, IMCUIContainer form, IMCUIContainer to, Vector2 pos, int tabViewIndex = 0)
        {
            if (form.GetChildControls().Contains(control))
            {
                form.RemoveControl(control);
                if (to.GetComponent<IMCTabView>())
                    to.GetComponent<IMCTabView>().AddControl(tabViewIndex, control, pos);
                else if (to.GetComponent<IMCScrollRect>())
                    to.GetComponent<IMCScrollRect>().AddControl(control, pos);
                else
                    to.AddControl(control, pos);
            }
        }
    }
}