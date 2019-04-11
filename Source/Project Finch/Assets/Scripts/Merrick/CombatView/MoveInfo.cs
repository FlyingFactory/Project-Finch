using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class MoveInfo
{
    //add more data that is going to be stored on database here
    public string u_player1Move;
    public string u_player2Move;
    public string a_playersMoves;

    public MoveInfo()
    { 
        this.u_player1Move = "default1";
        this.u_player2Move = "default2";
        this.a_playersMoves = "default3";

    }
}
