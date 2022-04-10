using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsNeed : MonoBehaviour
{
    public Text coinNeededText;
    public EnergyStation energyStation;

    public void Start()
    {
        coinNeededText = GetComponent<Text>();
        
    }
    private void Update()
    {
        coinNeededText.text = energyStation.amountOfCoinsNeeded.ToString();
    }


}
