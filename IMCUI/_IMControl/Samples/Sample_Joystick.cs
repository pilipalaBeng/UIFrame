using UnityEngine;
using System.Collections;
using System;
using IMCUI.UI;

public class Sample_Joystick : MonoBehaviour
{
    //public IMCJoystick joystick;
    public GameObject cube;
    public float speed = 1;

    public IMCButton jump;

    void Start()
    {
        jump.onPointerClick += Jump;
    }

    private void Jump(GameObject arg0)
    {
        if (cube.transform.GetChild(0).localPosition.y <= 0.1f)
            cube.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(new Vector3(0, 300, 0));
    }

    //void OnGUI()
    //{
    //    GUILayout.Label(joystick.horizontalValue.ToString());
    //    GUILayout.Label(joystick.verticalValue.ToString());
    //}

    //void Update()
    //{
    //    cube.transform.Translate(new Vector3(joystick.horizontalValue * speed, 0, joystick.verticalValue * speed));
    //}

}
