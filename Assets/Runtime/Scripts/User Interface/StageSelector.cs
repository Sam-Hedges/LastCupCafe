using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for switching to different levels(scenes) in the game
/// </summary>
public class StageSelector : MonoBehaviour
{

    /// <summary>
    /// Methods to select different levels or return to main menu
    /// </summary>
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Mainlevel()
    {
        SceneManager.LoadScene(1);
    }    
}
