using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour {

    public void LoadGame()
    {
        SceneManager.LoadScene("Main"); //Load scene
    }
    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings"); //Load scene
    }
    public void LoadCalibrate()
    {
        SceneManager.LoadScene("Calibrate");
    }

    public void LoadMenu()
    {
 
        MatlabServer.instance.StopThread();
        SceneManager.LoadScene("Menu");
    }
}
