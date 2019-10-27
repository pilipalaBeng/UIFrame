using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace IMCUI.UI
{
    public class IMCUIResouceManager : MonoBehaviour
    {
        //private static GameObject createControlGo = null;
        ///// <summary>
        ///// 创建组件（通过Resources加载）
        ///// </summary>
        //public static GameObject CreatControl(ControlType contyolEnum, string name, string customID, Transform parent)
        //{

        //    if (contyolEnum != ControlType.None)
        //    {
        //        switch (contyolEnum)
        //        {
        //            case ControlType.IMCProgressBar:
        //                break;
        //            case ControlType.IMCText:
        //                break;
        //            case ControlType.IMCImage:
        //                break;
        //            case ControlType.IMCRawImage:
        //                break;
        //            case ControlType.IMCButton:
        //                break;
        //            case ControlType.IMCToggle:
        //                break;
        //            case ControlType.IMCSlider:
        //                break;
        //            case ControlType.IMCScrollbar:
        //                break;
        //            case ControlType.IMCDropdown:
        //                break;
        //            case ControlType.IMCInputField:
        //                break;
        //            case ControlType.IMCScrollRect:
        //                break;
        //            case ControlType.IMCForm:
        //                break;
        //            case ControlType.IMCTabView:
        //                break;
        //            case ControlType.IMCGroup:
        //                break;
        //            case ControlType.IMCMessageBox:
        //                break;
        //            case ControlType.IMCCascadeDropDown:
        //                break;
        //        }
        //    }
        //    return createControlGo;
        //}
        class ResourceUnit
        {
            public string assetsPath;
            public UnityEngine.Object resourceObject;
            public GameObject gameObject;
        }
        private static Dictionary<string, ResourceUnit> loadCacheDic = new Dictionary<string, ResourceUnit>();
        private static Type loadType = null;
        private static GameObject loadInstantiateGo = null;
        private static GameObject loadCloneGo = null;
        /// <summary>
        /// 加载组件
        /// </summary>
        public static T LoadControl<T>(string path, Transform parent = null)
        {
            JudgeJumpScene();
            loadType = typeof(T);
            MemoryOptimization(path + "/" + loadType.ToString());

            if (loadCacheDic.ContainsKey(path + "/" + loadType.ToString()))
                ResetDictionaryList(loadCacheDic[path + "/" + loadType.ToString()]);
            else
                AddDictionaryList(path + "/" + loadType.ToString());

            return TransformInitialize(parent, Instantiate(loadCacheDic[path + "/" +loadType.ToString()].gameObject) as GameObject).GetComponent<T>();
        }

        /// <summary>
        /// 加载组件
        /// </summary>
        public static GameObject LoadControl(string path, string name, Transform parent = null)
        {
            JudgeJumpScene();
            MemoryOptimization(path + "/" + name);

            if (loadCacheDic.ContainsKey(path + "/" + name))
                ResetDictionaryList(loadCacheDic[path + "/" + name]);
            else
                AddDictionaryList(path + "/" + name);

            return TransformInitialize(parent, Instantiate(loadCacheDic[path + "/" + name].gameObject) as GameObject);
        }
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="pathAndName">注意：path 和 name 中间有"/"分割</param>
        public static GameObject LoadControl(string pathAndName, Transform parent = null)
        {
            JudgeJumpScene();
            MemoryOptimization(pathAndName);

            if (loadCacheDic.ContainsKey(pathAndName))
                ResetDictionaryList(loadCacheDic[pathAndName]);
            else
                AddDictionaryList(pathAndName);
            return TransformInitialize(parent, Instantiate(loadCacheDic[pathAndName].gameObject) as GameObject);
        }
        private static void AddDictionaryList(string pathAndName)
        {
            ResourceUnit tempUnit = new ResourceUnit();
            tempUnit.assetsPath = pathAndName;
            tempUnit.resourceObject = Resources.Load(pathAndName);
            tempUnit.gameObject = Instantiate(tempUnit.resourceObject) as GameObject;
            tempUnit.gameObject.SetActive(false);
            loadCacheDic.Add(pathAndName, tempUnit);
        }

        private static void ResetDictionaryList(ResourceUnit resourceUnit)
        {
            if (resourceUnit.resourceObject == null)
                resourceUnit.resourceObject = Resources.Load(resourceUnit.assetsPath);
            if (resourceUnit.gameObject == null)
                resourceUnit.gameObject = Instantiate(resourceUnit.resourceObject) as GameObject;
            resourceUnit.gameObject.SetActive(false);
        }

        private static string sceneName = "";
        private static void JudgeJumpScene()
        {
            if (sceneName == "")
                sceneName = SceneManager.GetActiveScene().name;
            else if (sceneName != SceneManager.GetActiveScene().name)
            {
                sceneName = SceneManager.GetActiveScene().name;
                loadCacheDic.Clear();
                m_listMemoryOptimization.Clear();
            }
        }
        private static List<string> m_listMemoryOptimization = new List<string>();
        private static List<string> m_listRemoveRecord = new List<string>();
        private static int temp = 1;

        private static void MemoryOptimization(string m_strPathName)
        {
            m_listMemoryOptimization.Add(m_strPathName);
            if (m_listMemoryOptimization.Count >= 5)
            {
                string m_strTemp = "";

                foreach (KeyValuePair<string, ResourceUnit> item in loadCacheDic)
                {
                    m_listMemoryOptimization.Add(item.Key);
                }
                for (int i = 0; i < m_listMemoryOptimization.Count; i++)
                {
                    m_strTemp = m_listMemoryOptimization[i];
                    for (int j = i + 1; j < m_listMemoryOptimization.Count; j++)
                    {
                        if (m_strTemp == m_listMemoryOptimization[j])
                        {
                            if (!m_listRemoveRecord.Contains(m_strTemp))
                                m_listRemoveRecord.Add(m_strTemp);
                        }
                    }
                }
                if (m_listRemoveRecord.Count > 0)
                {
                    for (int i = 0; i < m_listRemoveRecord.Count; i++)
                        m_listMemoryOptimization.Remove(m_listRemoveRecord[i]);

                    for (int i = 0; i < m_listRemoveRecord.Count; i++)
                    {
                        if (loadCacheDic.ContainsKey(m_listRemoveRecord[i]))
                        {
                            Destroy(loadCacheDic[m_listRemoveRecord[i]].gameObject);
                            loadCacheDic.Remove(m_listRemoveRecord[i]);
                        }
                    }
                }

                m_listMemoryOptimization.Clear();
                m_listRemoveRecord.Clear();
            }
        }
        private static GameObject TransformInitialize(Transform parent)
        {
            loadCloneGo.SetActive(true);
            if (parent == null)
                loadCloneGo.transform.SetParent(IMCUIManager.Instance.CanvasList[0].transform);
            else
                loadCloneGo.transform.SetParent(parent);
            loadCloneGo.transform.localScale = Vector3.one;
            loadCloneGo.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            return loadCloneGo;
        }

        private static GameObject TransformInitialize(Transform parent, GameObject child)
        {
            child.SetActive(true);
            if (parent == null)
                child.transform.SetParent(IMCUIManager.Instance.CanvasList[0].transform);
            else
                child.transform.SetParent(parent);
            child.transform.localScale = Vector3.one;
            child.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            return child;
        }
    }
}
