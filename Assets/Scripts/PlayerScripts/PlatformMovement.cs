using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Rigidbody2D rbody;
    public bool isOnPlatform;
    private Rigidbody2D platformRBody;
    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isOnPlatform)
        {
            rbody.velocity = rbody.velocity + platformRBody.velocity;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "MovingPlatform")
        {
            platformRBody = col.gameObject.GetComponent<Rigidbody2D>();
            isOnPlatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "MovingPlatform")
        {
            isOnPlatform = false;
            platformRBody = null;
        }
    }
}