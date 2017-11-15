using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadScenes : MonoBehaviour {

    public void LoadGame()
    {
        try
        {
            if (GameControl.instance.isRehab)
            {
                MatlabServer.instance.StopThread();
            }
        }
        catch
        {
            //SceneManager.LoadScene("Main");
        }
        SceneManager.LoadScene("Main"); 
    }
    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void LoadCalibrate()
    {
        SceneManager.LoadScene("Calibrate");
    }

    public void LoadMenu()
    {
        if (GameControl.instance.isRehab)
        {
            MatlabServer.instance.StopThread();
        }
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
        Application.Quit();
    }
}
