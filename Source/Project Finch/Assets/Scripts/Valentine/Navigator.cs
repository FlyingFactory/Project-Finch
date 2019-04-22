using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{
    /*private static Navigator _instance;
    public static Navigator Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;

            DontDestroyOnLoad(gameObject);
        
    }*/

    // I'm commenting this just in case. This seems to be the culprit.
    // I want to make it so that there's only one singleton variable for the scene.

    public void PlayGame()
    {
        SceneManager.LoadScene("Merrick");
    }

    public void GoToRoster()
    {
        SceneManager.LoadScene("Roster");
    }

    public void RostertoMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Logout()
    {
        SceneManager.LoadSceneAsync("LoginMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
        SceneManager.LoadScene("Game Quit"); // Might need to remove this after it's done.
    }

    public void LoadScene(string name)
    {
        // Use a coroutine to load the Scene in the background
        StartCoroutine(LoadAsyncScene(name));
    }

    IEnumerator LoadAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    public void GoToStore()
    {
        Debug.Log("Going from Stats to Store...");
        SceneManager.LoadScene("Store");
    }

    public void BacktoStats()
    {
        Debug.Log("Going back to Stats...");
        SceneManager.LoadScene("Roster");
    }

}
