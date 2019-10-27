using UnityEngine;
using System.Collections;

//using UnityEngine.UI;
using System.Collections.Generic;
using IMCUI.UI;

/// <summary>
/// 呼出手势：同一位置双击屏幕，随后点击屏幕从右往左滑动一段距离，然后再某一位置连续点击五下 AA←AAAAA
/// </summary>
public class IMCDebug : IMCForm
{
    public static bool isEnable = false;
    //public List<IMCDebugCreateData> debugList = new List<IMCDebugCreateData>();
    private static IMCDebug instance = null;

    public static IMCDebug Instance
    {
        get
        {
            if (instance == null)
            {
                instance = IMCUIResouceManager.LoadControl("IMCDebug", IMCUIManager.Instance.CanvasList[IMCUIManager.Instance.CanvasList.Count - 1].transform).GetComponent<IMCDebug>();
                DontDestroyOnLoad(instance.gameObject);
                for (int i = 0; i < instance.debugDatas.Count; i++)
                    instance.debugDatas[i].Init();
            }
            return instance;
        }
    }

    private bool gesturesPasswordA = true;
//手势密码
    private bool gesturesPasswordB = false;
    private bool gesturesPasswordC = false;
    private float mouseDownPosX;
    private float mouseUpPosX;

    protected override void Start()
    {
        Create();
    }

    private IEnumerator Gestures(float time)//手势
    {
        float curTime = 0;
        float itime = time;
        float progres = 0;
        int downNumb = 0;
        while (progres <= 1)
        {
            curTime += Time.deltaTime;
            progres = curTime / itime;
            if (Input.GetMouseButtonDown(0))
            {
                downNumb++;
                if (downNumb >= 5)
                {
                    openSelf = true;
                }
            }
            yield return null;
        }
        if (downNumb < 5)
        {
            gesturesPasswordA = true;
        }
    }

    /// <summary>
    /// 创建IMCDebug组件
    /// </summary>
    private void Create()
    {
        this.transform.SetParent(IMCUIManager.Instance.CanvasList[0].transform);
        this.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        this.transform.localScale = Vector3.one;
        CanvasGroupSwitch(false);
    }

    void Update()
    {
        if (gesturesPasswordB)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosX = Input.mousePosition.x;
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseUpPosX = Input.mousePosition.x;
                gesturesPasswordC = true;
            }
            if (gesturesPasswordC)
            {
                gesturesPasswordC = false;
                if (mouseDownPosX - mouseUpPosX > 300)
                {
                    gesturesPasswordB = false;
                    StopAllCoroutines();
                    StartCoroutine(Gestures(1.5f));
                }
                else
                {
                    Invoke("InvokegesturesPassword", 1f);
                }
                mouseDownPosX = 0;
                mouseUpPosX = 0;
            }
        }
        if (openSelf)
        {
            if (openSelfSwitch)
            {
                Show();
                openSelfSwitch = false;
                openSelfSwitch_b = true;

                //geng 2018年1月17日13:48:48 使用静态变量控制能否开启
                if (!isEnable)
                    Close();
            }
        }
        else
        {
            if (openSelfSwitch_b)
            {
                Close();
                openSelfSwitch = true;
                openSelfSwitch_b = false;
            }
        }
    }

    private bool openSelfSwitch = false;

    private void InvokegesturesPassword()
    {
        gesturesPasswordA = true;
        gesturesPasswordB = false;
    }

    Event e;
    private bool openSelf = false;
    private bool openSelfSwitch_b = true;

    void OnGUI()
    {
        e = Event.current;
        if (e.clickCount == 2)
        {
            if (gesturesPasswordA)
            {
                gesturesPasswordA = false;
                gesturesPasswordB = true;
            }
        }
    }

    public List<IMCDebugCreateData> debugDatas;
//0 debug 1 error 2 warning
    private int debugSort = 0;
    private static List<string> logContentList = new List<string>();
    private static List<string> logErrorContentList = new List<string>();
    private static List<string> logWarningContentList = new List<string>();

    public static void Log(string content, GameObject go = null)
    {
        logContentList.Add(content);
#if UNITY_EDITOR
        Debug.Log(content + "\n", go);
#endif
        ShowDebugLog(0, logContentList[logContentList.Count - 1], go);
    }

    public static void LogError(string content, GameObject go = null)
    {
        logErrorContentList.Add(content);
#if UNITY_EDITOR
        Debug.LogError(content + "\n", go);
#endif
        ShowDebugLog(1, logErrorContentList[logErrorContentList.Count - 1], go);
    }

    public static void LogWarning(string content, GameObject go = null)
    {
        logWarningContentList.Add(content);
#if UNITY_EDITOR
        Debug.LogWarning(content + "\n", go);
#endif
        ShowDebugLog(2, logWarningContentList[logWarningContentList.Count - 1], go);
    }

    private static void ShowDebugLog(int index, string content, GameObject go = null)
    {
        if (index > 2)
            return;
        if (Instance != null && isShow)
        {
            Instance.debugDatas[index].AddContent(TagDebugSort() + content + "\n");
            Instance.debugSort++;
        }
    }

    private void RefreshData()
    {
        if (logContentList.Count > 0)
            for (int i = 0; i < logContentList.Count; i++)
            {
                ShowDebugLog(0, logContentList[i]);
            }
        if (logErrorContentList.Count > 0)
            for (int i = 0; i < logErrorContentList.Count; i++)
            {
                ShowDebugLog(1, logErrorContentList[i]);
            }
        if (logWarningContentList.Count > 0)
            for (int i = 0; i < logWarningContentList.Count; i++)
            {
                ShowDebugLog(2, logWarningContentList[i]);
            }
    }

    private static string TagDebugSort()
    {
        return "<" + Instance.debugSort + ">";
    }

    private static bool isShow = false;

    public override void Show()
    {
        base.Show();
        SetAsLastSibling();
        isShow = true;
        RefreshData();
    }

    public override void Close()
    {
        base.Close();
        SetAsFirstSibling();
        for (int i = 0; i < debugDatas.Count; i++)
            debugDatas[i].ClearData();
        isShow = false;
        openSelf = false;
        debugSort = 0;
    }

    public void Clear()
    {
        CurrentComponent().ClearData();
        ClearAllMesssages();
    }
    //geng 2018年1月17日15:13:41
    public void ClearAllMesssages()
    {
        if (debugDatas == null)
            return;
        logContentList.Clear();
        logErrorContentList.Clear();
        logWarningContentList.Clear();
    }

    public void AddFontSize()
    {
        CurrentComponent().AddFontSize();
    }

    public void ReduceFontSize()
    {
        CurrentComponent().ReduceFontSize();
    }

    public IMCTabView tabView;

    private IMCDebugCreateData CurrentComponent()
    {
        for (int i = 0; i < tabView.toggles.Count; i++)
        {
            if (tabView.toggles[i].toggle.isOn)
                return debugDatas[i];
        }
        return debugDatas[0];
    }

}