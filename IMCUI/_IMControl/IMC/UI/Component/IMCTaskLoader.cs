using UnityEngine;
using System.Collections;
namespace IMCUI.UI
{
    public class IMCTaskLoader {
        public ControlType controlType;
        public string instanceID;
        public IMCUIBehaviour target;
        public LoadState loadState;
        public IEnumerator enumerator = null;
        public CloudFileLoader cloudFileLoader;
        public string path;
        public string url;
    }
}
