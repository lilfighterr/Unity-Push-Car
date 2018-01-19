using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Allows to reset the game
using UnityEngine.UI; //To allow for declaring a new public text variable

public class GameControl : MonoBehaviour
{

    public static GameControl instance; // Allows easy access from other scripts. They just have to do GameControl.instance.something

    public Text scoreText;
    public Text countdownText;
    public Text timeText;
    public Text gameOverText;
    public GameObject calibrateButton;
    public GameObject car;
    public GameObject handleOffset;
    public GameObject characterController;
    public SpriteRenderer blinker;
    public bool gameOver = false;
    public bool isRehab = false;
    public bool randomize = false;
    public bool drawLine = false;
    public bool forceFeedback = false;
    public bool gameStart = false;
    public bool isCW = true;
    public bool handleToggle = false;
    public SceneName sceneIndex;
    public int evalType = 0;
    public float blinkTimerOn = 2f;
    public float blinkTimerOff = 2f;

    private SplineForce carScript;
    private float timeLeft = 4.5f;
    private float viewedTime;
    private float timeElapsed = 0;
    private float goal;
    private float blinkerTime;
    

    // Use this for initialization
    void Awake()
    { //Always called before start() functions
      //Makes sure that there is only one instance of GameControl (singleton)
        if (instance == null) //If no game control found
        {
            sceneIndex = (SceneName)SceneManager.GetActiveScene().buildIndex;
            instance = this; //Then this is the instance of the game control
            isRehab = PlayerPrefs.GetInt("RehabToggle", 0) == 1 ? true : false;
            randomize = PlayerPrefs.GetInt("RandomizeToggle", 1) == 1 ? true : false;
            forceFeedback = PlayerPrefs.GetInt("ForceToggle", 1) == 1 ? true : false;
            isCW = PlayerPrefs.GetInt("DirectionToggle", 1) == 1 ? true : false;
            handleToggle = PlayerPrefs.GetInt("HandleToggle", 1) == 1 ? true : false;
            evalType = PlayerPrefs.GetInt("EvalType", 0);
            DetermineCharacter();
            goal = (evalType == 0) ? PlayerPrefs.GetFloat("Length", 40f) : PlayerPrefs.GetFloat("Time", 30f);
            if (sceneIndex == SceneName.Main) calibrateButton.SetActive(isRehab); //If main game, turn on blinker
            blinkerTime = blinkTimerOn;
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
        carScript = car.GetComponent<SplineForce>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (sceneIndex == SceneName.Main && !gameOver)
        {
            if (!gameStart) //During countdown
            {
                timeLeft -= Time.deltaTime;
                viewedTime = timeLeft - 1;
                if (viewedTime < -1)
                {
                    gameStart = true;
                    countdownText.enabled = false;
                }
                else if (viewedTime > 0.5)
                {
                    countdownText.text = viewedTime.ToString("F0");

                }
                else
                {
                    countdownText.text = "GO!";
                }
            }
            else //When game starts
            {
                blinkerTime -= Time.deltaTime; //For blinker
                if (blinkerTime < 0 && blinker.enabled)
                {
                    blinker.enabled = false;
                    blinkerTime = blinkTimerOff;
                }
                else if (blinkerTime < 0 && !blinker.enabled)
                {
                    blinker.enabled = true;
                    blinkerTime = blinkTimerOn;
                }
                timeElapsed += Time.deltaTime; //For time score
                timeText.text = "Time \n" + timeElapsed.ToString("F1");
            }

            CheckGameEnd();
        }
    }

    public void Restart()
    {
        //carScript.SetProgress(0);
        //carScript.ResetScore();
        if (isRehab)
        {
            MatlabServer.instance.StopThread();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reload current scene to restart
    }
    public void Calibrate()
    {
        if (GameControl.instance.isRehab)
        {
            MatlabServer.instance.StopThread();
        }
        SceneManager.LoadScene("Calibrate"); //Load scene
    }

    private void CheckGameEnd()
    {
        if (evalType == 0) //Length
        {
            if (carScript.GetScore >= goal)
            {
                GameOver();
            }
        }
        else if (evalType == 1) //Time
        {
            if (timeElapsed >= goal)
            {
                GameOver();
            }
        }
        else
        {

        }
    }

    private void GameOver()
    {
        gameOver = true;
        carScript.SetCurrentPosition(carScript.GetCurrentPosition); //Stop it at current position
        car.GetComponent<Rigidbody2D>().velocity = Vector2.zero; //Remove any velocity
        gameOverText.enabled = true;
    }

    private void DetermineCharacter()
    {
        if (handleToggle)
        {
            handleOffset.SetActive(true);
            characterController.SetActive(false);
        }
        else
        {
            handleOffset.SetActive(false);
            characterController.SetActive(true);
        }
    }

}
