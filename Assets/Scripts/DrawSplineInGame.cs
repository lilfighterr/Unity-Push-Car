using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSplineInGame : MonoBehaviour {
    public GameObject spline;
    public GameObject Car;

    private BezierSpline splineScript;
    private SplineForce carScript;
    private float drawDistance = 0.1f; //Distance in progress ([0,1]) 
    private float drawDuration = 0.05f;//How long the line will be drawn for
    private int steps = 10; //# of lines to be drawn
    

	// Use this for initialization
	void Start () {
        //Grab scripts of spline & car
        splineScript = spline.GetComponent<BezierSpline>();
        carScript = Car.GetComponent<SplineForce>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GameControl.instance.drawLine)
        {
            DrawSpline();
        }
	}

    private void DrawSpline()
    {
        float start = carScript.GetProgress; //Start point
        float step = drawDistance / steps; //Size of step increment
        float next = start + step; //next point to draw to

        for (int i = 0; i < steps; i++)
        {
            DrawLine(splineScript.GetPoint(start), splineScript.GetPoint(next), Color.green, drawDuration);
            start = next;
            next += step;
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f) //Got script from stackoverflow
    {
        GameObject myLine = new GameObject();
        myLine.transform.parent = transform;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.sortingLayerName = "Foreground";
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
