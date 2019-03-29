using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

//class to store variables and data that will be sent to firebase with PostToDatabase() function call
public class User
{
    //add more data that is going to be stored on database here
    public String userName;
    public int userScore;
    public int userId;
    

    public User()
    {
        userName = PlayerScores.player_name;
        userScore = PlayerScores.player_score;
    }
}
