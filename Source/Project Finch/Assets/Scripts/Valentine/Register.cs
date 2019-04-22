using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    public InputField username;
    public InputField pw;
    public InputField confirm_pw;

    private string Username;
    private string Password;
    private string ConfirmPassword;
    private string form;
    //private bool UsernameValid = false;


    public void RegisterButton()
    {
        Username = username.text;
        Password = pw.text;
        ConfirmPassword = confirm_pw.text;

        if (Password != "" && Username != "" && ConfirmPassword != "" && Password == ConfirmPassword)
        {
            print("creating account..");
            MenuView.PlayerAccount new_acc = new MenuView.PlayerAccount();
            new_acc.createNewAccount(Username, Password);
            print("Registration Successful!");
            SceneManager.LoadSceneAsync("LoginMenu");

        }

        else if (Password != ConfirmPassword)
        {
            Debug.LogWarning("Passwords do not match. Please reenter");
        }

        else
        {
            Debug.LogWarning("Invalid entries! Please try again.");
        }
        
        
    }


}
