using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class UserInfo
{
    //add more data that is going to be stored on database here
    public int? userId;
    public int matchID;
    public string userName;
    public bool InMatch;
    public int NumberOfSoldiers;
    public string Password;
    public MenuView.Soldier Soldier;
    public string soldierList;
    public List<MenuView.Soldier> listOfSoldiers;

    public UserInfo()
    {
        this.userId = 0;
        //matchID = CombatView.GameFlowController.matchID;
        //userName = "User";
        //InMatch = CombatView.GameFlowController.InMatch;
        //NumberOfSoldiers = CombatView.GameFlowController.numberOfSoldiers;
        //MenuView.Soldier soldier = new MenuView.Soldier();
        //Password = CombatView.GameFlowController.Password;
        //soldierList = CombatView.GameFlowController.soldierList;
        //listOfSoldiers = CombatView.GameFlowController.listOfSoldiers;
        
    }
}
