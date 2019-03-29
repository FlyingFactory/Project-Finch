using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class matchDetails
{
    //add more data that is going to be stored on database here
    public int matchedPlayer1;
    public int matchedPlayer2;
    public int matchID;
    public bool matchFound;


    public matchDetails()
    {
        matchID = PlayerScores.match_id;
        matchFound = PlayerScores.match_found;
        matchedPlayer1 = PlayerScores.matchedPlayer1;
        matchedPlayer2 = PlayerScores.matchedPlayer2;
    }
}
