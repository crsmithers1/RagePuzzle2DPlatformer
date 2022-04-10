using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnergyStation : MonoBehaviour
{
    public float amountOfCoinsNeeded;
    public PlayerController playerController;
    public AudioSource source;
    public AudioClip clip;

    public void Awake()
    {
        CoinCounter.coinAmount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CoinCounter.coinAmount >= amountOfCoinsNeeded)
        {
        playerController.IncreaseEnergyToMax();
        source.clip = clip;
        source.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }
}
