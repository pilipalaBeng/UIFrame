using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using IMCUI.UI;
/// <summary>
/// 文本控件，支持超链接、图片
/// </summary>
//[AddComponentMenu("UI/LinkImageText", 10)]
namespace IMCUI.UI
{
    public class IMCHyperlinksLabel : IMCText /*, IPointerClickHandler*/, IPointerDownHandler, IPointerClickHandler
    {
        /// <summary>
        /// 解析完最终的文本
        /// </summary>
        private string m_OutputText;

        /// <summary>
        /// 图片池
        /// </summary>
        protected readonly List<IMCImage> m_ImagesPool = new List<IMCImage>();

        /// <summary>
        /// 图片的最后一个顶点的索引
        /// </summary>
        private readonly List<int> m_ImagesVertexIndex = new List<int>();


        /// <summary>
        /// 超链接信息列表
        /// </summary>
        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

        /// <summary>
        /// 文本构造器
        /// </summary>
        protected static readonly StringBuilder s_TextBuilder = new StringBuilder();

        [Serializable]
        public class HrefClickEvent : UnityEvent<string> { }

        [SerializeField]
        private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

        /// <summary>
        /// 超链接点击事件
        /// </summary>
        public HrefClickEvent onHrefClick
        {
            get { return m_OnHrefClick; }
            set { m_OnHrefClick = value; }
        }
        [SerializeField]
        private int m_hyperlinksLaberFontSize = 45;
        [SerializeField]
        private string m_hyperlinksLaberColor = "#28A3C7FF";
        public int hyperlinksLaberFontSize
        {
            get
            {
                return m_hyperlinksLaberFontSize;
            }

            set
            {
                m_hyperlinksLaberFontSize = value;
            }
        }

        public Color hyperlinksLaberColor
        {
            get
            {
                return ColorConversionToColor(m_hyperlinksLaberColor);
            }

            set
            {
                m_hyperlinksLaberColor = ColorConversionTo16String(value);
            }
        }

        Vector2 size;
        protected override void Start()
        {
            base.Start();
            size = rectTransform.sizeDelta;
            onHrefClick.AddListener(OpenURL);
        }

        /// <summary>
        /// 正则取出所需要的属性
        /// </summary>
        private static readonly Regex s_ImageRegex =
            new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);

        /// <summary>
        /// 超链接正则
        /// </summary>
        private static readonly Regex s_HrefRegex =
            new Regex(@"<ar href=([^>\n\s]+)>(.*?)(</ar>)", RegexOptions.Singleline);

        /// <summary>
        /// 加载精灵图片方法
        /// </summary>
        public static Func<string, Sprite> funLoadSprite;

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            UpdateQuadImage();
        }

        protected void UpdateQuadImage()
        {
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab)
            {
                return;
            }
#endif
            m_OutputText = GetOutputText(text);
            m_ImagesVertexIndex.Clear();
            foreach (Match match in s_ImageRegex.Matches(m_OutputText))
            {
                var picIndex = match.Index;
                var endIndex = picIndex * 4 + 3;
                m_ImagesVertexIndex.Add(endIndex);

                m_ImagesPool.RemoveAll(image => image == null);
                if (m_ImagesPool.Count == 0)
                {
                    GetComponentsInChildren<IMCImage>(m_ImagesPool);
                }
                if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
                {
                    var resources = new DefaultControls.Resources();
                    var go = DefaultControls.CreateImage(resources);
                    go.layer = gameObject.layer;
                    var rt = go.transform as RectTransform;
                    if (rt)
                    {
                        rt.SetParent(rectTransform);
                        rt.localPosition = Vector3.zero;
                        rt.localRotation = Quaternion.identity;
                        rt.localScale = Vector3.one;
                    }
                    m_ImagesPool.Add(go.GetComponent<IMCImage>());
                }

                var spriteName = match.Groups[1].Value;
                var size = float.Parse(match.Groups[2].Value);
                var img = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
                if (img.sprite == null || img.sprite.name != spriteName)
                {
                    img.sprite = funLoadSprite != null ? funLoadSprite(spriteName) :
                        Resources.Load<Sprite>(spriteName);
                }
                img.rectTransform.sizeDelta = new Vector2(size, size);
                img.enabled = true;
            }

            for (var i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
            {
                if (m_ImagesPool[i])
                {
                    m_ImagesPool[i].enabled = false;
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            var orignText = m_Text;
            m_Text = m_OutputText;
            base.OnPopulateMesh(toFill);
            m_Text = orignText;

            UIVertex vert = new UIVertex();
            //for (var i = 0; i < m_ImagesVertexIndex.Count; i++)
            //{
            //    var endIndex = m_ImagesVertexIndex[i];
            //    var rt = m_ImagesPool[i].rectTransform;
            //    var size = rt.sizeDelta;
            //    if (endIndex < toFill.currentVertCount)
            //    {
            //        toFill.PopulateUIVertex(ref vert, endIndex);
            //        rt.anchoredPosition = new Vector2(vert.position.x + size.x / 2, vert.position.y + size.y / 2);

            //        // 抹掉左下角的小黑点
            //        toFill.PopulateUIVertex(ref vert, endIndex - 3);
            //        var pos = vert.position;
            //        for (int j = endIndex, m = endIndex - 3; j > m; j--)
            //        {
            //            toFill.PopulateUIVertex(ref vert, endIndex);
            //            vert.position = pos;
            //            toFill.SetUIVertex(vert, j);
            //        }
            //    }
            //}

            //if (m_ImagesVertexIndex.Count != 0)
            //{
            //    m_ImagesVertexIndex.Clear();
            //}

            // 处理超链接包围框
            foreach (var hrefInfo in m_HrefInfos)
            {
                hrefInfo.boxes.Clear();
                if (hrefInfo.startIndex >= toFill.currentVertCount)
                {
                    continue;
                }

                // 将超链接里面的文本顶点索引坐标加入到包围框
                toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);
                for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);
                    pos = vert.position;
                    if (pos.x < bounds.min.x) // 换行重新添加包围框
                    {
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(pos); // 扩展包围框
                    }
                }
                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
            }
        }

        /// <summary>
        /// 获取超链接解析后的最后输出文本
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOutputText(string outputText)
        {
            s_TextBuilder.Length = 0;
            m_HrefInfos.Clear();
            var indexText = 0;
            foreach (Match match in s_HrefRegex.Matches(outputText))
            {
                s_TextBuilder.Append(outputText.Substring(indexText, match.Index - indexText));
                s_TextBuilder.Append("<color=" + m_hyperlinksLaberColor + ">");  // 超链接颜色
                s_TextBuilder.Append("<size=" + m_hyperlinksLaberFontSize + "> ");

                var group = match.Groups[1];
                var hrefInfo = new HrefInfo
                {
                    startIndex = s_TextBuilder.Length * 4, // 超链接里的文本起始顶点索引
                    endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                    name = group.Value
                };
                m_HrefInfos.Add(hrefInfo);

                s_TextBuilder.Append(match.Groups[2].Value);
                s_TextBuilder.Append("</size>");
                s_TextBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }
            s_TextBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
            return s_TextBuilder.ToString();
        }

        private void OpenURL(string hrefName)
        {
            Debug.Log("点击了 " + hrefName);
            try
            {
                Application.OpenURL(hrefName);
            }
            catch (System.Exception)
            {
                IMCDebug.LogWarning("网址有误!");
            }
        }
        /// <summary>
        /// 超链接信息类
        /// </summary>
        private class HrefInfo
        {
            public int startIndex;

            public int endIndex;

            public string name;

            public readonly List<Rect> boxes = new List<Rect>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Vector2 lp;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            //foreach (var hrefInfo in m_HrefInfos)
            //{
            //    var boxes = hrefInfo.boxes;
            //    for (var i = 0; i < boxes.Count; ++i)
            //    {
            //        if (boxes[i].Contains(lp))
            //        {
            //            m_OnHrefClick.Invoke(hrefInfo.name);
            //            return;
            //        }
            //    }
            //}
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in m_HrefInfos)
            {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        m_OnHrefClick.Invoke(hrefInfo.name);
                        return;
                    }
                }
            }
        }
        public void SetHyperlinksContent(string url, string content)
        {
            this.text = "< ar href = " + url + ">" + content + "</ar>";
        }
        public void SetHyperlinksContent(string url, string content, Color contentColor, int contentFontSize)
        {
            SetHyperlinksContent(url, content);
            m_hyperlinksLaberFontSize = contentFontSize;
            m_hyperlinksLaberColor = ColorConversionTo16String(contentColor);
        }
        /// <param name="contentColor">16位字符串</param>
        public void SetHyperlinksContent(string url, string content, string contentColor, int contentFontSize)
        {
            SetHyperlinksContent(url, content);
            m_hyperlinksLaberFontSize = contentFontSize;
            m_hyperlinksLaberColor = contentColor;
        }
        private string ColorConversionTo16String(Color color)
        {
            string colorStr = "";
            float colorR = color.r, colorG = color.g, colorB = color.b, colorA = color.a;

            colorR *= 255;
            colorG *= 255;
            colorB *= 255;
            colorA *= 255;

            string colorRStr = "", colorGStr = "", colorBStr = "", colorAStr = "";

            colorRStr = Convert.ToString(Convert.ToInt32(colorR), 16);
            colorGStr = Convert.ToString(Convert.ToInt32(colorG), 16);
            colorBStr = Convert.ToString(Convert.ToInt32(colorB), 16);
            colorAStr = Convert.ToString(Convert.ToInt32(colorA), 16);

            ExhaustionTraverse(ref colorRStr);
            ExhaustionTraverse(ref colorGStr);
            ExhaustionTraverse(ref colorBStr);
            ExhaustionTraverse(ref colorAStr);

            return colorStr = "#" + colorRStr + colorGStr + colorBStr + colorAStr;
        }
        private string[] exhaustionCondition = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private void ExhaustionTraverse(ref string colorStr)
        {
            for (int i = 0; i < exhaustionCondition.Length; i++)
            {
                if (colorStr == exhaustionCondition[i])
                {
                    colorStr = "0" + colorStr;
                    break;
                }
            }
        }
        private Color ColorConversionToColor(string color)
        {
            Color col = Color.white;

            string colorRStr, colorGStr, colorBStr, colorAStr;
            if (color.Contains("#"))
                color = color.Remove(0, 1);
            colorRStr = color.Substring(0, 2);
            colorGStr = color.Substring(2, 2);
            colorBStr = color.Substring(4, 2);
            colorAStr = color.Substring(6, 2);

            int colorR, colorG, colorB, colorA;

            colorR = Convert.ToInt32(colorRStr, 16);
            colorG = Convert.ToInt32(colorGStr, 16);
            colorB = Convert.ToInt32(colorBStr, 16);
            colorA = Convert.ToInt32(colorAStr, 16);

            col = new Color((float)colorR / 255, (float)colorG / 255, (float)colorB / 255, (float)colorA / 255);
            return col;
        }
    }
}