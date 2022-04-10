using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectEnergy : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        playerController.IncreaseEnergy();
        Destroy(gameObject);
    }
}