using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using IMCUI.UI;

public class Sample_Forms: MonoBehaviour
{

    public IMCInputField inputField_id;
    string registId;
    public IMCInputField inputField_password;
    string password;
    public IMCInputField inputField_password2;

    public MessageBox box;

    void Start()
    {
        //print(IMControlManager.OneCanvas.transform.GetChild(0).name);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Message show"))
        {
            box.Create(messageBoxBack);
            //box.Show();
            box.Show(Vector3.zero);
            box.MoveTo(Vector3.zero, 0.5f);
        }
        if (GUILayout.Button("Message Close"))
        {
            box.Close();
        }
    }

    private void messageBoxBack(int arg0)
    {
        print("choice " + arg0);
    }

    void AddListeners()
    {
        inputField_id.onEndEdit.AddListener(inputField_idValidate);
        inputField_password.onEndEdit.AddListener(inputField_passwordValidate);
        inputField_password2.onEndEdit.AddListener(inputField_password2Validate);
    }

    private void inputField_idValidate(string str)
    {
        if (str == "")
        {
            print("name is empty!");
        }
        else if (str.Length > 8 || str.Length < 2)
        {
            print("name must 2-8 letters");
        }
        else
        {
            registId = str;
        }
        print("input: " + str);
    }
    private void inputField_passwordValidate(string str)
    {
        if (str == "")
        {
            print("password is empty!");
        }
        else if (str.Length > 8 || str.Length < 2)
        {
            print("password must 2-8 letters");
        }
        else
        {
            password = str;
        }
    }
    private void inputField_password2Validate(string str)
    {
        if (str != password)
        {
            print("not match!");
        }
    }
    public void UserRegist(string id, string password, string phoneNumber)
    {
        UserRegistInfo info = new UserRegistInfo(id, password, phoneNumber);
    }
}

public class UserRegistInfo
{
    public string id;
    public string password;
    public string phoneNumber;

    public UserRegistInfo(string id, string password, string phoneNumber)
    {
        this.id = id;
        this.password = password;
        this.phoneNumber = phoneNumber;
    }
}
