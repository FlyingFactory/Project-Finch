﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2); // Might need to remove this after it's done.
    }

}
