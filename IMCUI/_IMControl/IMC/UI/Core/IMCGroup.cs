using UnityEngine;
using System.Collections;

namespace IMCUI.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IMCUI/IMCGroup", 51)]
    public class IMCGroup : IMCUIContainer
    {
        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCGroup;
        }
    }
}