using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Allows to reset the game
using UnityEngine.UI; //To allow for declaring a new public text variable
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;



public class Calibration : MonoBehaviour
{

    public static Calibration instance; // Allows easy access from other scripts. They just have to do GameControl.instance.something
    public GameObject calibrationCrosshair;
    public GameObject doneText;
    public GameObject instructionsText;
    public GameObject characterObject;

    private int spacePressed = 0;
    private GrabScript characterScript;
    private Vector3 aR, bR, cR, dR; //Robot positions
    private Vector3 aS, bS, cS, dS; //Screen positions
    private Vector2 firstPos = new Vector2(-7, -2);
    private Vector2 secondPos = new Vector2(-7, 2);
    private Vector2 thirdPos = new Vector2(7, 2);
    private Vector2 fourthPos = new Vector2(7, -2);
    private Vector2 offset = new Vector3(0f, 2.2f);

    
    private void Start()
    {
        firstPos = (GameControl.instance.handleToggle) ? firstPos + offset : firstPos;
        secondPos = (GameControl.instance.handleToggle) ? secondPos + offset : secondPos;
        thirdPos = (GameControl.instance.handleToggle) ? thirdPos + offset : thirdPos;
        fourthPos = (GameControl.instance.handleToggle) ? fourthPos + offset : fourthPos;
        calibrationCrosshair.transform.position = firstPos;
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
                    calibrationCrosshair.transform.position = secondPos; //Move crosshair to 2nd pos
                    instructionsText.SetActive(false);
                    break;
                case 2:
                    bR = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0); //record 2nd robot pos
                    bS = new Vector3(calibrationCrosshair.transform.position.x, calibrationCrosshair.transform.position.y, 0); //record 2nd screen pos
                    calibrationCrosshair.transform.position = thirdPos; //Move crosshair to 3rd pos
                    break;
                case 3:
                    cR = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0); //record 3nd robot pos
                    cS = new Vector3(calibrationCrosshair.transform.position.x, calibrationCrosshair.transform.position.y, 0); //record 3nd screen pos
                    calibrationCrosshair.transform.position = fourthPos; //Move crosshair to 4th pos
                    break;
                case 4:
                    dR = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0); //record 3nd robot pos
                    dS = new Vector3(calibrationCrosshair.transform.position.x, calibrationCrosshair.transform.position.y, 0); //record 3nd screen pos
                    calibrationCrosshair.SetActive(false);
                    Calibrate();
                    characterScript.Calibrate();
                    doneText.SetActive(true);
                    break;
                default:
                    //Do nothing
                    break;
            }
        }

    }

    public void Recalibrate()
    {
        calibrationCrosshair.SetActive(true);
        doneText.SetActive(false);
        instructionsText.SetActive(true);
        spacePressed = 0;
        calibrationCrosshair.transform.position = new Vector3(-4f, -2f, 0);
    }

    private void Calibrate()
    {
        /*
         float[,] screen = { {aS.x, bS.x, cS.x },
                              {aS.y, bS.y, cS.y },
                              {1, 1, 1 } };
         float[,] robot = { {aR.x, bR.x, cR.x },
                             {aR.y, bR.y, cR.y },
                             {1, 1, 1 } };

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
        PlayerPrefs.SetFloat("T33", transformMatrix[2, 2]);*/

        // Ps = A*Pr
        /*
        float[,] screen = { {aS.x}, 
                        {aS.y}, 
                        {bS.x}, 
                        {bS.y} };
        float[,] robot = { {aR.x, -aR.y, 1, 0 }, 
                        {aR.y, aR.x, 0, 1 }, 
                        {bR.x, -bR.y, 1, 0 }, 
                        {bR.y, bR.x, 0, 1 } };


        Matrix<float> Ps = Matrix<float>.Build.DenseOfArray(screen);
        Matrix<float> Pr = Matrix<float>.Build.DenseOfArray(robot);
        Matrix<float> A = Pr.Inverse() * Ps;

        PlayerPrefs.SetFloat("a", A[0, 0]);
        PlayerPrefs.SetFloat("b", A[1, 0]);
        PlayerPrefs.SetFloat("c", A[2, 0]);
        PlayerPrefs.SetFloat("d", A[3, 0]);
        */

        //Homography DLT method
        
        Matrix<float> point1 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(aR, aS));
        Matrix<float> point2 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(bR, bS));
        Matrix<float> point3 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(cR, cS));
        Matrix<float> point4 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(dR, dS));
        
        /*Matrix<float> point1 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(new Vector3(141.5360f, 175.3260f, 0), new Vector3(0,0,0)));
        Matrix<float> point2 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(new Vector3(293.248f, 142.6483f, 0), new Vector3(990, 0,0)));
        Matrix<float> point3 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(new Vector3(295.4956f, 219.9856f, 0), new Vector3(990, 400,0)));
        Matrix<float> point4 = Matrix<float>.Build.DenseOfArray(CreateCorrespondencePointsForH(new Vector3(144.9074f, 261.3775f, 0), new Vector3(0, 400,0)));*/
        Matrix<float> combinedPoints = point1.Stack(point2).Stack(point3).Stack(point4);
        Svd<float> svd = combinedPoints.Svd();
        Matrix<float> Vmatrix = -svd.VT.Transpose();

        float[,] Hmatrix = { { Vmatrix[0, 8], Vmatrix[1, 8], Vmatrix[2, 8] },
                            { Vmatrix[3, 8], Vmatrix[4, 8], Vmatrix[5, 8]},
                            { Vmatrix[6, 8], Vmatrix[7, 8], Vmatrix[8, 8]} };

        PlayerPrefs.SetFloat("T11", Vmatrix[0, 8]);
        PlayerPrefs.SetFloat("T12", Vmatrix[1, 8]);
        PlayerPrefs.SetFloat("T13", Vmatrix[2, 8]);

        PlayerPrefs.SetFloat("T21", Vmatrix[3, 8]);
        PlayerPrefs.SetFloat("T22", Vmatrix[4, 8]);
        PlayerPrefs.SetFloat("T23", Vmatrix[5, 8]);

        PlayerPrefs.SetFloat("T31", Vmatrix[6, 8]);
        PlayerPrefs.SetFloat("T32", Vmatrix[7, 8]);
        PlayerPrefs.SetFloat("T33", Vmatrix[8, 8]);


    }

    private float[,] CreateCorrespondencePointsForH(Vector3 robotPoint, Vector3 screenPoint)
    {
        float[,] pointHomography = { {-robotPoint.x, -robotPoint.y, -1, 0, 0, 0, robotPoint.x*screenPoint.x, robotPoint.y*screenPoint.x, screenPoint.x },
                                        {0, 0, 0, -robotPoint.x, -robotPoint.y, -1, robotPoint.x*screenPoint.y, robotPoint.y*screenPoint.y, screenPoint.y} };
        return pointHomography;
    }


}