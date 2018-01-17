using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrabScript : MonoBehaviour {

    public Vector3 velocity;
    public Vector3 speed;
    public GameObject car;
    public BezierSpline spline;
    public float xForce, yForce;
    public float collisionStatus;

    private Vector3 mousePosScreen, currentPos, previousPos;
    private Rigidbody2D carRb2d;
    private Vector3 robotPos;
    private float T11 = 1, T12 = 0, T13 = 1, T21 = 0, T22 = 1, T23 = 1, T31 = 1, T32 = 1, T33 = 1;
    private float a = 0, b = 0, c = 0, d = 0;
    private bool forceFeedback;

    private void Start()
    {
        Calibrate();
        previousPos = transform.position;
        carRb2d = car.GetComponent<Rigidbody2D>();
        robotPos = Vector3.zero;
        forceFeedback = GameControl.instance.forceFeedback;
    }

    public void Calibrate() //Initialize
    {
        try
        {
            T11 = PlayerPrefs.GetFloat("T11");
            T12 = PlayerPrefs.GetFloat("T12");
            T13 = PlayerPrefs.GetFloat("T13");

            T21 = PlayerPrefs.GetFloat("T21");
            T22 = PlayerPrefs.GetFloat("T22");
            T23 = PlayerPrefs.GetFloat("T23");

            T31 = PlayerPrefs.GetFloat("T31");
            T32 = PlayerPrefs.GetFloat("T32");
            T33 = PlayerPrefs.GetFloat("T33");
            /*
            a = PlayerPrefs.GetFloat("a");
            b = PlayerPrefs.GetFloat("b");
            c = PlayerPrefs.GetFloat("c");
            d = PlayerPrefs.GetFloat("d");*/
        }
        catch
        {
            Debug.Log("Please Calibrate");
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
		if (Input.GetMouseButton(0) && GameControl.instance.isRehab == false)
        {
            mousePosScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosScreen.z = 0;
            transform.parent.transform.position = mousePosScreen;    //just transform.position if no handle      
        }
        else if (GameControl.instance.isRehab == true && MatlabServer.instance.serverRunning == true)
        {
            transform.parent.transform.position = CalibratedMovement();
        }
        else
        {
            //Do Nothing
        }
        velocity = (transform.position - previousPos) / Time.deltaTime;
        speed = new Vector3(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), 0);

        previousPos = transform.position;
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 onLine = spline.OnLine(transform.position);
            if (onLine == Vector3.zero)
            {
                Debug.Log("already on line");
            }
            else
            {
                transform.position = onLine;
            }
            Debug.Log(spline.SplineLength());
        }
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
        //While in contact with ball
        Vector3 contactNormal = collision.contacts[0].normal.normalized; //Gives unit vector direction normal to point of contact
        if (forceFeedback)
        {
            xForce = contactNormal.x * carRb2d.mass; //Force experienced by player from contact (z axis)
            yForce = contactNormal.y * carRb2d.mass; //Force experienced by player from contact (x axis)
        }
        else
        {
            xForce = 0;
            yForce = 0;
        }
        MatlabServer.instance.collisionStatus = 1f;
        MatlabServer.instance.xForce = xForce;
        MatlabServer.instance.yForce = yForce;

        //Debug.DrawRay(collision.contacts[0].point, contactNormal, Color.red, 2, true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        xForce = 0;
        yForce = 0;
        MatlabServer.instance.collisionStatus = 0f;
        MatlabServer.instance.xForce = xForce;
        MatlabServer.instance.yForce = yForce;
    }

    public Vector3 CalibratedMovement()
    {
        // Ps = T*Pr
        float xS = MatlabServer.instance.xMove * T11 + MatlabServer.instance.yMove * T12;
        float yS = MatlabServer.instance.xMove * T21 + MatlabServer.instance.yMove * T22;
        /*
        float xS = MatlabServer.instance.xMove * a - MatlabServer.instance.yMove * b + c;
        float yS = MatlabServer.instance.xMove * b + MatlabServer.instance.yMove * a + d;
        */
        return new Vector3(xS, yS, 0f);
    }
}
