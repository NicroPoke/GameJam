using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchBullet : MonoBehaviour
{
    [SerializeField] private int healByAmmout = 40;
    private float startTime;
    private float timeToLive = 5f;
    void Awake()
    {
        Destroy(gameObject);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent <PlayerController>().health += healByAmmout;
    }
}
