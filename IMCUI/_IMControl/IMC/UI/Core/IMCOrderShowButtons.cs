using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace IMCUI.UI
{
    public class IMCOrderShowButtons : IMCUIBehaviour
    {
        public List<IMCUIBehaviour> targets;
        public IMCButton leftBtn;
        public IMCButton rightBtn;

        private int i_index;
        private bool is_left = true;
        private bool is_right = false;
        protected override void Awake()
        {
            base.Awake();
            m_controlType = ControlType.IMCOrderShowButtons;
            m_containerType = ContainerType.Control;
        }
        protected override void Start()
        {
            base.Start();
            leftBtn.AddListener(LeftBtnClick);
            rightBtn.AddListener(RightBtnClick);
        }
        private CanvasGroup m_canvasGroup;
        private CanvasGroup canvasGroup
        {
            get
            {
                if (m_canvasGroup == null)
                    m_canvasGroup = this.GetComponent<CanvasGroup>();
                return m_canvasGroup;
            }
        }
        public override float alpha
        {
            get
            {
                return canvasGroup.alpha;
            }

            set
            {
                canvasGroup.alpha = value;
            }
        }
        public override bool interact
        {
            get
            {
                return canvasGroup.interactable;
            }

            set
            {
                canvasGroup.interactable = value;
            }
        }
        public override bool raycast
        {
            get
            {
                return canvasGroup.blocksRaycasts;
            }

            set
            {
                canvasGroup.blocksRaycasts = value;
            }
        }
        public void LeftBtnClick()
        {
            MTurnPage(is_left);
        }

        public void RightBtnClick()
        {
            MTurnPage(is_right);
        }


        private void MTurnPage(bool is_leftOrRight)
        {
            if (is_leftOrRight)
            {
                if (i_index >= 1)
                    i_index--;
            }
            else
            {
                if (i_index < targets.Count - 1)
                    i_index++;
            }
            if (targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (i == i_index)
                    {
                        targets[i].active = true;
                        continue;
                    }
                    targets[i].active = false;
                }
            }
        }
        public void ResetTargets(int index = 0)
        {
            i_index = index;
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].gameObject.SetActive(i == 0 ? true : false);
            }
        }

    }
}