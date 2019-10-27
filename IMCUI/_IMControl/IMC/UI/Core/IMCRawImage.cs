using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IMCUI.UI
{
    /// <summary>
    /// If you don't have or don't wish to create an atlas, you can simply use this script to draw a texture.
    /// Keep in mind though that this will create an extra draw call with each RawImage present, so it's
    /// best to use it only for backgrounds or temporary visible graphics.
    /// </summary>
    [AddComponentMenu("IMCUI/IMCRaw Image", 12)]
    public class IMCRawImage : MaskableGraphic
    {
        [FormerlySerializedAs("m_Tex")]
        [SerializeField]
        Texture m_Texture;
        [SerializeField]
        Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        protected IMCRawImage()
        {
            useLegacyMeshGeneration = false;
        }

        /// <summary>
        /// Returns the texture used to draw this Graphic.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return m_Texture;
            }
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Adjust the scale of the Graphic to make it pixel-perfect.
        /// </summary>

        public override void SetNativeSize()
        {
            Texture tex = mainTexture;
            if (tex != null)
            {
                int w = Mathf.RoundToInt(tex.width * uvRect.width);
                int h = Mathf.RoundToInt(tex.height * uvRect.height);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Texture tex = mainTexture;
            vh.Clear();
            if (tex != null)
            {
                var r = GetPixelAdjustedRect();
                var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

                {
                    var color32 = color;
                    vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(m_UVRect.xMin, m_UVRect.yMin));
                    vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(m_UVRect.xMin, m_UVRect.yMax));
                    vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(m_UVRect.xMax, m_UVRect.yMax));
                    vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(m_UVRect.xMax, m_UVRect.yMin));

                    vh.AddTriangle(0, 1, 2);
                    vh.AddTriangle(2, 3, 0);
                }
            }
        }
        #region ExpandRawImage
        protected override void Awake()//2017年5月25日10:05:26 添加 rawimag 自动改变枚举值
        {
            base.Awake();
            m_containerType = ContainerType.Control;
            m_controlType = ControlType.IMCRawImage;
        }

        public override float alpha
        {
            get
            {
                return color.a;
            }
            set
            {
                color = new Color(color.r, color.g, color.b, value);
            }
        }
        public override bool raycast
        {
            get
            {
                return raycastTarget;
            }
            set
            {
                raycastTarget = value;
            }
        }
        public override bool interact
        {
            get
            {
                return raycastTarget;
            }
            set
            {
                raycastTarget = value;
                alpha = value ? 1 : 0.3f;
            }
        }

        public string path
        {
            get
            {
                return m_path;
            }
        }

        public string url
        {
            get
            {
                return m_url;
            }
        }

        public bool isLoadIng = false;
        #endregion


        private string m_path;
        private string m_url;
        public void LoadImage(string path)
        {
            if (isLoadIng)
                StopLoadImage();
            IMCTaskLoader itl = new IMCTaskLoader();
            if (FileSystem.Instance.FileExists(path))
            {
                isLoadIng = true;
                m_path = path;
                m_url = "";
                itl.instanceID = GetInstanceID().ToString();
                itl.target = this;
                IMCUIManager.Instance.AddTask(itl);
            }
            else
            {
                itl.loadState = LoadState.Error;
                LoadComplete(itl);
            }
        }

        public void LoadImage(string url, string path)
        {
            if (isLoadIng)
                StopLoadImage();

            IMCTaskLoader itl = new IMCTaskLoader();
            if (FileSystem.Instance.FileExists(path) || !FileSystem.Instance.FileExists(path) && url != "")
            {
                isLoadIng = true;
                m_url = url;
                m_path = path;
                itl.controlType = controlType;
                itl.instanceID = GetInstanceID().ToString();
                itl.target = this;
                itl.path = m_path;
                itl.url = m_url;
                IMCUIManager.Instance.AddTask(itl);
            }
            else
            {
                itl.loadState = LoadState.Error;
                LoadComplete(itl);
            }
        }

        public void StopLoadImage()
        {
            isLoadIng = false;
            IMCUIManager.Instance.RemoveTask(GetInstanceID().ToString());
        }

        public override void LoadComplete(IMCTaskLoader loader)
        {
            isLoadIng = false;
            if (LoadStateChangeAction != null)
                LoadStateChangeAction(loader.loadState);
            loader = null;
        }

        public override void SetTexture(Texture texture)
        {
            if (this.texture)
            {
                //DestroyImmediate(this.texture);
                Resources.UnloadUnusedAssets();
            }
            if (this != null)
                this.texture = texture;
        }
    }
}
