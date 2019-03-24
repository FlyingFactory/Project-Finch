using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class User
{
    public String userName;
    public int userScore;

    public User()
    {
        userName = PlayerScores.player_name;
        userScore = PlayerScores.player_score;
    }
}
