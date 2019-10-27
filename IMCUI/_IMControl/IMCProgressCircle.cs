using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
using IMCUI.UI;
[ExecuteInEditMode]
public class IMCProgressCircle : IMCUIBehaviour
{
    [Space(20)]
    [Tooltip("fill组件")]
    public IMCImage imageCom;//填充物组件
    [Tooltip("标签")]
    public IMCText label;//标签
    [Tooltip("进度类型：数值；百分比")]
    public LabelType labelType = LabelType.Percent;//进度类型：数值；百分比
    //[Tooltip("小数位数"),Range(0,5)]
    private  int decimalsCount =1;//小数位数
    //[Tooltip("字符后缀")]
    private  string labelSuffix = "%";//字符后缀
    [Tooltip("进度值")]
    public float m_Value = 0f;//进度值
    [Tooltip("fill组件颜色")]
    public Color m_FillColor = Color.white;// fill组件颜色
    [Tooltip("value颜色")]
    public Color m_ValueColor = Color.black;//value颜色
    public enum LabelType
    {
        Value,
        Percent
    }
    protected override void Awake()
    {
        base.Awake();
         if (imageCom)
         {
             imageCom.type = IMCImage.Type.Filled;
             imageCom.fillAmount = 0;
         }
    }
    void Update()
    {
        if (imageCom)
        {
            imageCom.color = m_FillColor;
            m_Value = Mathf.Clamp01(m_Value);
            if (imageCom.fillAmount != m_Value)
                imageCom.fillAmount = m_Value;
        }
        SetLabelText(m_Value);
        ValueColor = m_ValueColor;
    }

    public void Create(float value = 0)
    {
        ProgressBarNumb = value;
    }

    public void Create(Color fillColor, float value = 0)
    {
        ProgressBarNumb = value;
        FillColor = fillColor;
    }

    //protected override void Initialize()
    //{
    //    base.Initialize();
    //    if (imageCom)
    //    {
    //        imageCom.type = IMCImage.Type.Filled;
    //        imageCom.fillAmount = 0;
    //    }
    //}

    public float ProgressBarNumb
    {
        get { return m_Value; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            m_Value = value;
        }
    }

    public Color FillColor
    {
        get { return imageCom.color; }
        set { imageCom.color = value; }
    }
 
    public Color ValueColor
    {
        set
        {
            if (label)
                label.color = value;
        }
    }

    private void SetLabelText(float value)
    {
        if (label != null)
        {
            value = Mathf.Clamp01(value);
            string str;
            if (decimalsCount <= 0)
                decimalsCount = 0;
            //string format = "F" + decimalsCount.ToString();
            switch (labelType)
            {
                case LabelType.Value:

                    str = value.ToString("F" + decimalsCount);
                    Debug.Log(str);
                    label.text = str;
                    break;
                case LabelType.Percent:
                    float percent = value * 100;
                    str = percent.ToString("F"+"0") + labelSuffix;
                    label.text = str;
                    break;
            }
        }
    }
}
