using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class MoveInfo
{
    //add more data that is going to be stored on database here
    public string moveInfo;
    public int moveNumber;
    public string NCmoveInfo;
    public int matchID;

    public MoveInfo()
    {
        this.matchID = 0;
        this.moveInfo = "default";
        this.moveNumber = 0;
        this.NCmoveInfo = "not confirmed default";
    }
}
