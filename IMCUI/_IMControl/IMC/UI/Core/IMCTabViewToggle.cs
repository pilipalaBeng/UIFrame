using UnityEngine;
using System.Collections;

namespace IMCUI.UI
{
    public class IMCTabViewToggle : MonoBehaviour
    {
        private IMCToggle m_tabControl;

        public IMCToggle toggle
        {
            get
            {
                if (!m_tabControl)
                    m_tabControl = this.GetComponent<IMCToggle>();
                return m_tabControl;
            }
        }
        public IMCTabView tabView;

        void Start()
        {
            toggle.onValueChanged.AddListener((bool bo) =>
            {
                tabView.TabViewControlShowOrClose(bo, this);
            });
        }
    }
}