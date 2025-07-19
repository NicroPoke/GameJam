using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    [HideInInspector] public Animator animator;
    bool isTrue;
    void Awake()
    {
        isTrue = true;
        animator = GetComponent<Animator>();
        startTime = Time.time;
    }

    void Update()
    {
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("IdleNormalBullet"));
        if (Time.time - startTime >= timeToLive)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var controller = collision.gameObject.GetComponent<PlayerController>();
            controller.TakeDamege(15);
            
            Destroy(gameObject);
        }
    }
}
