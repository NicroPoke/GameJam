using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
    private bool isOverheat;
    private float overheatValueRecoveryRate = 4f;
    private float overheatValueChagneRate = 2f;
    private float overheatValue = 0;
    private float timerAnalogue = 0;
    public float maxRange = 5;
    private float growthRate = 1f;
    public float pullForceMultiplier = 0.04f;
    private Vector2 mousePos;
    private LineRenderer line;
    private bool isPulled = false;
    private float cullDown = 0.35f;
    private float timeOfLastShot = 0;
    private float bulletSpeed = 4f;
    public GameObject basicGhost;
    private Vector2 mousePosition;
    private int currentSlot = 1;
    private List<InventorySlot> inventory = new List<InventorySlot>();
    private Vector2 scrollDirenction;

    private BoxCollider2D pullCollider;
    private Slider overheatScroller;

    InGameUIsctipts ui_controller;

    void Awake()
    {
        GameObject ui = GameObject.FindGameObjectWithTag("UI");

        ui_controller = ui.GetComponent<InGameUIsctipts>();
        ui_controller.SetSelected(currentSlot);

        for (int i = 0; i < 8; i++)
        {
            inventory.Add(new InventorySlot(4, "Empty"));
            ui_controller.ChangeSlotAmount(i, inventory[i].amount);

        }

        line = GetComponent<LineRenderer>();
        SetupStartLine();

        pullCollider = gameObject.AddComponent<BoxCollider2D>();
        pullCollider.isTrigger = true;


    }

    void Update()
    {
        if (isPulled)
        {
            Pull(mousePos);

            overheatValue += Time.deltaTime * overheatValueChagneRate;

        }
        else
        {
            DrawNothing();
            timerAnalogue = Time.time;

            if (isOverheat)
            {
                overheatValue -= Time.deltaTime * overheatValueRecoveryRate;

                if (overheatValue <= 0)
                {
                    isOverheat = false;
                    overheatValue = 0;
                }
            }

        }

        if (overheatValue >= 100)
        {
            isOverheat = true;
            overheatValue = 100;
        }
    }

    void Scroll()
    {
        Debug.Log("Created smthg");
        if (math.abs(scrollDirenction.y) > 0.5)
        {
            if ((currentSlot + (int)scrollDirenction.y) > 8)
            {
                ui_controller.ReturnToBaseColor(currentSlot);
                currentSlot = 0;
                ui_controller.SetSelected(currentSlot);
            }
            else if ((currentSlot + (int)scrollDirenction.y) < 0)
            {
                ui_controller.ReturnToBaseColor(currentSlot);
                currentSlot = 8;
                ui_controller.SetSelected(currentSlot);
            }
            {
                ui_controller.ReturnToBaseColor(currentSlot);
                currentSlot += (int)scrollDirenction.y;
                ui_controller.SetSelected(currentSlot);
            }

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

            ui_controller.ChangeSlotAmount(currentSlot, inventory[currentSlot].amount);
        }
    }

    float GetExponentialForce(float time, float initialForce, float growthRate)
    {
        return initialForce * Mathf.Exp(growthRate * time);
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
        
        ui_controller.ChangeSlotAmount(currentSlot, inventory[currentSlot].amount);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ghost"))
        {
            BaseGhost ghost = other.GetComponent<BaseGhost>();
            if (ghost != null)
            {
                float t = Time.time - timerAnalogue;
                float force = GetExponentialForce(t, pullForceMultiplier, growthRate);
                Vector2 direction = ((Vector2)transform.position - (Vector2)ghost.transform.position).normalized;
                
                ghost.ApplyExternalForce(direction * force);
                ghost.isPulling = true;

                if (Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position) < 1.7f)
                {
                    ConsumeGhost(other.gameObject);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost"))
        {
            BaseGhost ghost = collision.GetComponent<BaseGhost>();

            ghost.isPulling = false;
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

        if (mousePosition != new Vector2(0, 0))
            ShotGhost();
    }

    void OnScroll(InputValue input)
    {
        scrollDirenction = input.Get<Vector2>();

        Scroll();
    }

    void OnPull(InputValue input)
    {
        if (input.Get<Vector2>() == new Vector2(0, 0))
        {
            isPulled = false;
            pullCollider.enabled = false;
        }
        else
        {
            mousePos = input.Get<Vector2>();
            isPulled = true;
            pullCollider.enabled = true;
        }
    }

    void Pull(Vector2 input)
    {
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(input);
        mouseToWorld.z = 0;

        DrawLine(mouseToWorld);

        Vector2 midPoint = (mouseToWorld + transform.position) / 2f;

        Vector2 direction = ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, mouseToWorld);

        distance = Mathf.Clamp(0, maxRange, distance);
        float angle = Mathf.Atan2(direction.y, direction.x) + Mathf.Rad2Deg;

        pullCollider.size = new Vector2(distance, 2f);
        pullCollider.offset = transform.InverseTransformPoint(midPoint);
    }


    void DrawNothing()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }

    void DrawLine(Vector3 drawTowards)
    {
        Vector3 start = transform.position;

        line.SetPosition(0, start);
        line.SetPosition(1, drawTowards);
    }

    void SetupStartLine()
    {
        line.positionCount = 2;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.useWorldSpace = true;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }
    
    void OnDrawGizmos()
    {
        if (pullCollider == null)
            return;

        Gizmos.color = Color.green;

        Vector3 pos = pullCollider.transform.position + (Vector3)pullCollider.offset;
        Vector3 size = pullCollider.size;

        Gizmos.matrix = pullCollider.transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero + (Vector3)pullCollider.offset, size);
    }
}
