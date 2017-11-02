using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Allows to reset the game
using UnityEngine.UI; //To allow for declaring a new public text variable

public class GameControl : MonoBehaviour
{

    public static GameControl instance; // Allows easy access from other scripts. They just have to do GameControl.instance.something

    public Text scoreText;
    public bool gameOver = false;
    public bool isRehab = false;
    public bool randomize = false;
    public bool drawLine = false;

    private int score = 0;


    // Use this for initialization
    void Awake()
    { //Always called before start() functions
      //Makes sure that there is only one instance of GameControl (singleton)
        if (instance == null) //If no game control found
        {
            instance = this; //Then this is the instance of the game control
        }
        else if (instance != this) //If the game object finds that instance is already on another game object, then this destroys itself as it's not needed
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (isRehab && MatlabServer.instance.serverRunning == false)
        {
            MatlabServer.instance.StartThread();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            if (isRehab)
            {
                MatlabServer.instance.StopThread();
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reload current scene to restart
        }
    }

}
