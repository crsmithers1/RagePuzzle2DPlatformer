using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{
    public Text coinText;
    public static int coinAmount;

    public void Start()
    {
        coinText = GetComponent<Text>();
    }

    private void Update()
    {
        coinText.text = coinAmount.ToString();
    }

    public void CoinReset()
    {
        coinAmount = 0;
    }
}
