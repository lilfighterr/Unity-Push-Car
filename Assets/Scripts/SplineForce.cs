using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SplineForce : MonoBehaviour
{

    public BezierSpline splineCW;
    public BezierSpline splineCCW;
    public bool lookForward;
    public SplineWalkerMode mode;
    public GrabScript ball;
    public Text scoreText;

    private Rigidbody2D rb2d;
    private SplineRandomizer randomizerScript;
    private BezierSpline spline;
    private float progress = 0, prevProgress = 0, tempProgress = 0;
    private float velocity = 0;
    private float score = 0;
    private float lengthSum = 0;
    private bool goingForward = true;
    public int lap = 0;
    private Vector3 currentPosition;
    private bool goingBackwards = false;


    public float GetProgress
    {
        get
        {
            return progress;
        }
    }

    public void SetProgress(float value)
    {
        progress = value;
    }

    public void SetCurrentPosition(Vector3 value)
    {
        currentPosition = value;
    }

    public Vector3 GetCurrentPosition
    {
        get
        {
            return transform.localPosition;
        }
    }

    public float GetScore
    {
        get
        {
            return score;
        }
    }

    public void ResetScore()
    {
        lengthSum = 0;
    }

    public float GetLap
    {
        get
        {
            return lap;
        }
    }

    private void Start()
    {
        spline = (GameControl.instance.isCW) ? splineCW : splineCCW;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
        transform.localPosition = spline.GetPoint(0);
        currentPosition = transform.localPosition;
        randomizerScript = spline.GetComponent<SplineRandomizer>();
    }

    private void Update()
    {
        //Move method
        if (!GameControl.instance.gameStart || GameControl.instance.gameOver || goingBackwards) //Prevents car from moving during countdown or gameover
        {
            transform.localPosition = currentPosition;
        }
        else
        {
            MoveBySnappingToCurve();
        }

        UpdateScore();
        CarRotation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contactNormal = collision.contacts[0].normal.normalized; //Gives unit vector direction normal to point of contact
        float dotProduct = Vector3.Dot(contactNormal, spline.GetDirection(progress));

        if (!goingBackwards)
        {
            currentPosition = transform.localPosition;
        }
        goingBackwards = (dotProduct < 0) ? true : false;
        //Debug.DrawRay(spline.GetPoint(progress), spline.GetDirection(progress)*dotProduct, Color.blue, 2, true);
        //Debug.Log(collision.relativeVelocity.magnitude); 
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Force = collision normal * body mass?
        //Move the car depending on how much the ball is moving
        //Vector3 contactNormal = collision.contacts[0].normal.normalized; //Gives unit vector direction normal to point of contact
        //float dotProduct = Vector3.Dot(contactNormal, spline.GetDirection(progress));

        //Vector3 projection = dotProduct*spline.GetDirection(progress);



        //Vector3 force = Vector3.Dot(ball.velocity, spline.GetDirection(progress))*spline.GetDirection(progress);

        //Debug.Log("direction: "+spline.GetDirection(progress)*dotProduct+"Force: "+force);
        //rb2d.AddForce(force, ForceMode2D.Force);

        //Debug.DrawRay(spline.GetPoint(progress), projection, Color.blue, 2, true);

    }

    private void CarRotation()
    {
        //Used to make sure sprite is looking at the right direction
        if (lookForward)
        {
            Vector3 diff = spline.GetDirection(progress);

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        }
    }

    private void MoveBySnappingToCurve()
    {
        //Moves the sprite to position
        progress = spline.ClosestTimeOnBezier(transform.position); //Finds at what point t [0,1] the car is at 
        Vector3 position = spline.GetPoint(progress); // Finds the location on the spline t is
    
        if ((prevProgress - progress) > 0.9 && mode == SplineWalkerMode.Loop) //If there's a big jump in progress (going from current loop to the next)
        {
            lap++;
            lengthSum += spline.SplineLength(); //total length
            if (GameControl.instance.randomize)
            {
                randomizerScript.Randomize(true);
            }
        }
        if (position != transform.position) //If the position is not equal to current position, move to position
        {
            transform.localPosition = position;
            prevProgress = progress;
        }
    }

    private void UpdateScore()
    {
        score = lengthSum + spline.CurrentLength(progress);
        scoreText.text = " Score \n" + score.ToString("F2");
    }
}