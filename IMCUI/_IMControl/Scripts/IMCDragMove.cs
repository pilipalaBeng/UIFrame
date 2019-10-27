using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
using System.Collections;
using IMCUI.UI;
public class IMCDragMove : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Space(10)]
    [Tooltip("被移动物体")]
    public RectTransform panelRectTransform;    // 被移动物体
    [Tooltip("范围限制")]
    public bool rangeLimit = true;    // 范围限制
    private Vector2 originalLocalPointerPosition;    // 原始光标位置
    private Vector3 originalPanelLocalPosition;    //  被拖拽物体的原始位置
    private RectTransform parentRectTransform;// 面板组件

    void Start()
    {
        if (panelRectTransform == null)//如果没有给被拖拽物体变量赋值的话，默认被拖拽的是自身
        {
            panelRectTransform = this.transform as RectTransform;//被移动物体等于其父物体
        }
        if (panelRectTransform.parent != null)
            parentRectTransform = panelRectTransform.parent as RectTransform;//面板组件等于被移动物体的父物体
        else
            parentRectTransform = IMCUIManager.Instance.CanvasList[0].GetComponent<RectTransform>() ;
    }
    public void OnPointerDown(PointerEventData data)
    {
        originalPanelLocalPosition = panelRectTransform.localPosition;// 被移动物体的原始位置 等于被移动物体的localpos，基于父物体的位置
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
        //得到 被面板组件范围内光标的点pos，并返回给 光标原始位置 v2变量中
    }

    public void OnDrag(PointerEventData data)
    {
        if (panelRectTransform == null || parentRectTransform == null)
            return;

        Vector2 localPointerPosition;//当前的光标位置

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition))
        {
            // 实时获取 基于面板组件位置并返回给 当前的光标位置 v2变量中
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;//抵消原始v3变量  等于 当前的光标位置v2 减去原始光标位置v2
            panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
            //被移动物体的localpos等于 被移动物体的原始位置+ 抵消原始
        }
        if (rangeLimit)
        {
            ClampToWindow();
        }
    }

    private void ClampToWindow()
    {//移动范围限制的一个函数,立体UI时才会用到，平面UI不用管这个rangeLimit为flase就行

        Vector3 pos = panelRectTransform.localPosition;//pos等于被移动物体的实时localPos

        Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;//最小pos 等于 面板组件的矩形最小值 减去 被移动物体的矩形最小值
        Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;//最大pos 等于 面板组件的矩形最大值 减去 被移动物体的矩形最大值

        pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);//posX轴数值  实时获取   限制被移动物体的localposX轴在最小posX轴和最大posX轴
        pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);//posY轴数值  实时获取   限制被移动物体的localposY轴在最小posY轴和最大posY轴
        panelRectTransform.localPosition = pos;//被移动物体的localpos等于  pos v3变量

    }

}
