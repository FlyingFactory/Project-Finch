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
        string x = userId.text;

        MenuView.PlayerAccount.LoadDataInfo testLoadData = new MenuView.PlayerAccount.LoadDataInfo(x);
        
        MenuView.PlayerAccount.LoadData_Thread(testLoadData);
    }

    public void onGetPassword()
    {
        string x = userId.text;
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

    public void onCreateNewAccount()
    {
        string x = userId.text;
        string y = Password.text;
        MenuView.PlayerAccount player = new MenuView.PlayerAccount();
        player.createNewAccount(x,y);
    }

    public void onPutSoldier()
    {
        MenuView.Soldier new_soldier = new MenuView.Soldier();
        MenuView.PlayerAccount player = new MenuView.PlayerAccount();
        player.soldierNameList = new List<string>();
        player.userName = "user001";
        player.counter = 2;
        player.putSoldier(new_soldier, true);
    }

    public void onStringToDict()
    {
        string x = "{\"InMatch\":true,\"Password\":\"0a543f6269d88d66824e3305226bf311\",\"Soldiers\":{\"Soldier1\":{\"aim\":65,\"characterClass\":0,\"complete\":false,\"experience\":0.0,\"fatigue\":0.0,\"index\":\"1\",\"level\":1,\"maxHealth\":6,\"mobility\":6,\"name\":\"undefined\",\"owner\":\"\"},\"Soldier2\":{\"aim\":65,\"characterClass\":0,\"complete\":false,\"experience\":0.0,\"fatigue\":0.0,\"index\":\"2\",\"level\":1,\"maxHealth\":6,\"mobility\":6,\"name\":\"undefined\",\"owner\":\"\"},\"Soldier3\":{\"aim\":65,\"characterClass\":0,\"complete\":false,\"experience\":0.0,\"fatigue\":0.0,\"index\":\"3\",\"level\":1,\"maxHealth\":6,\"mobility\":6,\"name\":\"undefined\",\"owner\":\"\"}},\"matchID\":-1,\"numberOfSoldiers\":3,\"rankedMMR\":0.0,\"soldierList\":{\"value\":\"1,2,3\"},\"unrankedMMR\":0.0,\"userId\":\"\",\"userName\":\"user001\"}";
        //MenuView.PlayerAccount.stringToClass(x);
        MenuView.PlayerAccount.LoadDataInfo y = new MenuView.PlayerAccount.LoadDataInfo("user001");
        MenuView.PlayerAccount.getFromDatabase_thread(y);
    }
}
