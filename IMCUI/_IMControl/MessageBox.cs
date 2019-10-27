using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
//using UnityEngine.UI;
using System;
using IMCUI.UI;

public class MessageBox : IMCForm
{
    #region PUBLIC_VARIABLES
    [Space(20)]
    [Tooltip("背景")]
    public Transform pic;//背景
    [Tooltip("标题栏")]
    public IMCText title;//标题栏
    [Tooltip("内容栏")]
    public IMCText content;//内容栏
    [Tooltip("按钮组")]
    public GameObject btns;//按钮组
    [Tooltip("关闭按钮")]
    public IMCButton closeButton;
    /// <summary>
    /// 关闭方式：默认销毁；
    /// </summary>
    public bool CloseDestroy = true;
    [Tooltip("按钮间距")]
    public float ButtonSpacing = 90f;
    public List<IMCButton> buttonChilds = new List<IMCButton>();
    public UnityAction closeBack;

    #endregion //PUBLIC_VARIABLES
    private UnityAction<int> choiceBack;
    private IMCHorizontalLayoutGroup horizontalComponent;

    #region UNITY_METHODS

    #endregion //UNITY_METHODS


    #region PUBLIC_METHODS

    //public void Create(string title, string content, UnityAction<int> choiceBack, string[] buttonTexts, Sprite[] buttonSprites, Sprite messageBoxSprite = null)
    //{
    //    this.choiceBack = choiceBack;
    //    TitleValue = title;
    //    ContentValue = content;
    //    if (messageBoxSprite != null)
    //        pic.GetComponent<IMCImage>().sprite = messageBoxSprite;
    //    switch (buttonTexts.Length)
    //    {
    //        case 0:
    //            btns.gameObject.SetActive(false);
    //            break;
    //        case 1:
    //            SetButtonChilds(0, buttonTexts, buttonSprites);
    //            break;
    //        case 2:
    //            SetButtonChilds(1, buttonTexts, buttonSprites);
    //            break;
    //        case 3:
    //            SetButtonChilds(2, buttonTexts, buttonSprites);
    //            break;
    //        default:
    //            IMCDebug.Log("string array:" + buttonTexts + "     length beyond");
    //            break;
    //    }
    //}
    public void Create(UnityAction<int> choiceBack)
    {
        this.choiceBack = choiceBack;
    }
    public void Create(string title, string content, UnityAction<int> choiceBack)
    {
        this.choiceBack = choiceBack;
        TitleValue = title;
        ContentValue = content;
    }
    public void Create(string title, string content, string btnText0, UnityAction<int> choiceBack)
    {
        this.choiceBack = choiceBack;
        TitleValue = title;
        ContentValue = content;
        buttonChilds[0].Create(btnText0);
        buttonChilds[0].active = true;
        buttonChilds[1].active = false;
        buttonChilds[2].active = false;
    }
    public void Create(string title, string content, string btnText0, string btnText1, UnityAction<int> choiceBack)
    {
        this.choiceBack = choiceBack;
        TitleValue = title;
        ContentValue = content;
        buttonChilds[0].Create(btnText0);
        buttonChilds[1].Create(btnText1);
        buttonChilds[0].active = true;
        buttonChilds[1].active = true;
        buttonChilds[2].active = false;
    }
    public void Create(string title, string content, string btnText0, string btnText1, string btnText2, UnityAction<int> choiceBack)
    {
        this.choiceBack = choiceBack;
        TitleValue = title;
        ContentValue = content;
        buttonChilds[0].Create(btnText0);
        buttonChilds[1].Create(btnText1);
        buttonChilds[2].Create(btnText2);
        buttonChilds[0].active = true;
        buttonChilds[1].active = true;
        buttonChilds[2].active = true;
    }

    /// <summary>
    /// 延时关闭
    /// </summary>
    /// <param name="seconds"></param>
    public void CloseDelay(float seconds)
    {
        Invoke("UnInitMessageBox", seconds);
    }

    #endregion //PUBLIC_METHODS


    #region OVERRIDE_METHODS

    private void ShowInitialize()
    {
        if (btns)
        {
            if (!btns.GetComponent<IMCHorizontalLayoutGroup>())
                btns.gameObject.AddComponent<IMCHorizontalLayoutGroup>();
            horizontalComponent = btns.GetComponent<IMCHorizontalLayoutGroup>();
        }
        GetButtonChilds();
        alpha = 0;
        raycast = false;
    }

    protected override void Awake()
    {
        ShowInitialize();
    }
    public void Show(bool showCloseButton = false)
    {
        if (closeButton)
            closeButton.gameObject.SetActive(showCloseButton);
        base.Show();
        ShowCoroutineCallBack();
    }
    public void Show(Vector3 targetPosition, bool showCloseButton = false)
    {
        if (closeButton)
            closeButton.gameObject.SetActive(showCloseButton);
        base.Show(showTargetPosition);
        ShowCoroutineCallBack();
    }
    private void ShowCoroutineCallBack()//重写父类ShowCoroutineCallBack方法
    {
        //SetCanvasGroup(true);
        CanvasGroupSwitch(true);
        //RegisteredIMCControl(this);
        //if (focusStyle != focusStyleEnum.Disable)
        //{
        //    SetFocus(focusStyle);//代表他是对话框
        //}
        raycast = true;
    }

    public override void Close()
    {
        //if (base.CloseEvent != null)
        //    base.CloseEvent(this);
        base.Close();
        CloseCoroutineCallBack();
    }

    public override void Close(Vector3 closeTargetPosition, UnityEvent closeCallBack = null)
    {
        base.Close(closeTargetPosition, closeCallBack);
        CloseCoroutineCallBack();
    }
    private void CloseCoroutineCallBack()
    {
        //base.CloseCoroutineCallBack();
        //SetRaycast(false);
        CanvasGroupSwitch(false);
    }
    //public override void SetRaycast(bool enable)//重写父类SetRaycast方法
    //{
    //    CanvasGroup.blocksRaycasts = enable;
    //    if (/*DRAGMOVE*/canMove)
    //    {
    //        if (enable)
    //        {
    //            //title.GetComponent<IMCText>().canMove = true;
    //        }
    //        else
    //        {
    //            //title.GetComponent<IMCText>().canMove = false;
    //        }
    //    }
    //}

    #endregion //OVERRIDE_METHODS


    #region PRIVATE_METHODS

    //public void RemoveButtonChild(IMCButton btn)
    //{
    //    if (buttonChilds.Contains(btn))
    //    {
    //        buttonChilds.Remove(btn);
    //        btn.UnInit();
    //    }
    //}
    public Sprite Sprite
    {
        get { return pic.GetComponent<IMCImage>().sprite; }
        set { pic.GetComponent<IMCImage>().sprite = value; }
    }

    public string TitleValue
    {
        get { return title.text; }
        set { title.text = value; }
    }

    public string ContentValue
    {
        get { return content.text; }
        set { content.text = value; }
    }
    protected void SendChoiceMessage(GameObject go)//选择发送消息
    {
        for (int i = 0; i < buttonChilds.Count; i++)
        {
            if (go == buttonChilds[i].gameObject)
            {
                if (choiceBack != null) choiceBack(i);
            }
        }
        if (CloseDestroy)
            UnInit();
        else
            Close();
    }

    protected void GetButtonChilds()//获取所有button类型子物体
    {
        buttonChilds.Clear();
        IMCButton j;
        if (btns.transform.childCount > 0)
        {
            for (int i = 0; i < btns.transform.childCount; i++)
            {
                if (j = btns.transform.GetChild(i).GetComponent<IMCButton>())
                {
                    buttonChilds.Add(j);
                    buttonChilds[i].onPointerClick = SendChoiceMessage;
                }
            }
        }
    }

    protected void SetButtonChilds(int numb, string[] buttonText, Sprite[] sprites)//设置button类型子物体
    {
        for (int i = 0; i < buttonChilds.Count; i++)
        {
            if (i <= numb)
            {
                //buttonChilds[i].SetCanvasGroup(true);
                buttonChilds[i].raycast = true;
                buttonChilds[i].alpha = 1;
                buttonChilds[i].Create(buttonText[i], sprites[i]);
            }
            else
            {
                buttonChilds.Remove(buttonChilds[i]);
                buttonChilds[i].UnInit();
            }
        }
        UpdateButtonSpacing();
    }
    /// <summary>
    /// 设置按钮之间的间距值，根据面板上的值
    /// </summary>
    public void UpdateButtonSpacing()
    {
        horizontalComponent.spacing = ButtonSpacing;
    }

    /// <summary>
    /// 设置Button之间的间距,推荐90~100之间
    /// </summary>
    public void UpdateButtonSpacing(float spacing)
    {
        ButtonSpacing = spacing;
        horizontalComponent.spacing = ButtonSpacing;
    }

    /// <summary>
    /// 设置所有数组的宽高，当然也可以设置指定索引按钮的宽高
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public void SetButtonsWidthHeight(float width = 125f, float height = 50f)
    {
        LayoutElement lay;
        for (int i = 0; i < buttonChilds.Count; i++)
        {
            lay = buttonChilds[i].GetComponent<LayoutElement>();
            lay.minWidth = width;
            lay.minHeight = height;
        }
    }

    /// <summary>
    /// 设置指定索引的按钮宽高
    /// </summary>
    /// <param name="buttonIndex">按钮索引</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public void SetButtonWidthHeight(int buttonIndex, float width, float height)
    {
        if (buttonIndex > -1 && buttonIndex < buttonChilds.Count - 1)
        {
            LayoutElement lay;
            lay = buttonChilds[buttonIndex].GetComponent<LayoutElement>();
            lay.minWidth = width;
            lay.minHeight = height;
            return;
        }
        IMCDebug.LogWarning("没有找到匹配的索引 -IMCMessageBox.cs -SetButtonWidthHeight()");
    }

    /// <summary>
    /// 设置指定名字符合按钮宽高
    /// </summary>
    /// <param name="buttonName">按钮名字</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    public void SetButtonWidthHeight(string buttonName, float width, float height)
    {
        LayoutElement lay;
        for (int i = 0; i < buttonChilds.Count; i++)
        {
            if (buttonChilds[i].name == buttonName)
            {
                lay = buttonChilds[i].GetComponent<LayoutElement>();
                lay.minWidth = width;
                lay.minHeight = height;
            }
        }
        IMCDebug.LogWarning("没有找到匹配的按钮名字 -IMCMessageBox.cs -SetButtonWidthHeight()");
    }
    #endregion //PRIVATE_METHODS
    public void UnInitMessageBox()
    {
        if (closeBack != null)
            closeBack();
        closeBack = null;
        UnInit();
    }
}
