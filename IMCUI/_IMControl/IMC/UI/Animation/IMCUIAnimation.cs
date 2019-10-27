using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace IMCUI.UI
{
    public class IMCUIAnimation : MonoBehaviour
    {

        public IEnumerator MoveTo(IMCUIBehaviour obj, Vector3 targetPos, float time, UnityAction callback = null)
        {
            float recordTime = 0;//记录
            float operationTime = time;//运算
            float normalizedTime = 0;//归一化

            RectTransform objRT = obj.rectTransform;
            Vector3 beforePos = objRT.anchoredPosition3D;
            Vector3 vec = targetPos - obj.anchoredPosition3D;
            while (normalizedTime <= 1)
            {
                recordTime += Time.deltaTime;
                normalizedTime = recordTime / operationTime;
                objRT.anchoredPosition3D = beforePos + vec * Mathf.Clamp01(normalizedTime);
                yield return new WaitForEndOfFrame();
            }
            yield return 0;
            if (callback != null) callback();
            Destroy(this);
        }
        public IEnumerator RotationTo(IMCUIBehaviour obj, Vector3 targetAngle, float time, UnityAction callbacke = null)
        {
            float recordTime = 0;
            float operationTime = time;
            float normalizedTime = 0;

            Vector3 beforeAngle = obj.transform.eulerAngles;
            Vector3 distanceAngle = targetAngle - obj.transform.eulerAngles;
            while (normalizedTime <= 1)
            {
                recordTime += Time.deltaTime;
                normalizedTime = recordTime / operationTime;
                obj.transform.eulerAngles = beforeAngle + distanceAngle * Mathf.Clamp01(normalizedTime);
                yield return new WaitForEndOfFrame();
            }
            yield return 0;
            if (callbacke != null) callbacke();
            Destroy(this);
        }
        /// <summary>
        /// 推荐time设置为0.1f
        /// </summary>
        public IEnumerator ScaleTo(IMCUIBehaviour obj, Vector3 targetScale, float time, UnityAction callback = null)
        {
            float recordTime = 0;
            float operationTime = time;
            float normalizedTime = 0;

            Transform objTF = obj.transform;
            Vector3 beforeSize = obj.transform.localScale;
            Vector3 distanceScale = targetScale - obj.transform.localScale;
            while (normalizedTime <= 1)
            {
                recordTime += Time.deltaTime;
                normalizedTime = recordTime / operationTime;
                obj.transform.localScale = beforeSize + distanceScale * Mathf.Clamp01(normalizedTime);
                yield return new WaitForEndOfFrame();
            }
            yield return 0;
            if (callback != null) callback();
            Destroy(this);
        }
        //public IEnumerator ColorTo(IMCUIBehaviour obj,Color targetColor,float time,UnityAction callback=null)
        //{
           
        //}
        public IEnumerator AlphaTo(IMCUIBehaviour obj, float alpha,float time,UnityAction callback=null)
        {
            float recordTime = 0;
            float operationTime = time;
            float normalizedTime = 0;

            Transform objTF = obj.transform;

            float beforeAlpha = obj.alpha;
            float distanceAlpha = alpha - obj.alpha;

            while (normalizedTime <= 1)
            {
                recordTime += Time.deltaTime;
                normalizedTime = recordTime / operationTime;
                obj.alpha = beforeAlpha + distanceAlpha * Mathf.Clamp01(normalizedTime);
                yield return new WaitForEndOfFrame();
            }
            yield return 0;
            if (callback != null) callback();
            Destroy(this);
        }

    }
}