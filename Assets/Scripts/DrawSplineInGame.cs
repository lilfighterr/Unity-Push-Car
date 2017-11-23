using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DrawSplineInGame : MonoBehaviour {
    public GameObject spline;
    public GameObject Car;

    private BezierSpline splineScript;
    private SplineForce carScriptForce;
    private SplineWalker carScriptWalker;
    private float drawDistance = 0.1f; //Distance in progress ([0,1]) 
    private float drawDuration = 0.05f;//How long the line will be drawn for
    private int steps = 100; //# of lines to be drawn
    private int drawLine = 1;
    private bool inSettings;
    private GameObject[] lineArray;
    private bool drawnInitially = false;
    
    

	// Use this for initialization
	void Start () {
        //Grab scripts of spline & car
        splineScript = spline.GetComponent<BezierSpline>();
        
        if (SceneManager.GetActiveScene().name == "Settings")
        {
            carScriptWalker = Car.GetComponent<SplineWalker>();
            inSettings = true;
        }
        else
        {
            carScriptForce = Car.GetComponent<SplineForce>();
            inSettings = false;
        }

        drawLine = PlayerPrefs.GetInt("ShowLineToggle", 1);
        drawDistance = PlayerPrefs.GetFloat("DrawDistance", 0.1f);

    }

    // Update is called once per frame
    void Update()
    {
        if (!drawnInitially && drawLine == 1) //Draw line initially, filling the array
        {
            lineArray = new GameObject[steps];
            DrawSpline();
            drawnInitially = true;
        }
        if (drawLine == 1) //Move the lines in the array instead of creating/destroying more lines
        {
            DrawSpline();
        }
    }

    public void ToggleLine(bool value) //To be used by toggle in settings
    {
        drawLine = value ? 1 : 0;
        gameObject.SetActive(value);
    }

    public void ChangeDrawDistance(float value) //To be used by toggle in settings
    {
        drawDistance = value;
    }

    private void DrawSpline()
    {
        
        float start = (inSettings) ? carScriptWalker.GetProgress :carScriptForce.GetProgress; //Start point
        float step = drawDistance / steps; //Size of step increment
        float next = start + step; //next point to draw to

        for (int i = 0; i < steps; i++)
        {
            if (!drawnInitially)
            {
                DrawLine(splineScript.GetPoint(start), splineScript.GetPoint(next), Color.gray, i);
            }
            else
            {
                lineArray[i].GetComponent<LineRenderer>().SetPosition(0, splineScript.GetPoint(start));
                lineArray[i].GetComponent<LineRenderer>().SetPosition(1, splineScript.GetPoint(next));
            }
            start = next;
            next += step;
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, int index) //Got script from stackoverflow
    {
        GameObject myLine = new GameObject();
        myLine.transform.parent = transform;
        myLine.transform.position = start;
        myLine.name = index.ToString();
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.sortingLayerName = "Foreground";
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lineArray[index] = myLine;
    }
}
