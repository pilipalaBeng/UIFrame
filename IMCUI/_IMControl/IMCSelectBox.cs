using UnityEngine;
using System.Collections;
//using UnityEngine.UI;
using System.Collections.Generic;
//using UnityEngine.EventSystems;
using System;
using IMCUI.UI;
public class IMCSelectBox : IMCUIBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Space(20)]
    [Tooltip("selectBox组件宽度")]
    public float m_selectBoxWidth;
    [Tooltip("selectBox组件高度")]
    public float m_selectBoxHeight;
    [Tooltip("元素组件宽度")]
    public float m_elementsWidth;
    [Tooltip("元素组件高度")]
    public float m_elementsHieght;
    [Tooltip("元素字体大小")]
    public int m_elementsFontSizeNumb;
    [Tooltip("内容list")]
    public List<string> m_contentList;
    [Tooltip("对齐方式")]
    public TextAnchor m_anchor = TextAnchor.MiddleCenter;

    [HideInInspector]
    public  IMCText m_CenterObject;//中心物体
    private bool SelectBoxActivateToggle = false;//组件激活开关
    private List<IMCText> m_childs = new List<IMCText>();
    private List<string> dateElements;//需要知道长度
    private Vector3 upTargetPosition;//上方目标点
    private Vector3 downTargetPosition;//下方目标点
    private bool inertia = false;//惯性
    private Vector3 RecordMouseDragPosition;//记录鼠标每帧的位置
    private Vector3 InertiaDistancePosition;
    private bool downToggle = false;//下滑判断
    private bool upToggle = false;//上滑判断
    private Vector3 ExtrudePos;//ExtrudeObject组件位置
    private IMCText TagObject;//标记
    private bool adsorptionToggle;//吸附开关

    private int totalNumb = 0;//要生成元素的总数量
    Vector3 instantiateObjectPosition;

    void InitializeArrangementContent(float controlsWidth, float controlsHeight, float elementsWidth, float elementsHeight, int fontSizeNumb, TextAnchor anchor)//组件排列
    {
        totalNumb = Convert.ToInt32(controlsHeight / elementsHeight) + 2/* 上下各多出来两个*/;

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(controlsWidth, controlsHeight);

        GameObject m_child;
        //m_child = Instantiate(Resources.Load("IMControls/IMCText")) as GameObject;

        m_child = Instantiate(m_CenterObject.gameObject) as GameObject;
        m_child.transform.SetParent(this.transform);
        m_child.transform.localScale = Vector3.one;

        m_child.GetComponent<RectTransform>().sizeDelta = new Vector2(elementsWidth, elementsHeight);
        m_child.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, totalNumb / 2 * elementsHeight, 0);//把位置设置到最上面

        m_child.GetComponent<IMCText>().alignment = anchor;
        m_child.GetComponent<IMCText>().fontSize = fontSizeNumb;
        m_child.GetComponent<IMCText>().color = new Color(0, 0, 0, 0.3f);
        m_childs.Add(m_child.GetComponent<IMCText>());

        instantiateObjectPosition = m_child.GetComponent<RectTransform>().anchoredPosition3D;

        for (int i = 1; i <= totalNumb; i++)
        {
            instantiateObjectPosition.y = m_child.GetComponent<RectTransform>().anchoredPosition3D.y - i * m_child.GetComponent<RectTransform>().sizeDelta.y;
            SetChilds(m_child, instantiateObjectPosition);
        }
        //m_CenterObject.SetAsLastSibling();//!@#$%^
        //m_CenterObject.fontStyle = FontStyle.Bold;

        instantiateObjectPosition = m_child.GetComponent<RectTransform>().anchoredPosition3D;

        //记录上下两个极限点
        upTargetPosition = new Vector3(0, (m_childs.Count / 2 * m_childs[0].sizeDelta.y), 0);
        downTargetPosition = new Vector3(0, -(m_childs.Count / 2 * m_childs[0].sizeDelta.y), 0);

        ExtrudePos = m_CenterObject.anchoredPosition3D;//获取突出对象组件的位置

        #region 设置中心组件相关属性
        m_CenterObject.sizeDelta = m_childs[0].sizeDelta;
        m_CenterObject.fontSize = m_childs[0].fontSize;
        m_CenterObject.alignment = m_childs[0].alignment;
        #endregion
    }
    private void SetChilds(GameObject targetObject, Vector3 targetPos)
    {
        GameObject instantiateObject;
        instantiateObject = Instantiate(targetObject) as GameObject;
        instantiateObject.transform.SetParent(this.transform);
        instantiateObject.transform.localScale = Vector3.one;
        instantiateObject.GetComponent<RectTransform>().anchoredPosition3D = targetPos;

        m_childs.Add(instantiateObject.GetComponent<IMCText>());
    }

    private void InitializeElementsContent(List<string> content)//元素内容排列
    {
        try
        {
            dateElements = new List<string>();
            for (int i = 0; i < content.Count; i++)
            {
                dateElements.Add(content[i]);
            }

            #region 使最开始的数据显示第一个索引
            int dateElementsIndex = 0;
            for (int i = m_childs.Count - m_childs.Count / 2 - 1; i < m_childs.Count; i++)
            {
                m_childs[i].text = dateElements[dateElementsIndex];
                dateElementsIndex++;
            }

            dateElementsIndex = dateElements.Count - 1;
            for (int i = m_childs.Count - m_childs.Count / 2 - 1 - 1; i >= 0; i--)
            {
                m_childs[i].text = dateElements[dateElementsIndex];
                dateElementsIndex--;
            }
            #endregion

            m_CenterObject.text = dateElements[0];

            SelectBoxActivateToggle = true;
        }
        catch
        {
            for (int i = 0; i < m_childs.Count; i++)
            {
                m_childs[i].text = "";
            }
            SelectBoxActivateToggle = false;
            IMCDebug.LogWarning(this.name + "  " + "Initialize failure!");
        }
    }
    //public  override void Initialize()
    //{
    //    try
    //    {
    //        base.Initialize();
    //        Transform c = transform.FindChild("Extrude");
    //        IMCText cente = null;
    //        if (c)
    //        {
    //            cente = c.GetComponent<IMCText>();
    //        }
    //        if (cente)
    //        {
    //            m_CenterObject = cente;
    //        }
    //    }
    //    catch
    //    {
    //        SelectBoxActivateToggle = false;
    //        IMCDebug.LogWarning("SelectBox m_CenterObject is null");
    //    }
    //}
    void Awake()
    {
        Transform c = transform.FindChild("Extrude");
        IMCText cente = null;
        if (c)
        {
            cente = c.GetComponent<IMCText>();
        }
        if (cente)
        {
            m_CenterObject = cente;
        }
    }


    public void Create()
    {
        if (m_selectBoxWidth <= 0)
            m_selectBoxWidth = 349;
        if (m_selectBoxHeight <= 0)
            m_selectBoxHeight = 429;
        if (m_elementsWidth <= 0)
            m_elementsWidth = m_selectBoxWidth;
        if (m_elementsHieght <= 0)
            m_elementsHieght = 120;
        if (m_elementsFontSizeNumb <= 0)
            m_elementsFontSizeNumb = 70;
        if (m_contentList.Count == 0)
        {
            IMCDebug.LogWarning("SelectBox m_contentList.Count is 0");
            return;
        }

        Invoke("InvkeStart", 0.1f);//使用延迟0.1秒使得不影响排列
    }


    public void Create(float controlsWidth, float controlsHeight, float elementsWidth, float elementsHeight, int fontSizeNumb, List<string> content, TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        m_selectBoxWidth = controlsWidth;
        m_selectBoxHeight = controlsHeight;
        m_elementsWidth = elementsWidth;
        m_elementsHieght = elementsHeight;
        m_elementsFontSizeNumb = fontSizeNumb;
        m_anchor = anchor;
        if (content.Count != 0)
        {
            m_contentList.Clear();
            m_contentList = content;
        }
        else
        {
            IMCDebug.LogWarning("SelectBox content.Count is 0");
            return;
        }

        Invoke("InvkeStart", 0.1f);
    }
    /// <summary>
    /// 刷新数据
    /// </summary>
    public void Refresh(List<string> content)
    {
        inertia = false;
        Vector3 m_RefreshPos = Vector3.zero;
        for (int i = 0; i < m_childs.Count; i++)
        {
            m_RefreshPos.y = instantiateObjectPosition.y - i * this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
            m_childs[i].anchoredPosition3D = m_RefreshPos;
        }
        CountAdsorption();
        InitializeElementsContent(content);
    }
    private void InvkeStart()
    {
        InitializeArrangementContent(m_selectBoxWidth, m_selectBoxHeight, m_elementsWidth, m_elementsHieght, m_elementsFontSizeNumb, m_anchor);
        InitializeElementsContent(m_contentList);
    }
    /// <summary>
    /// 内部调用
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (SelectBoxActivateToggle)
        {
            inertia = false;
            adsorptionToggle = false;
            StopToggle(false, false);

            m_CenterObject.alpha = 0;
            //m_CenterObject.text = "";
            RecordMouseDragPosition.y = Input.mousePosition.y;
        }

    }
    void DisplacementPosition(IMCText child)
    {
        int m_index = 0;
        for (int i = 0; i < dateElements.Count; i++)
        {
            if (child.text == dateElements[i])
            {
                m_index = i;
            }
        }

        if (downToggle)
        {
            if (child.anchoredPosition3D.y > upTargetPosition.y)
            {
                child.anchoredPosition3D -= new Vector3(0, m_childs.Count * child.sizeDelta.y, 0);

                for (int i = 0; i < m_childs.Count; i++)//内容置换
                {
                    m_index++;
                    if (m_index > dateElements.Count - 1)
                    {
                        m_index = 0;
                    }
                }
                child.text = dateElements[m_index];//获取元素赋值
            }
        }
        else if (upToggle)
        {
            if (child.anchoredPosition3D.y < downTargetPosition.y)
            {
                child.anchoredPosition3D += new Vector3(0, m_childs.Count * child.sizeDelta.y, 0);

                for (int i = 0; i < m_childs.Count; i++)//内容置换
                {
                    m_index--;
                    if (m_index < 0)
                    {
                        m_index = dateElements.Count - 1;
                    }
                }
                child.text = dateElements[m_index];//获取元素赋值
            }
        }
    }
    /// <summary>
    /// 内部调用
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (SelectBoxActivateToggle)
        {
            for (int i = 0; i < m_childs.Count; i++)
            {
                m_childs[i].anchoredPosition3D += new Vector3(0, eventData.delta.y * 2, 0);//拖拽时的阻尼 

                if (eventData.delta.y > 0)//往上滑
                {
                    StopToggle(true, false);
                    DisplacementPosition(m_childs[i]);
                }
                else if (eventData.delta.y < 0)//往下滑
                {
                    StopToggle(false, true);
                    DisplacementPosition(m_childs[i]);
                }
            }

            RecordMouseDragPosition = Input.mousePosition;
        }
    }


    /// <summary>
    /// 内部调用
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (SelectBoxActivateToggle)
        {
            if (RecordMouseDragPosition.y != Input.mousePosition.y)
            {
                InertiaDistancePosition.y = (Input.mousePosition.y - RecordMouseDragPosition.y) * 0.1f;//惯性
                inertia = true;
            }
            else
            {
                inertia = false;
                CountAdsorption();
            }
        }
    }

    void FixedUpdate()
    {
        if (SelectBoxActivateToggle)
        {
            if (adsorptionToggle)//吸附
            {
                if (TagObject.anchoredPosition3D.y > ExtrudePos.y)
                {
                    for (int i = 0; i < m_childs.Count; i++)
                    {
                        m_childs[i].anchoredPosition3D -= new Vector3(0, 4, 0);//吸附回弹时的速度
                    }
                    if (TagObject.anchoredPosition3D.y <= ExtrudePos.y)
                    {
                        m_CenterObject.alpha = 1;
                        m_CenterObject.text = TagObject.text;
                        adsorptionToggle = false;
                    }
                }
                else if (TagObject.anchoredPosition3D.y < ExtrudePos.y)
                {
                    for (int i = 0; i < m_childs.Count; i++)
                    {
                        m_childs[i].anchoredPosition3D += new Vector3(0, 4, 0);
                    }
                    if (TagObject.anchoredPosition3D.y >= ExtrudePos.y)
                    {
                        m_CenterObject.alpha = 1;
                        m_CenterObject.text = TagObject.text;
                        adsorptionToggle = false;
                    }
                }
                else
                {
                    m_CenterObject.alpha = 1;
                    m_CenterObject.text = TagObject.text;
                    adsorptionToggle = false;
                }
            }

            if (inertia)//惯性
            {
                for (int i = 0; i < m_childs.Count; i++)
                {
                    m_childs[i].transform.localPosition += InertiaDistancePosition;

                    if (InertiaDistancePosition.y > 1)
                    {
                        StopToggle(true, false);
                    }
                    else if (InertiaDistancePosition.y < -1)
                    {
                        StopToggle(false, true);
                    }

                    DisplacementPosition(m_childs[i]);
                }
                if (InertiaDistancePosition.y > 1)
                {
                    //InertiaDistancePosition.y -= 1f;
                    InertiaDistancePosition.y -= 2f;
                    if (InertiaDistancePosition.y <= 0)
                    {
                        CountAdsorption();
                        inertia = false;
                    }
                }
                else if (InertiaDistancePosition.y < -1)
                {
                    //InertiaDistancePosition.y +=1f;
                    InertiaDistancePosition.y += 2f;
                    if (InertiaDistancePosition.y >= 0)
                    {
                        CountAdsorption();
                        inertia = false;
                    }
                }
                else
                {
                    CountAdsorption();
                    inertia = false;
                }
            }
        }
    }

    private void StopToggle(bool down, bool up)
    {
        downToggle = down;
        upToggle = up;
    }

    private void CountAdsorption()//吸附
    {
        for (int i = 0; i < m_childs.Count; i++)
        {
            if (Vector3.Distance(m_childs[i].anchoredPosition3D, ExtrudePos) <= m_CenterObject.sizeDelta.y / 1.5f)
            {
                TagObject = m_childs[i];
                adsorptionToggle = true;
                return;
            }
        }
    }
    /// <summary>
    /// 获取当前吸附开关状态，如果没有动作，返回flase， 反之返回true
    /// </summary>
    public bool GetSelectBoxState()
    {
        return adsorptionToggle;
    }

    /// <summary>
    /// SelectBox组件当前索引值
    /// </summary>
    public string SelectValue
    {
        get { return m_CenterObject.text; }
        set { m_CenterObject.text = value; }
    }
    public Color color
    {
        get { return GetComponent<IMCImage>().color; }
        set { GetComponent<IMCImage>().color = value; }
    }
    public Sprite sprite
    {
        get { return GetComponent<IMCImage>().sprite; }
        set { GetComponent<IMCImage>().sprite = value; }
    }
}