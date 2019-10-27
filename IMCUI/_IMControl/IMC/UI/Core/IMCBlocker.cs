using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine.SceneManagement;

namespace IMCUI.UI
{
    public class IMCBlocker : IMCUIBehaviour, IPointerClickHandler
    {
        private static IMCBlocker instance;
        public static IMCBlocker Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateBlockerObject();
                return IMCBlocker.instance;
            }
            set { IMCBlocker.instance = value; }
        }
        private  bool is_blockerPointerClick = false;//2017年9月1日10:05:18 添加变量
        private Color transparentColor = new Color(0, 0, 0, 0);
        private Color grayBackgroundColor = new Color(0, 0, 0, 0.58f);
        private static IMCImage blockerImage;
        private Camera RenderCamera;
        private RenderTexture renderTexture;
        private List<BlockerStruct> m_targets;
        private List<BlockerStruct> targets
        {
            get
            {
                if (m_targets == null)
                    m_targets = new List<BlockerStruct>(10);
                return m_targets;
            }
            set { m_targets = value; }
        }
        public bool Is_blockerPointerClick { get { return is_blockerPointerClick; } }
        public enum ShowStyleEnum
        {
            Transparent,
            GrayBackground,
            GroundGlass,
        }
        private struct BlockerStruct
        {
            public List<IMCUIBehaviour> targetUis;
            public ShowStyleEnum showStyle;
            public UnityAction callback;
            public Sprite sprite;
            public bool isDestroy;
            public Transform parent;
        }
        public IMCBlocker Create(List<IMCUIBehaviour> targetUis, ShowStyleEnum showStyle, UnityAction callback = null, bool isDestroy = true, Transform parent = null)
        {
            BlockerStruct blockerStruct = new BlockerStruct();
            blockerStruct.targetUis = targetUis;
            blockerStruct.showStyle = showStyle;
            blockerStruct.callback = callback;
            blockerStruct.sprite = showStyle == ShowStyleEnum.GroundGlass ? Screenshot() : null;
            blockerStruct.isDestroy = isDestroy;
            blockerStruct.parent = parent;

            targets.Add(blockerStruct);
            Instance.transform.SetParent(parent == null ? targets[targets.Count - 1].targetUis[0].transform.parent : parent);
            Instance.anchorMin = Vector2.zero;
            Instance.anchorMax = Vector2.one;
            Instance.anchoredPosition3D = Vector3.zero;
            Instance.sizeDelta = Vector2.zero;
            Instance.transform.localScale = Vector3.one;

            BlockerRefresh();

            return Instance.GetComponent<IMCBlocker>();
        }

        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCBlocker;
            m_containerType = ContainerType.Control;

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (targets.Count > 0)
                targets.Clear();
        }

        private void RemoveArray()
        {
            is_blockerPointerClick = false;
            if (targets.Count > 0)
                targets.RemoveAt(targets.Count - 1);
            EliminateLayer();
        }

        private void BlockerRefresh()
        {
            switch (targets[targets.Count - 1].showStyle)
            {
                case ShowStyleEnum.Transparent:
                    blockerImage.color = transparentColor;
                    blockerImage.sprite = null;
                    break;
                case ShowStyleEnum.GrayBackground:
                    blockerImage.color = grayBackgroundColor;
                    blockerImage.sprite = null;
                    break;
                case ShowStyleEnum.GroundGlass:
                    blockerImage.color = Color.white;
                    blockerImage.sprite = targets[targets.Count - 1].sprite;
                    break;
            }
            Instance.SetAsLastSibling();
            for (int i = 0; i < targets[targets.Count - 1].targetUis.Count; i++)
                targets[targets.Count - 1].targetUis[i].SetAsLastSibling();
        }

        private void EliminateLayer()
        {
            if (targets.Count > 0)
            {
                BlockerRefresh();
            }
            else
                Destroy(Instance.gameObject);//2017年9月1日09:33:36  
        }

        public void RemoveStack(IMCUIBehaviour removeTarget)
        {
            if (targets.Count > 0)
                for (int i = 0; i < targets.Count; i++)
                    if (targets[i].targetUis.Contains(removeTarget))
                        targets.RemoveAt(i);
            EliminateLayer();
        }

        private static IMCBlocker CreateBlockerObject()
        {
            GameObject go = new GameObject("IMC Blocker");
            IMCBlocker blocker = go.AddComponent<IMCBlocker>();
            go.AddComponent<RectTransform>();
            blockerImage = go.AddComponent<IMCImage>();

            return blocker;
        }

        public Sprite Screenshot()
        {
            GameObject RCObj = new GameObject("RenderCamera");
            RenderCamera = RCObj.AddComponent<Camera>();
            RenderCamera.transform.parent = Camera.main.gameObject.transform.parent;
            RenderCamera.hideFlags = HideFlags.None;
            if (!renderTexture)
                renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            RenderCamera.CopyFrom(Camera.main);
            for (int i = 0; i < Camera.main.gameObject.transform.childCount; i++)
                Camera.main.gameObject.transform.GetChild(i).gameObject.layer = 31;
            RenderCamera.depth = 0;
            RenderCamera.cullingMask = 1 << 31;
            RenderCamera.targetTexture = renderTexture;
            RenderCamera.Render();
            Rect rect = new Rect(0, 0, RenderCamera.targetTexture.width, RenderCamera.targetTexture.height);
            //Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            Texture2D tempTexture2D = new Texture2D(RenderCamera.targetTexture.width, RenderCamera.targetTexture.height, TextureFormat.RGB24, false);
            tempTexture2D.ReadPixels(rect, 0, 0);
            tempTexture2D.Apply();
            Sprite sprite = Sprite.Create(tempTexture2D, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            return sprite;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (targets[targets.Count - 1].callback != null)
                targets[targets.Count - 1].callback();

            for (int i = 0; i < targets[targets.Count - 1].targetUis.Count; i++)
            {
                if (targets[targets.Count - 1].isDestroy)
                {
                    is_blockerPointerClick = true;
                    targets[targets.Count - 1].targetUis[i].UnInit();
                }
                else
                {
                    targets[targets.Count - 1].targetUis[i].raycast = false;
                    targets[targets.Count - 1].targetUis[i].alpha = 0;
                }
            }

            RemoveArray();
        }
    }
}