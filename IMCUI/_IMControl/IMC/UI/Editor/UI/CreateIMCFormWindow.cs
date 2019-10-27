using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using IMCUI.UI;
using System.Reflection;
using System;
using UnityEngine.EventSystems;

namespace IMCUIEditor.UI
{
    public class CreateIMCFormWindow : EditorWindow
    {
        private static CreateIMCFormWindow window;
        private string formName = "";
        private string titleText = "please input IMCForm name!";
        [MenuItem("GameObject/CreateIMCUI_Csharp/CreateForm", false, 0)]
        static void CreatWindow()
        {
            Rect wd = new Rect(500, 500, 300, 100);
            window = (CreateIMCFormWindow)EditorWindow.GetWindowWithRect(typeof(CreateIMCFormWindow), wd, true, "CreateIMCForm");
            window.Show();
        }
        void OnGUI()
        {
            EditorGUILayout.SelectableLabel(titleText);
            formName = EditorGUILayout.TextField("IMCForm name", formName);

            if (GUILayout.Button("Ok", GUILayout.Width(290)))
            {
                if (formName != string.Empty)
                {
                    window.Close();
                    CreateIMCFormCsharp.Instance.CreateScript(formName);
                    AssetDatabase.Refresh();
                }
                else
                {
                    titleText = "dont input empty name!";
                }
            }
        }

        //[MenuItem("GameObject/替换", false, 0)]//2017年9月1日10:36:21 防止误点击，注销该方法
        //static void Th()
        //{
        //    GameObject[] obj = FindObjectsOfType<GameObject>();
        //    for (int i = 0; i < obj.Length; i++)
        //    {
        //        if (!obj[i].GetComponent<IMCForm>())
        //        {
        //            if (obj[i].GetComponent<CanvasGroup>())
        //            {
        //                DestroyImmediate(obj[i].GetComponent<CanvasGroup>());
        //            }
        //        }
        //        if (obj[i].name!="Canvas"&&obj[i].GetComponent<UnityEngine.UI.GraphicRaycaster>())
        //        {
        //             DestroyImmediate(obj[i].GetComponent<UnityEngine.UI.GraphicRaycaster>());
        //             DestroyImmediate(obj[i].GetComponent<Canvas>());
        //        }
        //        if (obj[i].GetComponent<UnityEngine.UI.Mask>())
        //        {
        //            obj[i].AddComponent<IMCUI.UI.Mask>();
        //            DestroyImmediate(obj[i].GetComponent<UnityEngine.UI.Mask>());
        //        }
        //        if (obj[i].GetComponent<Image>())
        //        {
        //            MTh<Image, IMCImage>(obj[i].gameObject);
        //        }

        //        if (obj[i].GetComponent<Text>())
        //        {
        //            MTh<Text, IMCText>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<RawImage>())
        //        {
        //            MTh<RawImage, IMCRawImage>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<Slider>())
        //        {
        //            MTh<Slider, IMCSlider>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<Scrollbar>())
        //        {
        //            MTh<Scrollbar, IMCScrollbar>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<ScrollRect>())
        //        {
        //            MTh<ScrollRect, IMCScrollRect>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<InputField>())
        //        {
        //            MTh<InputField, IMCInputField>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<Dropdown>())
        //        {
        //            MTh<Dropdown, IMCDropdown>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<Toggle>())
        //        {
        //            MTh<Toggle, IMCToggle>(obj[i].gameObject);
        //        }
        //        if (obj[i].GetComponent<UnityEngine.UI.Selectable>())
        //        {
        //            MTh<UnityEngine.UI.Selectable, IMCButton>(obj[i].gameObject);
        //        }
        //    }
        //}


        //static void MTh<Old, New>(GameObject obj)//2017年9月1日10:36:21 防止误点击，注销该方法
        //{
        //    Type type = typeof(Old);
        //    Type type2 = typeof(New);
        //    //Type type2 = Type.GetType("IMC" + type.Name);


        //    Old oldCom = (Old)(object)obj.GetComponent(type);

        //    if (oldCom != null)
        //    {
        //        New newCom = (New)(object)obj.GetComponent(type2);
        //        if (obj.GetComponent<IMCText>())
        //        {
        //            newCom = (New)(object)obj.GetComponent(type2);
        //        }
        //        else
        //            newCom = (New)(object)obj.AddComponent(type2);
        //        foreach (System.Reflection.PropertyInfo oldP in oldCom.GetType().GetProperties())
        //        {
        //            foreach (System.Reflection.PropertyInfo newP in newCom.GetType().GetProperties())
        //            {
        //                if (oldP.Name == newP.Name && oldP.PropertyType == newP.PropertyType)
        //                {
        //                    try
        //                    {
        //                        newP.SetValue(newCom, oldP.GetValue(oldCom, null), null);
        //                    }
        //                    catch
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }
        //        }
        //        DestroyImmediate(oldCom as UnityEngine.Object);
        //    }
        //}
    }
}