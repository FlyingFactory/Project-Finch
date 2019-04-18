using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToRoster()
    {
        SceneManager.LoadScene(3);
    }

    public void RostertoMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void Logout()
    {
        SceneManager.LoadSceneAsync(5);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
        SceneManager.LoadScene(2); // Might need to remove this after it's done.
    }









    public void GoToStore()
    {
        Debug.Log("Going from Stats to Store...");
        SceneManager.LoadScene(4);
    }

    public void BacktoStats()
    {
        Debug.Log("Going back to Stats...");
        SceneManager.LoadScene(3);
    }

}
