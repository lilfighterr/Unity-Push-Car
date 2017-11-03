using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Allows to reset the game
using UnityEngine.UI; //To allow for declaring a new public text variable

public class Calibration : MonoBehaviour
{

    public static Calibration instance; // Allows easy access from other scripts. They just have to do GameControl.instance.something
    public GameObject calibrationCrosshair;
    public GameObject doneText;
    public GameObject carObject;

    private int spacePressed = 0;
    private double bottomLimit, topLimit;
    private double crossHairBottom, crossHairTop;
    private GrabScript carScript;

    private void Start()
    {
        carScript = carObject.GetComponent<GrabScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            spacePressed++;
            if (spacePressed == 1) //First time space got pressed
            {
                bottomLimit = MatlabServer.instance.yMove;
                crossHairBottom = calibrationCrosshair.transform.position.y;
                calibrationCrosshair.transform.position = new Vector2(calibrationCrosshair.transform.position.x, 3.85f);
            }
            else if (spacePressed == 2)
            {
                topLimit = MatlabServer.instance.yMove;
                crossHairTop = calibrationCrosshair.transform.position.y;
                calibrationCrosshair.SetActive(false);

                Calibrate(bottomLimit, topLimit, crossHairBottom, crossHairTop);
                carScript.Calibrate();
                doneText.SetActive(true);
            }
            else
            {
                if (GameControl.instance.isRehab)
                {
                    MatlabServer.instance.StopThread();
                }
                SceneManager.LoadScene("Main");    
            }
        }

    }

    private void Calibrate(double rehabBot, double rehabTop, double screenBot, double screenTop)
    {
        double rangeRehab = rehabTop - rehabBot;
        double rangeScreen = screenTop - screenBot;

        PlayerPrefs.SetFloat("rangeRehab", (float)rangeRehab);
        PlayerPrefs.SetFloat("rangeScreen", (float)rangeScreen);
        PlayerPrefs.SetFloat("rehabTop", (float)rehabTop);
        PlayerPrefs.SetFloat("screenTop", (float)screenTop);
    }

}