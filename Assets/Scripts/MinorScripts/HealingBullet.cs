using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;

public class HealingBullet : MonoBehaviour
{
    [SerializeField] private int healByAmmout = 40;
    private float startTime;
    private float timeToLive = 5f;
    void Awake()
    {
        Destroy(gameObject);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().health += healByAmmout;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossHandler>().TakeDamege(7);
        }
        Destroy(gameObject);

    }
}
