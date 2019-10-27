using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;

namespace IMCUIEditor.EventSystems
{
    [CustomEditor(typeof(EventSystem), true)]
    public class EventSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
           
            DrawDefaultInspector();
            var eventSystem = target as EventSystem;
            if (eventSystem == null)
                return;

            if (eventSystem.GetComponent<BaseInputModule>() != null)
                return;

            // no input modules :(
            if (GUILayout.Button("Add Default Input Modules"))
            {
                Undo.AddComponent<IMCStandaloneInputModule>(eventSystem.gameObject);
                Undo.AddComponent<IMCTouchInputModule>(eventSystem.gameObject);
            }
        }

        public override bool HasPreviewGUI()
        {
            return Application.isPlaying;
        }

        private GUIStyle m_PreviewLabelStyle;

        protected GUIStyle previewLabelStyle
        {
            get
            {
                if (m_PreviewLabelStyle == null)
                {
                    m_PreviewLabelStyle = new GUIStyle("PreOverlayLabel")
                    {
                        richText = true,
                        alignment = TextAnchor.UpperLeft,
                        fontStyle = FontStyle.Normal
                    };
                }

                return m_PreviewLabelStyle;
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            var system = target as EventSystem;
            if (system == null)
                return;

            GUI.Label(rect, system.ToString(), previewLabelStyle);
        }
    }
}
