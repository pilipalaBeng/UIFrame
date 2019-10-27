using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IMCUI.UI
{
    public class IMCCanvas : IMCUIBehaviour
    {
        public string Name = "";
        public AudioClip buttonClickClip;
        public List<IMCForm> forms = new List<IMCForm>();
        private AudioSource m_audioSource;
        private AudioSource audioSource
        {
            get
            {
                if (!m_audioSource)
                {
                    m_audioSource = this.GetComponent<AudioSource>();
                    if (!m_audioSource)
                        m_audioSource = this.gameObject.AddComponent<AudioSource>();
                }
                return m_audioSource;
            }
        }
        public void PlayAudio(AudioClip clip)
        {
            if (clip)
                audioSource.clip = clip;
            else
                audioSource.clip = buttonClickClip;
            if (audioSource.clip)
                audioSource.Play();
        }
        protected override void Awake()
        {
            if (!m_audioSource)
                m_audioSource = this.GetComponent<AudioSource>();
            m_controlType = ControlType.IMCCanvas;
            m_containerType = ContainerType.Container;

            IMCUIManager.Instance.CanvasList.Add(this);
            for (int i = 0; i < this.transform.childCount; i++)
            {
                IMCForm tempform = this.transform.GetChild(i).gameObject.GetComponent<IMCForm>();
                if (tempform)
                {
                    tempform.canvas = this;
                    forms.Add(tempform);

                    if (tempform.showOnAwake)
                        tempform.Show();
                }
            }
            for (int i = 0; i < forms.Count; i++)
            {
                forms[i].Initialize();
            }
        }
        /// <summary>
        /// 向forms数组中添加对应元素
        /// </summary>
        public void RegisterForm(IMCForm form)
        {
            form.canvas = this;
            form.Initialize();
            if (!forms.Contains(form))
                forms.Add(form);
        }
        /// <summary>
        /// 从forms数组中移除掉对应元素
        /// </summary>
        public void UnRegisterForm(IMCForm form)
        {
            form.canvas = null;
            if (forms.Contains(form))
                forms.Remove(form);
        }
        /// <summary>
        /// 通过form的name属性获取forms数组内对应的元素,如果没有符合条件的元素，返回null
        /// </summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMCForm FindFormByName(string name)
        {
            if (forms.Count > 0)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    if (forms[i].name == name)
                    {
                        return forms[i];
                    }
                }
            }
            return null;
        }
        public List<IMCForm> FindFormsByName(string name)
        {
            List<IMCForm> returnForms = new List<IMCForm>();
            if (forms.Count > 0)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    if (forms[i].name == name)
                        returnForms.Add(forms[i]);
                }
            }
            return returnForms;
        }
        public T FindFormByByte<T>() where T:IMCForm
        {
            if (forms.Count > 0)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    if (forms[i].GetComponent<T>() != null)
                    {
                        return forms[i].GetComponent<T>();
                    }
                }
            }
            return default(T);
        }
        public List<T> FindFormsByByte<T>() where T : IMCForm
        {
            List<T> returnForms = new List<T>();
            if (forms.Count > 0)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    if (forms[i].GetComponent<T>() != null)
                        returnForms.Add(forms[i].GetComponent<T>());
                }
            }
            return returnForms;
        }
        protected override void OnDestroy()
        {
            if (IMCUIManager.Instance.CanvasList.Contains(this))
                IMCUIManager.Instance.CanvasList.Remove(this);
        }
        private IMCGraphicRaycaster graphocRaycaster = null;
        public void RaycastSwitch(bool isSwitch)
        {
            if (graphocRaycaster==null) 
                graphocRaycaster = this.GetComponent<IMCGraphicRaycaster>();
            if (graphocRaycaster == null)
                return;
            graphocRaycaster.RaycastSwitch(!isSwitch);
        }
    }
}
