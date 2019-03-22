using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

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
