using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;

public class PlayerScores : MonoBehaviour
{
    public Text scoreText;
    public InputField nameText;
    private System.Random random = new System.Random();

    public static int player_score;
    public static string player_name;


    // Start is called before the first frame update
    void Start()
    {
        player_score = random.Next(0,100);
        scoreText.text = "Score:" + player_score.ToString();
    }

    public void OnSubmit()
    {
        player_name = nameText.text;
        PostToDatabase();
    }

    public void PostToDatabase()
    {
        User user = new User();
        RestClient.Post("https://project-finch-database.firebaseio.com/.json", user);
    }

}
