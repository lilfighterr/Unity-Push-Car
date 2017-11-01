using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrabScript : MonoBehaviour {
    private Vector3 mousePosScreen, currentPos, previousPos;
    private Rigidbody2D carRb2d;
    private Vector3 robotPos;

    public Vector3 velocity;
    public Vector3 speed;
    public GameObject car;
    public BezierSpline spline;
    public float xForce, yForce;

    private void Start()
    {
        previousPos = transform.position;
        carRb2d = car.GetComponent<Rigidbody2D>();
        robotPos = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate () {
		if (Input.GetMouseButton(0) && GameControl.instance.isRehab == false)
        {
            mousePosScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosScreen.z = 0;
            transform.position = mousePosScreen;           
        }
        else if (GameControl.instance.isRehab == true && MatlabServer.instance.serverRunning == true)
        {
            robotPos = new Vector3(MatlabServer.instance.xMove, MatlabServer.instance.yMove, 0);
            transform.position = robotPos;
        }
        else
        {

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
        xForce = contactNormal.x * carRb2d.mass; //Force experienced by player from contact (z axis)
        yForce = contactNormal.y * carRb2d.mass; //Force experienced by player from contact (x axis)
        MatlabServer.instance.xForce = xForce;
        MatlabServer.instance.yForce = yForce;

        //Debug.DrawRay(collision.contacts[0].point, contactNormal, Color.red, 2, true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        xForce = 0;
        yForce = 0;
        MatlabServer.instance.xForce = xForce;
        MatlabServer.instance.yForce = yForce;
    }
}
