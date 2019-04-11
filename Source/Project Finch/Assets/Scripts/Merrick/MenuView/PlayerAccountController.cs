using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAccountController : MonoBehaviour
{
    public InputField userId;
    public InputField Password;

    public void onGetPlayerAccountsInfo()
    {
        int x = Convert.ToInt32(userId.text);

        MenuView.PlayerAccount.LoadDataInfo testLoadData = new MenuView.PlayerAccount.LoadDataInfo(x);
        
        MenuView.PlayerAccount.LoadData_Thread(testLoadData);
    }

    public void onGetPassword()
    {
        int x = Convert.ToInt32(userId.text);
        string y = Password.text;

        MenuView.PlayerAccount.LoginInfo testLoginInfo = new MenuView.PlayerAccount.LoginInfo(x , y);

        MenuView.PlayerAccount.LoginAndLoadAllData_Thread(testLoginInfo);
    }

    public void onCheckForMatch()
    {
        string x = userId.text;
        MenuView.PlayerAccount player = new MenuView.PlayerAccount();
        player.userId = x;
        MenuView.PlayerAccount.checkForMatch(player);
    }
}
