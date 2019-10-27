using UnityEngine;
using System.Collections;

namespace IMCUI.UI
{
    public abstract class IMCViewItem : IMCUIBehaviour
    {
        public int Index = -1;
        abstract public void SetValue(IMCViewItem item, int index);
    }
}

