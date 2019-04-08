using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class UserInfo
{
    //add more data that is going to be stored on database here
    public int? UserID;
    public int matchID;
    public string UserName;
    public bool InMatch;
    
    public UserInfo()
    {
        UserID = CombatView.GameFlowController.UserID;
        matchID = CombatView.GameFlowController.matchID;
        UserName = CombatView.GameFlowController.UserName;
        InMatch = CombatView.GameFlowController.InMatch;
    }
}
