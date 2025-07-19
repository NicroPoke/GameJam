using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    bool isPullerActive = false;
    public Animator animator;
    public int health = 100;
    [SerializeField] private float pullForceMultiplier = 5f;
    public float baseSpeed = 9f;
    public float speed;
    private Rigidbody2D rb;
    [HideInInspector] public bool IsWalking = false;
    [HideInInspector] public bool IsHurting = false;
    [HideInInspector] public Vector2 inputVector;
    public float currentVelocity = 0f;
    private BaseGhost lastGhost;
    [SerializeField] private float knockbackForce = 7f;

    private bool isKnockbacked = false;
    private float knockbackDuration = 0.25f;
    private CameraFollow cameraFollow;
    [HideInInspector]public bool isDead = false;

    void Awake()
    {
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        lastGhost = null;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            health = 0;
        }
        if (health <= 0)
        {
            StartCoroutine(Die());
        }

        if (isDead)
        {
            Debug.Log("DEAAAAAAAAAAAD");
        }
    
        animator.SetBool("IsDead", isDead);
        animator.SetBool("IsWalking", IsWalking);
        animator.SetBool("IsHurting", IsHurting);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (!isKnockbacked)
        {
            Vector2 movement = speed * inputVector;
            rb.linearVelocity = movement;
            currentVelocity = movement.sqrMagnitude;
            IsWalking = currentVelocity > 0;
        }


    }

    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    IEnumerator Die()
    {
        isDead = true;
        GameObject.Find("Gun").SetActive(false);
        GameObject.Find("Sholder").SetActive(false);
        GameObject.Find("bollte1").SetActive(false);

        yield return new WaitForSeconds(1.4f);
        Time.timeScale = 0f;
    }



    public void TakeDamege(int damage)
    {
        health -= damage;
        ChangeSliderHP();

        if (cameraFollow != null)
        {
            cameraFollow.ShakeExternal();
        }

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        StartCoroutine(ApplyKnockback(randomDirection));
        StartCoroutine(DamageAnimation());
    }

    IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockbacked = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockbacked = false;
    }

    IEnumerator DamageAnimation()
    {
        IsHurting = true;
        yield return new WaitForSecondsRealtime(0.25f);
        IsHurting = false;
    }

    void ChangeSliderHP()
    {
        GameObject target = GameObject.Find("HP");

        if (target != null)
        {
            target.GetComponent<Image>().fillAmount = health * 0.01f;
        }
    }
}
