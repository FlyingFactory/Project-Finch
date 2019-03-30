using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class MatchDetails
{
    //add more data that is going to be stored on database here
    public int matchedPlayer1;
    public int matchedPlayer2;
    public int matchID;
    public bool matchFound;
    public string matchDetail;


    public MatchDetails()
    {
        matchID = CombatView.GameFlowController.matchID;
        matchedPlayer1 = CombatView.GameFlowController.matchedPlayer1;
        matchedPlayer2 = CombatView.GameFlowController.matchedPlayer2;
        matchFound = CombatView.GameFlowController.matchfound;
        matchDetail = CombatView.GameFlowController.match_details1;
        
    }
}
