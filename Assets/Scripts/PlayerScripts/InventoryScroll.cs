using System;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Mathematics;
using UnityEngine;
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
    private Sound sound;
    [HideInInspector] public bool isOverheat;
    [HideInInspector] public float coolingTime = 0f;
    private float overheatValueRecoveryRate = 15f;
    private float overheatValueChagneRate = 10f;
    private float overheatValue = 0;
    private float timerAnalogue = 0;
    public float maxRange = 5;
    private float growthRate = 1f;
    public float pullForceMultiplier = 0.04f;
    private Vector2 mousePos;
    private LineRenderer line;
    [HideInInspector] public bool isPulled = false;
    private float cullDown = 0.4f;
    private float timeOfLastShot = 0;
    private float bulletSpeed = 25f;

    public GameObject basicGhost;
    public GameObject furryBullet;
    public GameObject toxicBullet;
    public GameObject glitchBullet;
    public GameObject bobjBullet;
    public GameObject screamBullet;
    public GameObject elctroBullet;
    public GameObject skeletonBullet;
    public GameObject angelBullet;

    private Vector2 mousePosition;
    private int currentSlot = 1;
    private List<InventorySlot> inventory = new List<InventorySlot>();
    private Vector2 scrollDirenction;

    private BoxCollider2D pullCollider;
    private UnityEngine.UI.Slider overheatScroller;

    InGameUIsctipts ui_controller;
    private ColorChangeHandler colorChange;

    void Awake()
    {
        GameObject ui = GameObject.Find("Dock");

        colorChange = ui.transform.parent.gameObject.GetComponent<ColorChangeHandler>();
        UnityEngine.Debug.Log(colorChange);
        if (ui != null)
        {
            ui_controller = ui.GetComponent<InGameUIsctipts>();
            ui_controller.SetSelected(currentSlot);
        }

        DebugAddGhostTypes();

        UnityEngine.Debug.Log(inventory.Count);
        for (int i = 0; i < 8; i++)
        {
            UnselectSlot(i);
        }

        line = GetComponent<LineRenderer>();
        SetupStartLine();
        sound = GameObject.Find("SoundManager").GetComponent<Sound>();
    }

    void Update()
    {
        line.enabled = !isOverheat;

        if (isPulled)
        {
            coolingTime = 0f;

            if (!isOverheat)
            {
                overheatValue += Time.deltaTime * overheatValueChagneRate;
                ChangeSliderHP();
            }
        }
        else
        {
            DrawNothing();
            timerAnalogue = Time.time;
            coolingTime += Time.deltaTime;
            overheatValue -= Time.deltaTime * overheatValueRecoveryRate;
            if (overheatValue <= 0)
            {
                isOverheat = false;
                overheatValue = 0;
            }
            ChangeSliderHP();
        }

        if (overheatValue >= 100)
        {
            isOverheat = true;
            overheatValue = 100;
            ChangeSliderHP();
        }
    }
    void UnselectSlot(int i)
    {
        if (inventory[i].amount > 0) ui_controller.SetSelected(i, true);
        else ui_controller.ReturnToBaseColor(i);
    }

    void DebugAddGhostTypes()
    {
        inventory.Add(new InventorySlot(3, "Contact"));
        inventory.Add(new InventorySlot(3, "Furry"));
        inventory.Add(new InventorySlot(3, "Bobj"));
        inventory.Add(new InventorySlot(3, "Glitch"));
        inventory.Add(new InventorySlot(3, "Scream"));
        inventory.Add(new InventorySlot(3, "Toxic"));
        inventory.Add(new InventorySlot(3, "Electric"));
        inventory.Add(new InventorySlot(3, "Skeleton"));
        inventory.Add(new InventorySlot(3, "Angel"));
    }

    bool IsNotFilled()
    {
        int total = 0;
        foreach (InventorySlot slot in inventory)
        {
            total += slot.amount;
        }

        if (total <= 0) return true;
        else return false;
    }

    int GetFilledSlot()
    {
        foreach (InventorySlot slot in inventory)
        {
            if (slot.amount > 0) return inventory.IndexOf(slot);
        }
        return -1;
    }

    void Scroll()
    {
        if (math.abs(scrollDirenction.y) <= 0.5f || ui_controller == null || inventory == null || inventory.Count == 0)
            return;

        if (currentSlot == -1)
        {
            currentSlot = GetFilledSlot();
            if (currentSlot != -1)
            {
                ui_controller.SetSelected(currentSlot);
                
            }
            else return; 
        }

        int direction = (int)math.sign(scrollDirenction.y); 
        int startSlot = currentSlot;
        int attempts = 0;
        int maxSlots = inventory.Count; 

        do
        {
            currentSlot += direction;

            if (currentSlot >= maxSlots)
                currentSlot = 0;
            else if (currentSlot < 0)
                currentSlot = maxSlots - 1;

            attempts++;

            if (inventory[currentSlot] != null && inventory[currentSlot].amount > 0)
            {
                UnselectSlot(startSlot);
                ui_controller.SetSelected(currentSlot);
                if (colorChange != null) colorChange.ChangeColor(inventory[currentSlot].type);
                return;
            }

        } while (currentSlot != startSlot && attempts < maxSlots);
    }


    void ShotGhost()
    {
        if (!(inventory[currentSlot].amount <= 0) && ui_controller != null)
        {
            if (Time.time - timeOfLastShot <= cullDown)
                return;
            switch (inventory[currentSlot].type)
            {
                case "Contact":
                    ShotBasic(basicGhost);
                    break;
                case "Furry":
                    ShotBasic(furryBullet);
                    break;
                case "Bobj":
                    ShotBasic(bobjBullet);
                    break;
                case "Toxic":
                    ShotBasic(toxicBullet);
                    break;
                case "Glitch":
                    ShotBasic(glitchBullet);
                    break;
                case "Scream":
                    ShotBasic(screamBullet);
                    break;
                case "Electric":
                    ShotBasic(elctroBullet);
                    break;
                case "Skeleton":
                    ShotBasic(skeletonBullet);
                    break;
                case "Angel":
                    ShotBasic(angelBullet);
                    break;
            }
            inventory[currentSlot].amount--;

            if (inventory[currentSlot].amount <= 0)
            {
                ui_controller.ReturnToBaseColor(currentSlot);
                if (colorChange != null) colorChange.ChangeColor();
                if (!IsNotFilled())
                {
                    currentSlot = GetFilledSlot();
                    ui_controller.SetSelected(currentSlot);
                    if (colorChange != null) colorChange.ChangeColor(inventory[currentSlot].type);
                }
            }
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

    public void ConsumeGhost(GameObject ghost)
    {
        BaseGhost bg = ghost.GetComponent<BaseGhost>();
        InventorySlot slot = null;
        if (gameObject.CompareTag("Ghost") || gameObject.CompareTag("Angel"))
        {
            string type = ghost.GetComponent<BaseGhost>().GhostType;

            slot = GetGhostWithNeededType(type);
        }
        else
        {
            string type = ghost.name;

            slot = GetGhostWithNeededType(type);
        }

        Debug.Log("IS NOT FILLED");

        if (IsNotFilled())
        {
            currentSlot = inventory.IndexOf(slot);
            ui_controller.SetSelected(currentSlot);
            if (colorChange != null) colorChange.ChangeColor(inventory[currentSlot].type);
        }
        sound.pop.Play();
        slot.amount++;
        Destroy(ghost);
    }

    void ShotBasic(GameObject pivot)
    {
        Vector2 direction = CreateDirectionVector(mousePosition);

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

        GameObject ghostBullet = Instantiate(pivot, transform.position, rotation);
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
        }
        else
        {
            mousePos = input.Get<Vector2>();
            isPulled = true;
        }
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

    void ChangeSliderHP()
    {
        GameObject target = GameObject.Find("Overheat");

        if (target != null)
        {
            target.GetComponent<UnityEngine.UI.Image>().fillAmount = overheatValue * 0.01f;
        }
    }

    InventorySlot GetGhostWithNeededType(string type)
    {
        foreach (InventorySlot slot in inventory)
        {
            if (slot.type == type)
            {
                return slot;
            }
        }
        return inventory[0];
    }
}
