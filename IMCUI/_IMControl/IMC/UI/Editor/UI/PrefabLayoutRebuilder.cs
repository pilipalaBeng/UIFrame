using IMCUI;
using IMCUI.UI;
using UnityEditor;
using UnityEngine;
namespace IMCUIEditor.UI
{
    [InitializeOnLoad]
    internal class PrefabLayoutRebuilder
    {
        static PrefabLayoutRebuilder()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdates;
        }

        static void OnPrefabInstanceUpdates(GameObject instance)
        {
            if (instance)
            {
                RectTransform rect = instance.transform as RectTransform;
                if (rect)
                    LayoutRebuilder.MarkLayoutForRebuild(rect);
            }
        }
    }
}
