using IMCUI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sample_Controls : MonoBehaviour
{
    public IMCDropdown dropdown1;
    public IMCProgressBar progressBar;
    public float progressValue = 0;

    void Start()
    {
        List<string> options = new List<string>();
        for (int i = 1; i < 20; i++)
            options.Add("option " + i);

        dropdown1.AddOptions(options);
        dropdown1.SetCanvasGroup(true);

        //print(IMControlManager.OneCanvas.transform.GetChild(0).name);
    }

    void Update()
    {
        //progressBar.Value = (progressValue++) % (progressBar.sliderCom.maxValue + 1);
    }

    void OnGUI()
    {
        if (GUILayout.Button("dropdown1.Hide"))
        {
            dropdown1.Hide();
        }
    }

    void ValueChanged(int index)
    {
        //print("dropdown1 changed,selectedIndex " + index + "  " + dropdown1.ValueData.text);
    }
}
