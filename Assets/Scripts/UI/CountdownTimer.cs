using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining;
    public float amountOfTries;
    public bool timerIsRunning = false;
    public Text timeText;
    public ControlCenter controlCenter;
    public PlayerController playerController;

    public void Awake()
    {
        timeRemaining = controlCenter.levelTime;
        if (playerController.devMode == false)
        {
        timerIsRunning = true;
        }
        else if(playerController.devMode == true)
        {
            timerIsRunning = false;
        }
    }
    public void Update()
    {
        DisplayTime(timeRemaining);

        if (timerIsRunning)
        {
            if(timeRemaining < 5 && timeRemaining > 0)
            {
                GetComponent<Text>().color = Color.red;
            }
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                IncreaseAmountOfTries();
                controlCenter.ResetScene();
            }
        }
    }
    public void IncreaseAmountOfTries()
    {
        amountOfTries++;
    }
    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}  :  {1:00}", minutes, seconds);
    }
}