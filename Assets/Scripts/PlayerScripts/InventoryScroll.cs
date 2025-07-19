using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.ExceptionServices;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    public bool IsFull()
    {
        int totalAmount = 0;
        foreach (InventorySlot slot in inventory)
        {
            totalAmount += slot.amount;
        }

        return totalAmount >= 9;
    }
    void UnselectSlot(int i)
    {
        if (inventory[i].amount > 0) ui_controller.SetSelected(i, true);
        else ui_controller.ReturnToBaseColor(i);
    }

    void DebugAddGhostTypes()
    {
        inventory.Add(new InventorySlot(0, "Contact"));
        inventory.Add(new InventorySlot(0, "Furry"));
        inventory.Add(new InventorySlot(0, "Bobj"));
        inventory.Add(new InventorySlot(0, "Glitch"));
        inventory.Add(new InventorySlot(0, "Scream"));
        inventory.Add(new InventorySlot(0, "Toxic"));
        inventory.Add(new InventorySlot(0, "Electric"));
        inventory.Add(new InventorySlot(0, "Skeleton"));
        inventory.Add(new InventorySlot(0, "Angel"));
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
        if(Time.deltaTime == 0f) return;
        if (!(inventory[currentSlot].amount <= 0) && ui_controller != null)
        {
            if (Time.time - timeOfLastShot <= cullDown)
                return;

            ShotBasic(GetBullet(inventory[currentSlot].type));

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

    GameObject GetBullet(string type)
    {
        switch (type)
        {
            case "Contact":
                return basicGhost;
            case "Furry":
                return furryBullet;
            case "Bobj":
                return bobjBullet;
            case "Toxic":
                return toxicBullet;
            case "Glitch":
                return glitchBullet;
            case "Scream":
                return screamBullet;
            case "Electric":
                return elctroBullet;
            case "Skeleton":
                return skeletonBullet;
            case "Angel":
                return angelBullet;
            default:
                return null;
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

    void ShotRound(int numShots)
    {
        if (Time.deltaTime == 0f) return;
        List<Vector2> vectors = GenerateCircleVectors(numShots);

        if (vectors.Count <= 0) return;

        List<string> types = GenerateBulletTypes();
        sound.shot.Play();
        for (int i = 0; i < vectors.Count; i++)
        {
            float rotationZ = Mathf.Atan2(vectors[i].y, vectors[i].x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

            GameObject ghostBullet = Instantiate(GetBullet(types[i]), transform.position, rotation);

            ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = vectors[i] * bulletSpeed;

            timeOfLastShot = Time.time;
        }

        ClearInventory();
    }

    List<string> GenerateBulletTypes()
    {
        List<string> list = new List<string>();
        foreach (InventorySlot slot in inventory)
        {
            int amount = slot.amount;
            for (int i = 0; i < amount; i++)
            {
                list.Add(slot.type);   
            }
        }

        return list;
    }

    public void ConsumeGhost(GameObject ghost)
    {
        if (Time.deltaTime == 0f) return;
        BaseGhost bg = ghost.GetComponent<BaseGhost>();
        InventorySlot slot = null;
        if (ghost.gameObject.CompareTag("Ghost") || ghost.gameObject.CompareTag("Angel"))
        {
            string type = ghost.GetComponent<BaseGhost>().GhostType;
            Debug.Log(type);
            slot = GetGhostWithNeededType(type);

            Debug.Log(slot.type);
        }
        else
        {
            string type = ghost.name;

            slot = GetGhostWithNeededType(type);
        }

        if (IsNotFilled())
        {
            currentSlot = inventory.IndexOf(slot);
            ui_controller.SetSelected(currentSlot);
            if (colorChange != null) colorChange.ChangeColor(inventory[currentSlot].type);
            slot.amount++;
        }
        else
        {
            slot.amount++;
            UnselectSlot(inventory.IndexOf(slot));
        }
        sound.pop.Play();
        Destroy(ghost);
    }

    void ShotBasic(GameObject pivot)
    {
        if(Time.deltaTime == 0f) return;
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
        if(Time.deltaTime == 0f) return;
        mousePosition = input.Get<Vector2>();

        if (mousePosition != new Vector2(0, 0))
            ShotGhost();
    }

    void OnScroll(InputValue input)
    {
        scrollDirenction = input.Get<Vector2>();

        Scroll();
    }

    int GetTotalAmount()
    {
        int totalAmount = 0;
        foreach (InventorySlot slot in inventory)
        {
            totalAmount += slot.amount;
        }

        return totalAmount;
    }

    void ClearInventory()
    {
        foreach (InventorySlot slot in inventory)
        {
            slot.amount = 0;
            UnselectSlot(inventory.IndexOf(slot));
        }
    }

    void OnCircle()
    {
        ShotRound(GetTotalAmount());
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
