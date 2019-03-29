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

    User user = new User();
    public static int player_score;
    public static string player_name;


    // Start is called before the first frame update
    void Start()
    {
        //just to generate a random score to simulate game data that needs to be sent online
        player_score = random.Next(0,100);
        scoreText.text = "Score:" + player_score.ToString();
    }

    //function call when submit button is clicked
    public void OnSubmit()
    {
        player_name = nameText.text;
        scoreText.text = "Score:" + player_score.ToString();
        PostToDatabase();
    }

    public void OnGetScore()
    {
        RetrieveFromDatabase();
    }

    private void UpdateScore()
    {
        scoreText.text = "Score:"+ user.userScore;
    }

    //called to post to firebase.
    public void PostToDatabase()
    {
        User user = new User(); //user is a class that contains player_name and player_score, which will be posted to firebase
        RestClient.Put("https://project-finch-database.firebaseio.com/"+player_name+".json", user); //link is the firebase i created
    }

    public void RetrieveFromDatabase()
    {
        RestClient.Get<User>("https://project-finch-database.firebaseio.com/"+nameText.text+".json").Then(response =>
        {
            user = response;
            UpdateScore();
        });
    }
}
