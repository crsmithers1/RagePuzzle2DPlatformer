using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float startFallTimer;
    private float fallTimer;
    public float fallSpeed;
    public PlayerController playerController;
    public Rigidbody2D fallingRb;
    private bool fallPlatform;


    private void Start()
    {
        fallTimer = startFallTimer;
        fallingRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        StartFall();
    }

    private void StartFall()
    {
        if (playerController.isPlatform && fallPlatform)
        {
        fallTimer -= Time.deltaTime;
        }
        else
        {
            fallTimer = startFallTimer;
        }

        if (fallTimer <= 0)
        {
            fallingRb.velocity = new Vector2 (0, -fallSpeed);
            gameObject.transform.parent = null;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        col.gameObject.transform.SetParent(gameObject.transform, true);
        fallPlatform = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        col.gameObject.transform.parent = null;
        fallPlatform = false;
    }
}
