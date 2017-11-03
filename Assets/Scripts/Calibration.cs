using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Allows to reset the game
using UnityEngine.UI; //To allow for declaring a new public text variable
using MathNet.Numerics.LinearAlgebra;



public class Calibration : MonoBehaviour
{

    public static Calibration instance; // Allows easy access from other scripts. They just have to do GameControl.instance.something
    public GameObject calibrationCrosshair;
    public GameObject doneText;
    public GameObject carObject;

    private int spacePressed = 0;
    private GrabScript carScript;
    private Vector3 aR, bR, cR; //Robot positions
    private Vector3 aS, bS, cS; //Screen positions
    
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
            switch (spacePressed)
            {
                case 1:
                    aR = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0); //record 1st robot pos
                    aS = new Vector3(calibrationCrosshair.transform.position.x, calibrationCrosshair.transform.position.y, 0); //record 1st screen pos
                    calibrationCrosshair.transform.position = new Vector2(0, 3f); //Move crosshair to 2nd pos
                    break;
                case 2:
                    bR = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0); //record 2nd robot pos
                    bS = new Vector3(calibrationCrosshair.transform.position.x, calibrationCrosshair.transform.position.y, 0); //record 2nd screen pos
                    calibrationCrosshair.transform.position = new Vector2(4f, 0f); //Move crosshair to 2nd pos
                    break;
                case 3:
                    cR = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0); //record 3rd robot pos
                    cS = new Vector3(calibrationCrosshair.transform.position.x, calibrationCrosshair.transform.position.y, 0); //record 3rd screen pos
                    calibrationCrosshair.SetActive(false);
                    Calibrate();
                    //carScript.Calibrate();
                    doneText.SetActive(true);
                    break;
                default:
                    if (GameControl.instance.isRehab)
                    {
                        MatlabServer.instance.StopThread();
                    }
                    SceneManager.LoadScene("Main");
                    break;
            }
        }

    }

    private void Calibrate()
    {
        Matrix<double> robotMatrix = 
        /*PlayerPrefs.SetFloat("rangeRehab", (float)rangeRehab);
        PlayerPrefs.SetFloat("rangeScreen", (float)rangeScreen);
        PlayerPrefs.SetFloat("rehabTop", (float)rehabTop);
        PlayerPrefs.SetFloat("screenTop", (float)screenTop);*/
    }

}