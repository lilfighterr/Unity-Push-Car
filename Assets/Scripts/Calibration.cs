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
    public GameObject instructionsText;
    public GameObject characterObject;

    private int spacePressed = 0;
    private GrabScript characterScript;
    private Vector3 aR, bR, cR; //Robot positions
    private Vector3 aS, bS, cS; //Screen positions
    
    private void Start()
    {
        calibrationCrosshair.transform.position = new Vector3(-4f, -2f, 0);
        characterScript = characterObject.GetComponent<GrabScript>();
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
                    instructionsText.SetActive(false);
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
                    characterScript.Calibrate();
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
        
         float[,] screen = { {aS.x, bS.x, cS.x },
                              {aS.y, bS.y, cS.y },
                              {1, 1, 1 } };
         float[,] robot = { {aR.x, bR.x, cR.x },
                             {aR.y, bR.y, cR.y },
                             {1, 1, 1 } };
        
        /*double[,] screen = { {12, 17, 16 },
                             {10, 13, 6 },
                             {1, 1, 1 } };
        double[,] robot = { {-24.4165, 26.8797, 14.0124},
                            {-12.3001, -43.5058, 21.4809 },
                            {1, 1, 1 } };*/

        Matrix<float> robotMatrix = Matrix<float>.Build.DenseOfArray(robot);
        Matrix<float> screenMatrix = Matrix<float>.Build.DenseOfArray(screen);
        Matrix<float> transformMatrix = screenMatrix * robotMatrix.Inverse();


        PlayerPrefs.SetFloat("T11", transformMatrix[0, 0]);
        PlayerPrefs.SetFloat("T12", transformMatrix[0, 1]);
        PlayerPrefs.SetFloat("T13", transformMatrix[0, 2]);

        PlayerPrefs.SetFloat("T21", transformMatrix[1, 0]);
        PlayerPrefs.SetFloat("T22", transformMatrix[1, 1]);
        PlayerPrefs.SetFloat("T23", transformMatrix[1, 2]);

        PlayerPrefs.SetFloat("T31", transformMatrix[2, 0]);
        PlayerPrefs.SetFloat("T32", transformMatrix[2, 1]);
        PlayerPrefs.SetFloat("T33", transformMatrix[2, 2]);
    }

}