using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Register : MonoBehaviour
{
    public GameObject Name;
    public GameObject PW;
    public GameObject Confirm;

    private string Username;
    private string Password;
    private string ConfirmPassword;
    private string form;
    private bool UsernameValid = false;

    void Start()
    {
        
    }

    public void RegisterButton()
    {
        print("Registration Successful!");
        // By default, this should push to Firebase, but I am not sure of where to write the data.
    }

    void Update()
    {
        Username = Name.GetComponent<InputField>().text;
        Password = PW.GetComponent<InputField>().text;
        ConfirmPassword = Confirm.GetComponent<InputField>().text;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Password != "" && Username != "" && ConfirmPassword != "" && Password == ConfirmPassword)
            {
                RegisterButton();
            }

            else if(Password != ConfirmPassword)
            {
                print("Password and Confirm password does not match!");
            }

            else{
                print("Invalid entries! Please try again.");
            }
        }
        
    }


}
