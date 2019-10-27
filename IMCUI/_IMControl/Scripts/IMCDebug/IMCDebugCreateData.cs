using UnityEngine;
using System.Collections;
using IMCUI.UI;
using System.Collections.Generic;

public class IMCDebugCreateData : MonoBehaviour
{
    public IMCText prefabText;
    private List<IMCText> texts = null;
    //private List<string> strs = null;
    private int fontSize = 0;
    public IMCScrollRect list;
    private bool isInit = true;
    public void Init()
    {
        if (!isInit)
            return;
        texts = new List<IMCText>();

        texts.Add(CreateTextComponent());
        fontSize = prefabText.fontSize;
        isInit = false;
    }
    /// <summary>
    /// 放大字号
    /// </summary>
    /// <param name="fontNumb"></param>
    public void AddFontSize()
    {
        fontSize++;
        SetFontSize(fontSize);
    }
    /// <summary>
    /// 缩小字号
    /// </summary>
    /// <param name="fontNumb"></param>
    public void ReduceFontSize()
    {
        fontSize--;
        fontSize = fontSize <= 0 ? 1 : fontSize;

        SetFontSize(fontSize);
    }
    private void SetFontSize(int fontNumb)//设置字体大小
    {
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].fontSize = fontNumb;
        }
    }
    private int addTextLength = 0, addTextLength2 = 0;
    public void AddContent(string content)
    {
        if (content.Length >= 16000)
        {
            addTextLength = content.Length / 16000;
            if (content.Length % 16000 > 0)
                addTextLength2 = 1;
            else
                addTextLength2 = 0;
            for (int i = 0; i < addTextLength; i++)
            {
                texts.Add(CreateTextComponent());
                texts[texts.Count - 1].text += content.Substring(0, 16000);
                content = content.Remove(0, 16000);
            }
            if (addTextLength2 == 1)
            {
                texts.Add(CreateTextComponent());
                texts[texts.Count - 1].text += content;
            }
        }
        else
        {
            if (texts[texts.Count - 1].text.Length + content.Length >= 16000)
                texts.Add(CreateTextComponent());
            texts[texts.Count - 1].text += content;
        }
    }
    public void ClearData()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            Destroy(texts[i].gameObject);
        }
        texts.Clear();

        texts.Add(CreateTextComponent());
    }
    private bool CountTextContentLength(string textContent)
    {
        return textContent.Length > 16250 ? false : true;
    }
    private IMCText CreateTextComponent()
    {
        if (prefabText)
        {
            IMCText tempText = Instantiate(prefabText) as IMCText;
            tempText.transform.SetParent(list.content);
            tempText.transform.localScale = Vector3.one;
            return tempText;
        }
        Debug.Log("Don't have prefabText component!");
        return null;
    }
    #region load
    //private List<string> contentArr = new List<string>();
    //private List<Text> textArr = new List<Text>();
    //public Text textComponent;
    //public ScrollRect scrollRect;
    //public RectTransform contentComponent;
    //private int fontSize;//字体大小

    //void Start()
    //{
    //    Initialize();
    //}
    //private void Initialize()
    //{
    //    //IMCDebug.Instance.debugList.Add(this);
    //    fontSize = textComponent.fontSize;
    //    //textArr.Add(textComponent);
    //}
    ///// <summary>
    ///// 输出
    ///// </summary>
    //public void AddContent(string content, bool openSwitch)
    //{
    //    //JumpTextComponentNumber();
    //    JudgeAddDataText(content);
    //    if (openSwitch)
    //        Show();
    //    else
    //        Close();
    //}
    ///// <summary>
    ///// 放大
    ///// </summary>
    //public void Amplification(int increasing)
    //{
    //    SetFontSize(fontSize += increasing);
    //}
    ///// <summary>
    ///// 缩小
    ///// </summary>
    //public void Shrink(int diminishing)
    //{
    //    SetFontSize(fontSize -= diminishing);
    //}
    //private void JudgeAddDataText(string content)//判断添加的text
    //{
    //    if (textArr.Count == 0)
    //    {
    //        InstantiateText();
    //    }
    //    if (contentArr.Count == 0)
    //    {
    //        contentArr.Add(content);
    //    }
    //    else
    //        if (contentArr[contentArr.Count - 1].Length + content.Length > 16250)
    //        {
    //            contentArr.Add(content);//新建一个元素
    //            InstantiateText();//新建一个text
    //        }
    //        else
    //        {
    //            contentArr[contentArr.Count - 1] += "\n" + content;
    //        }
    //}
    //public void Show()
    //{
    //    if (textArr.Count != 0 && contentArr.Count != 0)
    //        for (int i = 0; i < contentArr.Count; i++)
    //        {
    //            textArr[i].text = contentArr[i];
    //        }
    //    scrollRect.verticalScrollbar.value = 0;
    //}
    //public void Close()
    //{
    //    for (int i = 0; i < textArr.Count; i++)
    //    {
    //        textArr[i].text = "";
    //    }
    //}
    //private void InstantiateText()
    //{
    //    Text tempText = Instantiate(textComponent) as Text;
    //    tempText.transform.SetParent(contentComponent);
    //    tempText.transform.localScale = Vector3.one;
    //    tempText.text = "";
    //    tempText.fontSize = fontSize;
    //    textArr.Add(tempText);
    //}
    //private void JumpTextComponentNumber()
    //{
    //    if (contentArr.Count > 5)
    //    {
    //        GameObject go = textArr[0].gameObject;
    //        textArr.Remove(textArr[0]);
    //        Destroy(go);
    //    }
    //}
    //public void ClearData()//清理数据
    //{
    //    for (int i = 0; i < textArr.Count; i++)
    //    {
    //        Destroy(textArr[i].gameObject);
    //    }
    //    textArr.Clear();
    //    contentArr.Clear();
    //    //for (int i = textArr.Count - 1; i >= 1; i--)
    //    //{
    //    //    GameObject go = textArr[i].gameObject;
    //    //    textArr.Remove(textArr[i]);
    //    //    Destroy(go);
    //    //}
    //}

    //private void SetFontSize(int fonSize)//设置字体大小
    //{
    //    for (int i = 0; i < textArr.Count; i++)
    //    {
    //        textArr[i].fontSize = fonSize;
    //    }
    //}
    #endregion 
}
