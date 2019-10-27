using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IMCUI.UI
{
    public class IMCUIManager : MonoBehaviour
    {
        private static EventSystem m_eventSystem;

        public static EventSystem IMCEventSystem
        {
            get
            {
                if (m_eventSystem == null)
                    m_eventSystem = FindObjectOfType<EventSystem>();
                return m_eventSystem;
            }
        }

        private static IMCUIManager instance;

        public static IMCUIManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<IMCUIManager>();
                    if (!instance)
                    {
                        GameObject tempGo = new GameObject("IMCUIManager");
                        instance = tempGo.AddComponent<IMCUIManager>();
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }
                return IMCUIManager.instance;
            }
        }
        private List<IMCCanvas> canvasList = new List<IMCCanvas>();

        public List<IMCCanvas> CanvasList
        {
            get
            {
                //if (canvasList.Count == 0)
                //{
                //    CreateCanvas
                //    addCanvas
                //}
                return canvasList;
            }
            set { canvasList = value; }
        }

        public IMCCanvas defaultCanvas
        {
            get
            {
                return CanvasList[0];
            }
        }

        public T FindCanvasByType<T>()
        {
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (canvasList[i].GetComponent<T>() != null)
                {
                    return canvasList[i].GetComponent<T>();
                }
            }
            return default(T);
        }

        public List<T> FindCanvasesByTypes<T>()
        {
            List<T> tempList = new List<T>();
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (canvasList[i].GetComponent<T>() != null)
                {
                    tempList.Add(canvasList[i].GetComponent<T>());
                }
            }
            return tempList;
        }

        public T FindCanvasByNameAndType<T>(string name)
        {
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (canvasList[i].GetComponent<T>() != null && name == canvasList[i].name)
                {
                    return canvasList[i].GetComponent<T>();
                }
            }
            return default(T);
        }

        public IMCCanvas FindCanvasByName(string name)
        {
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (name == canvasList[i].name)
                {
                    return canvasList[i];
                }
            }
            return null;
        }
        public List<IMCCanvas> FindCanvasesByName(string name)
        {
            List<IMCCanvas> tempList = new List<IMCCanvas>();
            for (int i = 0; i < canvasList.Count; i++)
            {
                if (name == canvasList[i].name)
                    tempList.Add(canvasList[i]);
            }
            return tempList;
        }

        #region private
        private List<IMCTaskLoader> taskQueues = new List<IMCTaskLoader>();
        private bool isLoadIng = false;
        public int MaxTaskCount = 2;
        public void AddTask(IMCTaskLoader loader)
        {
            if (!DetectionSupportControlType(loader))
                return;
            taskQueues.Add(loader);
            loader.loadState = LoadState.Wait;
            if (GetLoadingTaskCount() < MaxTaskCount)
                TaskProcessing(loader);
        }

        public void RemoveTask(string instanceID)
        {
            IMCTaskLoader loader = FindLoaderByInstanceID(instanceID);
            if (loader == null)
                return;
            if (loader.loadState == LoadState.Wait)
            {
                taskQueues.Remove(loader);
                loader.loadState = LoadState.Stop;
                loader.target.LoadComplete(loader);
                loader = null;
            }
            else if (loader.loadState == LoadState.Loading)
            {
                if (loader.cloudFileLoader != null)
                {
                    loader.cloudFileLoader.StopTask();
                    loader.cloudFileLoader = null;
                }
                if (loader.enumerator != null)
                {
                    StopCoroutine(loader.enumerator);
                    loader.enumerator = null;
                }
                taskQueues.Remove(loader);
                loader.loadState = LoadState.Stop;
                loader.target.LoadComplete(loader);
                loader = null;
                NextTask();
            }
        }

        private void TaskProcessing(IMCTaskLoader loader)
        {
            loader.loadState = LoadState.Loading;
            if (FileSystem.Instance.FileExists(loader.path))
            {
                loader.enumerator = LoadImageFromLocalEnumerator(loader);
                StartCoroutine(loader.enumerator);
            }
            else
                DownLoadImage(loader);
        }

        private void DownLoadImage(IMCTaskLoader loader)
        {
            loader.cloudFileLoader = CloudFileSystem.Instance.CreateHttpDownLoadByLoadType(loader.url, loader.path, LoadType.DownLoadMemory, loader.instanceID, loader);
            loader.cloudFileLoader.LoaderStateChange += LoaderStateChange;
        }

        private void LoaderStateChange(CloudFileLoader obj)
        {
            IMCTaskLoader loader = (IMCTaskLoader)obj.Parms;
            loader.cloudFileLoader = null;
            if (obj.loadState == LoadState.Complete)
            {
                FileSystem.Instance.WriteToFile(loader.path, obj.byteBuffer, obj.byteBuffer.Length);
                Texture2D t2d = new Texture2D(1, 1);
                t2d.LoadImage(obj.byteBuffer);
                obj.byteBuffer = null;
                obj = null;
                System.GC.Collect();
                SetTexture2DToImageControl(loader, t2d);
            }
            else if (obj.loadState != LoadState.Wait && obj.loadState != LoadState.Loading)
            {
                loader.loadState = LoadState.Error;
                taskQueues.Remove(loader);
                loader.target.LoadComplete(loader);
                loader = null;
                obj = null;
                System.GC.Collect();
                NextTask();
            }
        }
        private IEnumerator LoadImageFromLocalEnumerator(IMCTaskLoader loader)
        {
            var url =
#if UNITY_IOS
        "file://" + loader.path;
#else
        "file:///" + loader.path;
#endif
            WWW www = new WWW(url);
            yield return www;
            if (www.texture != null)
            {
                SetTexture2DToImageControl(loader, www.texture);
                Resources.UnloadUnusedAssets();
                //System.GC.Collect();
                www.Dispose();
                www = null;
            }
            yield return 0;
        }

        private void SetTexture2DToImageControl(IMCTaskLoader loader, Texture2D texture)
        {
            loader.loadState = LoadState.Complete;
            IMCUIBehaviour behaviour = loader.target;
            behaviour.SetTexture(texture);
            taskQueues.Remove(loader);
            loader.target.LoadComplete(loader);
            loader = null;
            NextTask();
        }

        private bool DetectionSupportControlType(IMCTaskLoader loader)
        {
            switch (loader.target.controlType)
            {
                case ControlType.IMCImage:
                    return true;
                case ControlType.IMCRawImage:
                    return true;
                default:
                    return false;
            }
        }

        private int GetLoadingTaskCount()
        {
            int count = 0;
            for (int i = taskQueues.Count - 1; i >= 0; i--)
            {
                if (taskQueues[i] != null)
                    if (taskQueues[i].loadState == LoadState.Loading)
                        count++;
            }
            return count;
        }

        private void NextTask()
        {
            if (taskQueues.Count > 0 && GetLoadingTaskCount() < MaxTaskCount)
            {
                int index = -1;
                for (int i = 0; i < taskQueues.Count; i++)
                {
                    if (taskQueues[i].loadState == LoadState.Wait)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                    TaskProcessing(taskQueues[index]);
            }
        }

        private IMCTaskLoader FindLoaderByInstanceID(string instanceID)
        {
            for (int i = taskQueues.Count - 1; i >= 0; i--)
            {
                if (taskQueues[i].instanceID == instanceID)
                    return taskQueues[i];
            }
            return null;
        }
        #endregion
    }
}