using System;
using UnityEngine;

namespace IMCUI.UI
{
    [Obsolete("Not supported anymore.", true)]
    public interface IMask
    {
        bool Enabled();
        RectTransform rectTransform { get; }
    }
}
