using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxEnergyUI : MonoBehaviour
{
    public Text maxEnergyText;
    public ControlCenter controlCenter;

    public void Start()
    {
        maxEnergyText = GetComponent<Text>();

    }
    private void Update()
    {
        maxEnergyText.text = controlCenter.maxEnergy.ToString();
    }


}
