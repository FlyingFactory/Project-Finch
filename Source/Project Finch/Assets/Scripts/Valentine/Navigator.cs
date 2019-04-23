using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Proyecto26;
using System.Threading;

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

    public async void SearchForGame()
    {
        if (!MenuView.PlayerAccount.currentPlayer.InMatch)
        {
            UserQueue new_queue = new UserQueue(MenuView.PlayerAccount.currentPlayer.userName);
            RestClient.Put("https://project-finch-database.firebaseio.com/queuingForMatch/" + MenuView.PlayerAccount.currentPlayer.userName + ".json", new_queue);
            //SceneManager.LoadScene("Merrick");
        }

        while (!MenuView.PlayerAccount.currentPlayer.InMatch)
        {
            MenuView.PlayerAccount.loadDataAndLoadSoldierInfo new_instance = new MenuView.PlayerAccount.loadDataAndLoadSoldierInfo();
            new_instance.loadDataInfo.output = MenuView.PlayerAccount.currentPlayer;
            new_instance.loadDataInfo.userID = MenuView.PlayerAccount.currentPlayer.userName;

            Thread loadDataThread = new Thread(new ParameterizedThreadStart(MenuView.PlayerAccount.LoadData_Thread));
            loadDataThread.Start(new_instance);

            System.Threading.CancellationToken cancel3 = new CancellationToken();
            for (int i = 0; i < 10; i++)
            {
                if (new_instance.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel3);
                if (cancel3.IsCancellationRequested) break;
            };

            if (new_instance.loadDataInfo.output == null)
            {
                Debug.Log("Load data unsuccessful");
            }
            else
            {  
                MenuView.PlayerAccount.currentPlayer = new_instance.loadDataInfo.output;
            }
        }
        Debug.Log("found Game!, MatchID: "+ MenuView.PlayerAccount.currentPlayer.matchID);

        if (MenuView.PlayerAccount.currentPlayer.matchID != -1) {
            /* Information needed to start game properly
             * 
             */
            SceneManager.LoadSceneAsync("Merrick");
        }


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
