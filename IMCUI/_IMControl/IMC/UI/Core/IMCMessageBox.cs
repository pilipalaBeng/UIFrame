using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace IMCUI.UI
{
    [DisallowMultipleComponent]
    public class IMCMessageBox : IMCUIBehaviour
    {
        public struct MessageBoxButtonAttribute
        {
            public string buttonTitle;
            public Sprite buttonSprite;
            public UnityAction buttonAction;
        }

        public IMCImage background;
        public IMCText title;
        public IMCText content;
        public GameObject buttonParent;
        public IMCButton closeButton;
        public GameObject closeObject;
        public List<IMCButton> btns = new List<IMCButton>();

        private UnityAction<int> choiceBack;
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
            m_controlType = ControlType.IMCMessageBox;
            m_containerType = ContainerType.Control;

            GetButtonChilds();
            SetCloseObject();
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
        /// <summary>
        /// 获取buttonParent(ButtonList)下的所有按钮，如在场景运行前数组不为空的话，默认该数组在运行前已经把按钮手动设置好，就不会执行查找操作。
        /// 向已存在数组中的按钮中的委托onPointerClick添加SendChoiceMessage函数
        /// </summary>
        protected void GetButtonChilds()
        {
            if (btns.Count == 0 && buttonParent)
            {
                IMCButton tempButton;
                for (int i = 0; i < buttonParent.transform.childCount; i++)
                {
                    if (tempButton = buttonParent.transform.GetChild(i).GetComponent<IMCButton>())
                        btns.Add(tempButton);
                }
            }
            if (btns.Count > 0)
                for (int i = 0; i < btns.Count; i++)
                    btns[i].onPointerClick = SendChoiceMessage;
        }
        private void SetCloseObject()//设置close按钮所关闭的对象，如果closeObject对象在运行前为null的话，默认关闭的对象为IMCMessageBox自身
        {
            if (closeButton)
            {
                closeButton.AddListener(() =>
                {
                    UnInit();
                    if (closeObject && closeObject != this.gameObject)
                        if (closeObject.GetComponent<IMCUIBehaviour>())    //2018年1月4日16:47:19 判断如果是ui类就执行基类中的UnInit函数
                            closeObject.GetComponent<IMCUIBehaviour>().UnInit();
                        else
                            Destroy(closeObject.gameObject);
                });
            }
        }
        /// <summary>
        /// 部署MessageBox
        /// </summary>
        public void Deploy(UnityAction<int> choiceBack, string messageBoxTitle, string messageBoxContent, Sprite messageBoxBackground, MessageBoxButtonAttribute[] messageBoxAttribute)
        {
            Deploy(choiceBack, messageBoxTitle, messageBoxContent, messageBoxBackground);
            int tempButtonListLength = 0;
            if (messageBoxAttribute != null)
                tempButtonListLength = messageBoxAttribute.Length;

            if (tempButtonListLength > btns.Count)
                tempButtonListLength = btns.Count;

            if (tempButtonListLength != 0)
                for (int i = 0; i < tempButtonListLength; i++)
                    SetButton(btns[i], messageBoxAttribute[i].buttonTitle, messageBoxAttribute[i].buttonSprite, messageBoxAttribute[i].buttonAction);
        }
        /// <summary>
        /// 部署MessageBox
        /// </summary>
        public void Deploy(UnityAction<int> choiceBack)
        {
            this.choiceBack = choiceBack;
        }
        /// <summary>
        /// 部署MessageBox
        /// </summary>
        public void Deploy(UnityAction<int> choiceBack, string messageBoxTitle, string messageBoxContent)
        {
            Deploy(choiceBack);
            title.text = messageBoxTitle;
            content.text = messageBoxContent;
        }
        /// <summary>
        /// 部署MessageBox
        /// </summary>
        public void Deploy(UnityAction<int> choiceBack, string messageBoxTitle, string messageBoxContent, Sprite messageBoxBackground)
        {
            Deploy(choiceBack, messageBoxTitle, messageBoxContent);
            background.sprite = messageBoxBackground;
        }
        /// <summary>
        /// 设置btns数组下对应按钮属性
        /// </summary>
        /// <param name="buttonIndex">按钮所在btns数组中的索引查找对应的按钮（如果索引值非法，不会进行任何操作）</param>
        public void SetButton(int buttonIndex, string buttonTitle = "", Sprite buttonSprite = null, UnityAction buttonAction = null)
        {
            if (buttonIndex >= 0 && buttonIndex <= btns.Count - 1)
                MSetButton(btns[buttonIndex], buttonTitle, buttonSprite, buttonAction);
        }
        /// <summary>
        /// 设置btns数组下对应按钮属性
        /// </summary>
        /// <param name="buttonIndex">按钮所在btns数组中的索引查找对应的按钮（如果索引值非法，不会进行任何操作）</param>
        public void SetButton(int buttonIndex, MessageBoxButtonAttribute buttonAttribute)
        {
            if (buttonIndex >= 0 && buttonIndex <= btns.Count - 1)
                MSetButton(btns[buttonIndex], buttonAttribute.buttonTitle, buttonAttribute.buttonSprite, buttonAttribute.buttonAction);
        }
        /// <summary>
        /// 设置btns数组下对应按钮属性
        /// </summary>
        /// <param name="buttonObject">btns数组中对应的按钮（如果buttonObject不存在于btns数组中，不会进行任何操作）</param>
        public void SetButton(IMCButton buttonObject, string buttonTitle = "", Sprite buttonSprite = null, UnityAction buttonAction = null)
        {
            if (btns.Contains(buttonObject))
                MSetButton(buttonObject, buttonTitle, buttonSprite, buttonAction);
        }
        /// <summary>
        /// 设置btns数组下对应按钮属性
        /// </summary>
        /// <param name="buttonObject">btns数组中对应的按钮（如果buttonObject不存在于btns数组中，不会进行任何操作）</param>
        public void SetButton(IMCButton buttonObject, MessageBoxButtonAttribute buttonAttribute)
        {
            if (btns.Contains(buttonObject))
                MSetButton(buttonObject, buttonAttribute.buttonTitle, buttonAttribute.buttonSprite, buttonAttribute.buttonAction);
        }
        private void MSetButton(IMCButton buttonObject, string buttonTitle, Sprite buttonSprite, UnityAction buttonAction)
        {
            if (buttonObject.Text && buttonTitle != "")
                buttonObject.Text.text = buttonTitle;
            if (buttonAction != null)
                buttonObject.AddListener(buttonAction);
            if (buttonSprite != null)
                buttonObject.image.sprite = buttonSprite;
        }
        /// <summary>
        /// 移除btns数组中指定的按钮
        /// </summary>
        /// <param name="buttonIndex">btns数组中的索引（如果索引值非法，不会进行任何操作）</param>
        public void RemovButtonElement(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex <= btns.Count - 1)
            {
                IMCButton tempButton = btns[buttonIndex];
                btns.Remove(tempButton);
                tempButton.UnInit();
            }
        }
        /// <summary>
        ///  移除btns数组中指定的按钮
        /// </summary>
        /// <param name="buttonObject">btn数组中的button对象（如果buttonObject不存在于btns数组中，不会进行任何操作）</param>
        public void RemoveButtonElement(IMCButton buttonObject)
        {
            if (btns.Contains(buttonObject))
            {
                btns.Remove(buttonObject);
                Destroy(buttonObject.gameObject);
            }
        }

        protected void SendChoiceMessage(GameObject go)//按下btns数组下的任意按钮后触发该方法，向choiceBack委托发送 是第几号按钮触发了该函数（如果choiceBack为空，则不执行choice委托），并销毁IMCMessageBox自身
        {
            for (int i = 0; i < btns.Count; i++)
            {
                if (go == btns[i])
                    if (choiceBack != null)
                        choiceBack(i);
            }
            UnInit();
        }
    }
}
