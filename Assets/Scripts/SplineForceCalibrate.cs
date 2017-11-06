using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SplineForceCalibrate : MonoBehaviour
{

    public GrabScript ball;

    private Rigidbody2D rb2d;
    private float velocity = 0;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contactNormal = collision.contacts[0].normal.normalized; //Gives unit vector direction normal to point of contact
        float dotProduct = Vector3.Dot(contactNormal, transform.right);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector3 force = Vector3.Dot(ball.velocity, transform.right) * transform.right;
        rb2d.AddForce(force, ForceMode2D.Force);
    }

}