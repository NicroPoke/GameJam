using UnityEngine;
using System.Collections;

public class ElectroGhost : BaseGhost
{
    public GameObject ballLightning;
    public float ballSpeed = 3f;
    private Animator electroAnimator;
    private float lightningCooldownBase = 3.3f;
    private float lightningCooldownRange = 0.75f;
    private float currentLightningCooldown;
    [HideInInspector] public bool AAA;
    private float lightningTimer = 0f;

    protected override void Start()
    {
        base.Start();
        AAA = false;
        electroAnimator = GetComponent<Animator>();
        Speed = 0.5f;
        WanderSpeed = 1.5f;
        GhostType = "Electric";
        isPulling = false;
        HardGhost = true;
        ResetLightningCooldown();
        lightningTimer = currentLightningCooldown;
    }

    protected override void Update()
    {
        base.Update();
        electroAnimator.SetBool("ISDEAD", isDying);
        electroAnimator.SetBool("ISSOSAT", isPulling);
        electroAnimator.SetBool("ISATAK", AAA);
        lightningTimer += Time.deltaTime;
        if (isAggroed && lightningTimer >= currentLightningCooldown && !AAA)
        {
            Speed = 0.5f;
            StartCoroutine(ShotTowardsPlayer());
            ResetLightningCooldown();
            lightningTimer = 0f;
        }

        if (isAggroed && lightningTimer >= currentLightningCooldown)
        {
            Speed = 0.5f;
            ShotTowardsPlayer();
            ResetLightningCooldown();
            lightningTimer = 0f;
        }
    }

    private void ResetLightningCooldown()
    {
        currentLightningCooldown = lightningCooldownBase + Random.Range(-lightningCooldownRange, lightningCooldownRange);
    }

    IEnumerator ShotTowardsPlayer()
    {
        if (!Alive)
            yield break;

        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        Vector2 direction = (targetRb.position - (Vector2)transform.position).normalized;
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);
        AAA = true;
        GameObject lightingBall = Instantiate(ballLightning, transform.position, rotation);
        lightingBall.GetComponent<Rigidbody2D>().linearVelocity = direction * ballSpeed;
        yield return new WaitForSeconds(1.4f);
        AAA = false;
    }
    protected override void AnimationCorrector() { }
    
    protected override void OnTriggerStay2D(Collider2D collision) { }
}
