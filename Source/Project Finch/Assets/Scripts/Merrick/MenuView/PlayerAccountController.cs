using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAccountController : MonoBehaviour
{
    public InputField userId;

    public void onGetPlayerAccountsInfo()
    {
        int x = Convert.ToInt32(userId.text);

        MenuView.PlayerAccount.LoadDataInfo testLoadData = new MenuView.PlayerAccount.LoadDataInfo(x);
        
        MenuView.PlayerAccount.LoadData_Thread(testLoadData);
    }
}
