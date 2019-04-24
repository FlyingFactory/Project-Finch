using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class MatchDetails
{
    //add more data that is going to be stored on database here
    public string matchedPlayer1;
    public string matchedPlayer2;
    public int matchID;
    public bool matchFound;
    public string matchDetail;
    public int mapSeed;
    public string map;
    public bool complete;


    public MatchDetails()
    {
        matchID = -1;
        matchedPlayer1 = "";
        matchedPlayer2 = "";
        matchFound = false;
        matchDetail = "";
        map = "";
        mapSeed = -1;
        complete = false;

    }
}
