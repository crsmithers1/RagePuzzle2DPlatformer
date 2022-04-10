using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlCenter : MonoBehaviour
{
    public CoinCounter coinCounter;
    public CountdownTimer countdownTimer;
    public PlayerController playerController;
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        coinCounter.CoinReset();
        playerController.IncreaseEnergyToMax();
        countdownTimer.IncreaseAmountOfTries();
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
        coinCounter.CoinReset();
        playerController.IncreaseEnergyToMax();
    }
}
