using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    public AudioSource source;
    public AudioClip coinCollect;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CoinCounter.coinAmount += 1;
            source.clip = coinCollect;
            source.Play();
            Destroy(gameObject);
        }
    }
}
