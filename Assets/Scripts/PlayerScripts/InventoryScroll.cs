using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.InputSystem;

class InventorySlot {
    public int amount;
    public string type;

    public InventorySlot(int amount, string type)
    {
        this.amount = amount;
        this.type = type;
    }
}
public class InventoryScroll : MonoBehaviour
{
    private bool isPulled = false;
    private float cullDown = 0.35f;
    private float timeOfLastShot = 0;
    private float bulletSpeed = 4f;
    public GameObject basicGhost;
    private Vector2 mousePosition;
    private int currentSlot = 1;
    private List<InventorySlot> inventory = new List<InventorySlot>();
    private Vector2 scrollDirenction;

    void Awake()
    {
        Debug.Log("Written");
        for (int i = 0; i < 10; i++)
        {
            inventory.Add(new InventorySlot(10, "Empty"));
        }
        Debug.Log("Inventory initialized. Count: " + inventory.Count);
    }

    void Update()
    {
        if ((currentSlot + (int)scrollDirenction.y) > 9 || (currentSlot + (int)scrollDirenction.y) < 0)
        {
            currentSlot = 0;
        }
        else
        {
            currentSlot += (int)scrollDirenction.y;
        }
    }

    void ShotGhost()
    {
        if (!(inventory[currentSlot].amount <= 0))
        {
            switch (inventory[currentSlot].type)
            {
                case "Empty":
                    ShotBasic();
                    break;
            }
            inventory[currentSlot].amount--;
        }
    }

    List<Vector2> GenerateCircleVectors(int num)
    {
        List<Vector2> vectors = new List<Vector2>();
        float angleStep = 360f / num;

        for (int i = 0; i < num; i++)
        {
            float deg = i * angleStep * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(deg), Mathf.Sin(deg));
            vectors.Add(direction);
        }

        return vectors;
    }

    void ShotRound()
    {
        if (Time.time - timeOfLastShot <= cullDown)
            return;

        List<Vector2> vectors = GenerateCircleVectors(10);

        foreach (Vector2 direction in vectors)
        {
            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

            GameObject ghostBullet = Instantiate(basicGhost, transform.position, rotation);
            ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

            timeOfLastShot = Time.time;
        }
    }

    void ConsumeGhost(GameObject ghost)
    {
        Destroy(ghost);

        inventory[currentSlot].amount++; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {

            var controller = gameObject.GetComponent<BaseGhost>();

            if (isPulled)
            {
                ConsumeGhost(collision.gameObject);
            }
        }
    }

    void ShotBasic()
    {
        if (Time.time - timeOfLastShot <= cullDown)
            return;

        Vector2 direction = CreateDirectionVector(mousePosition);

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

        GameObject ghostBullet = Instantiate(basicGhost, transform.position, rotation);
        ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

        timeOfLastShot = Time.time;
    }

    Vector2 CreateDirectionVector(Vector2 mousePos)
    {
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mouseToWorld.z = 0;
        return ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;
    }

    void OnShot(InputValue input)
    {
        mousePosition = input.Get<Vector2>();

        ShotGhost();
    }

    void OnScroll(InputValue input)
    {
        scrollDirenction = input.Get<Vector2>();
    }

    void OnPull(InputValue input)
    {
        if (input.Get<Vector2>() == new Vector2(0, 0))
        {
            isPulled = false;
        }
        else
        {
            isPulled = true;
        }
    }
}
