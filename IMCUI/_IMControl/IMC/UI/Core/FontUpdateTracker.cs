using System;
using System.Collections.Generic;
using IMCUI;
using UnityEngine;

namespace IMCUI.UI
{
    public static class FontUpdateTracker
    {
        static Dictionary<Font, List<IMCText>> m_Tracked = new Dictionary<Font, List<IMCText>>();

        public static void TrackText(IMCText t)
        {
            if (t.font == null)
                return;

            List<IMCText> exists;
            m_Tracked.TryGetValue(t.font, out exists);
            if (exists == null)
            {
                // The textureRebuilt event is global for all fonts, so we add our delegate the first time we register *any* Text
                if (m_Tracked.Count == 0)
                    Font.textureRebuilt += RebuildForFont;

                exists = new List<IMCText>();
                m_Tracked.Add(t.font, exists);
            }

            if (!exists.Contains(t))
                exists.Add(t);
        }

        private static void RebuildForFont(Font f)
        {
            List<IMCText> texts;
            m_Tracked.TryGetValue(f, out texts);

            if (texts == null)
                return;

            for (var i = 0; i < texts.Count; i++)
                texts[i].FontTextureChanged();
        }

        public static void UntrackText(IMCText t)
        {
            if (t.font == null)
                return;

            List<IMCText> texts;
            m_Tracked.TryGetValue(t.font, out texts);

            if (texts == null)
                return;

            texts.Remove(t);

            if (texts.Count == 0)
            {
                m_Tracked.Remove(t.font);

                // There is a global textureRebuilt event for all fonts, so once the last Text reference goes away, remove our delegate
                if (m_Tracked.Count == 0)
                    Font.textureRebuilt -= RebuildForFont;
            }
        }
    }
}
