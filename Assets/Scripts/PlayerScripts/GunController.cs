using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    public bool isSlowed = false;

    private float lastConsumeTime = -1f;
    private float consumeCooldown = 0.5f;
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
    private LineRenderer line;
    private float timerAnalogue = 0;

    private PlayerController playerController;
    private float baseSpeed;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        body = transform.parent.gameObject;
        playerController = body.GetComponent<PlayerController>();
        baseSpeed = playerController.speed;
        line = GetComponent<LineRenderer>();
        SetupStartLine();

        pullCollider = gameObject.AddComponent<BoxCollider2D>();
        pullCollider.isTrigger = true;
        pullCollider.enabled = false;
    }

    void Update()
    {
        if (body.GetComponent<PlayerController>().currentVelocity != 0)
        {
            ShuffleGun();
        }
        else
        {
            Vector3 pos = transform.localPosition;
            pos.y = 0;
            transform.localPosition = pos;
        }
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
        if(Time.deltaTime == 0f) return;
        Vector2 direction = -1 * CreateDirectionVector(Input.mousePosition);
        float angle = Mathf.Atan2(direction.y, direction.x);
        float rotationZ = Mathf.Rad2Deg * angle;
        HandleSlide(angle);
        Quaternion rotation = Quaternion.Euler(180, 180, rotationZ);
        transform.rotation = rotation;
    }

    void HandleSlide(float degrees)
    {
        if(Time.deltaTime == 0f) return;
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

        if (Mathf.Abs(cosin) > 0.7f && handedGun)
        {
            handedGun = false;
            sr.sprite = unhandedSprite;
        }
        else if (Mathf.Abs(cosin) < 0.7f && facingRight)
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
        line.SetPosition(0, start);
        line.SetPosition(1, drawTowards);
    }

    void ChangeSliderHP()
    {
        GameObject target = GameObject.Find("SliderOverheat");
        if (target != null)
            target.GetComponent<SliderScript>().ChangeSliderValue((int)overheatValue);
    }

    void ShuffleGun()
    {
        if(Time.deltaTime == 0f) return;
        Vector3 pos = transform.localPosition;
        pos.y += Mathf.Cos(Time.time * 12f) * 0.003f; 
        pos.x += Mathf.Cos(Time.time * 12f) * 0.001f; 
        transform.localPosition = pos;
    }

    void DrawNothing()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }

    void FlipCharacter()
    {
        if(Time.deltaTime == 0f) return;
        Vector3 euler = body.transform.eulerAngles;
        euler.y += 180f;
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

        Vector2 direction = ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(mouseToWorld, transform.position);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        pullCollider.size = new Vector2(distance / body.transform.localScale.y, 2.5f);
        pullCollider.offset = new Vector2(distance / body.transform.localScale.y / 2f, 0f);
    }

    void OnPull(InputValue input)
    {
        if (input.Get<Vector2>() == Vector2.zero)
        {
            isPulled = false;
            pullCollider.enabled = false;
            playerController.speed = baseSpeed;
        }
        else if (!isOverheat)
        {
            mousePos = input.Get<Vector2>();
            isPulled = true;
            pullCollider.enabled = true;

            playerController.speed = baseSpeed * 0.7f;
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
                if (Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position) < 1.7f && isPulled && Time.time - lastConsumeTime > consumeCooldown)
                {
                    body.GetComponent<InventoryScroll>().ConsumeGhost(other.gameObject);
                    lastConsumeTime = Time.time;
                }
            }
        }
        else if (other.CompareTag("Consumable"))
        {
            Vector2 direction = ((Vector2)transform.position - (Vector2)other.transform.position);
            other.GetComponent<Rigidbody2D>().linearVelocity = direction * force;
            if (Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position) < 1.7f && isPulled && Time.time - lastConsumeTime > consumeCooldown)
            {
                body.GetComponent<InventoryScroll>().ConsumeGhost(other.gameObject);
                lastConsumeTime = Time.time;
            }
        }
        else if (other.CompareTag("Angel"))
        {
            BaseGhost ghost = other.GetComponent<BaseGhost>();
            if (ghost != null)
            {
                Vector2 direction = ((Vector2)transform.position - (Vector2)ghost.transform.position).normalized;
                ghost.ApplyExternalForce(direction * force);
                ghost.isPulling = true;

                if (Vector2.Distance((Vector2)other.transform.position, (Vector2)transform.position) < 1.7f && isPulled && Time.time - lastConsumeTime > consumeCooldown)
                {
                    body.GetComponent<PlayerController>().TakeDamege(50);
                    body.GetComponent<InventoryScroll>().ConsumeGhost(other.gameObject);
                    lastConsumeTime = Time.time;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost") || collision.CompareTag("Angel"))
        {
            BaseGhost ghost = collision.GetComponent<BaseGhost>();
            ghost.isPulling = false;
        }
    }

    float GetExponentialForce(float time, float initialForce, float growthRate)
    {
        return initialForce * Mathf.Exp(growthRate * time);
    }
}
