using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    private bool isOverheat;
    private float overheatValueRecoveryRate = 15f;
    private float overheatValueChagneRate = 10f;
    private float overheatValue = 0;

    public float growthRate = 1f;
    public float pullForceMultiplier = 0.04f;

    private bool facingRight = false;
    private GameObject body;
    private SpriteRenderer sr;
    private bool handedGun = true;

    public Sprite handedSprite;
    public Sprite unhandedSprite;


    private bool isPulled = false;
    private Vector2 mousePos;

    private BoxCollider2D pullCollider;
    public float maxRange = 5;

    private LineRenderer line;
    private float timerAnalogue = 0;

    private GameObject colliderHolder;

    private float bodySpeed;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        body = transform.parent.gameObject;
        bodySpeed = body.GetComponent<PlayerController>().speed;

        line = GetComponent<LineRenderer>();
        SetupStartLine();

        pullCollider = gameObject.AddComponent<BoxCollider2D>();
        pullCollider.isTrigger = true;
    }

    void Update()
    {
        line.enabled = !isOverheat;


        if (isPulled)
        {
            Pull(mousePos);

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


        RotationHandler();
    }

    void RotationHandler()
    {
        Vector2 direction = -1 * CreateDirectionVector(Input.mousePosition);

        float angle = Mathf.Atan2(direction.y, direction.x);
        float rotationZ = Mathf.Rad2Deg * angle;

        HandleSlide(angle);
        Quaternion rotation = Quaternion.Euler(180, 180, rotationZ);

        transform.rotation = rotation;
    }

    void HandleSlide(float degrees)
    {
        float cosin = Mathf.Cos(degrees);

        if (cosin > 0 && facingRight)
        {
            FlipCharacter();
            facingRight = false;
        }
        else if (cosin < 0 && !facingRight)
        {
            FlipCharacter();
            facingRight = true;
        }

        if (math.abs(cosin) > 0.7 && handedGun)
        {
            handedGun = false;
            sr.sprite = unhandedSprite;
        }
        else if (math.abs(cosin) < 0.7 && facingRight)
        {
            handedGun = true;
            sr.sprite = handedSprite;
        }
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

    void DrawLine(Vector3 drawTowards)
    {
        Vector3 start = transform.position;

        line = GetComponent<LineRenderer>();

        line.SetPosition(0, start);
        line.SetPosition(1, drawTowards);
    }
    void ChangeSliderHP()
    {
        GameObject target = GameObject.Find("SliderOverheat");

        if (target != null)
        {
            target.GetComponent<SliderScript>().ChangeSliderValue((int)overheatValue);
        }
    }

    void DrawNothing()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }

    void FlipCharacter()
    {
        Vector3 euler = body.transform.eulerAngles;
        euler.y = euler.y + 180f;
        body.transform.eulerAngles = euler;

        sr.flipY = !sr.flipY;
    }

    Vector2 CreateDirectionVector(Vector2 mousePos)
    {
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mouseToWorld.z = 0;
        return ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;
    }

    void Pull(Vector2 input)
    {
        if (isOverheat) return;

        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseToWorld.z = 0;

        DrawLine(mouseToWorld);

        Vector2 midPoint = (mouseToWorld + transform.position) / 2f;

        Vector2 direction = ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(mouseToWorld, transform.position);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        pullCollider.size = new Vector2(distance / body.transform.localScale.y, 1f);
        pullCollider.offset = new UnityEngine.Vector2(distance / body.transform.localScale.y / 2f, 0f);
    }

    void OnPull(InputValue input)
    {
        if (input.Get<Vector2>() == new Vector2(0, 0))
        {
            isPulled = false;
            pullCollider.enabled = false;

            body.GetComponent<PlayerController>().speed = bodySpeed;
        }
        else if (!isOverheat)
        {
            mousePos = input.Get<Vector2>();
            isPulled = true;
            pullCollider.enabled = true;

            body.GetComponent<PlayerController>().speed = bodySpeed * 0.3f;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isOverheat) return;
        float t = Time.time - timerAnalogue;
        float force = GetExponentialForce(t, pullForceMultiplier, growthRate);

        if (other.CompareTag("Ghost"))
        {
            BaseGhost ghost = other.GetComponent<BaseGhost>();
            if (ghost != null && !ghost.HardGhost)
            {

                Vector2 direction = ((Vector2)transform.position - (Vector2)ghost.transform.position).normalized;

                ghost.ApplyExternalForce(direction * force);
                ghost.isPulling = true;

                if (Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position) < 1.7f && isPulled)
                {
                    body.GetComponent<InventoryScroll>().ConsumeGhost(other.gameObject);
                }
            }
        }
        else if (other.CompareTag("Consumable"))
        {
            Vector2 direction = ((Vector2)transform.position - (Vector2)other.transform.position);
            other.GetComponent<Rigidbody2D>().linearVelocity = direction * force;

            if (Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position) < 1.7f && isPulled)
            {
                body.GetComponent<InventoryScroll>().ConsumeGhost(other.gameObject);
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

    float GetExponentialForce(float time, float initialForce, float growthRate)
    {
        return initialForce * Mathf.Exp(growthRate * time);
    }

}
