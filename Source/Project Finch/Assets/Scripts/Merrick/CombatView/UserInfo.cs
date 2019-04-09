using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

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
    public string Soldiers;

    public UserInfo()
    {
        userId = CombatView.GameFlowController.userId;
        matchID = CombatView.GameFlowController.matchID;
        userName = CombatView.GameFlowController.userName;
        InMatch = CombatView.GameFlowController.InMatch;
        NumberOfSoldiers = CombatView.GameFlowController.numberOfSoldiers;
        Soldiers = CombatView.GameFlowController.Soldiers;
        Password = CombatView.GameFlowController.Password;
    }
}
