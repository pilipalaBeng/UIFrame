#if UNITY_EDITOR
using System.Collections.Generic;
using IMCUI.UI.Collections;
using UnityEngine;

namespace IMCUI.UI
{
    public static class GraphicRebuildTracker
    {
        static IList<Graphic> m_Tracked = new IndexedSet<Graphic>();
        static bool s_Initialized;

        public static void TrackGraphic(Graphic g)
        {
            if (!s_Initialized)
            {
                CanvasRenderer.onRequestRebuild += OnRebuildRequested;
                s_Initialized = true;
            }

            m_Tracked.Add(g);
        }

        public static void UnTrackGraphic(Graphic g)
        {
            m_Tracked.Remove(g);
        }

        static void OnRebuildRequested()
        {
            StencilMaterial.ClearAll();
            for (int i = 0; i < m_Tracked.Count; i++)
            {
                m_Tracked[i].OnRebuildRequested();
            }
        }
    }
}
#endif // if UNITY_EDITOR
