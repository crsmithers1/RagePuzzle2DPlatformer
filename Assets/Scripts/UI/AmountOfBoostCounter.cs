using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmountOfBoostCounter : MonoBehaviour
{
   public PlayerController playerController;
    public Text boostText;
    public void Start()
    {
        boostText = GetComponent<Text>();
    }
    private void Update()
    {
        boostText.text = playerController.EnergyLeft.ToString();
    }
}
