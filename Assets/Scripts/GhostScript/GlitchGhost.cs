using UnityEngine;

public class GlitchGhost : BaseGhost
{
    private float maxDist = 10f;
    private float distToTarget;
    private float tpDelay = 2f;
    private float tpTimer = 0f;
    private Animator glitchAnimator;
    private bool isDead = false;
    private bool isTP = false;
    private bool isAttak = false;

    protected override void Start()
    {
        distToTarget = maxDist;
        base.Start();
        Speed = 4f;
        WanderSpeed = 3f;
        GhostType = "Glitch";
        isPulling = false;
        tpTimer = 0f;
        HardGhost = true;

        glitchAnimator = GetComponent<Animator>();

        Collider2D myCol = GetComponent<Collider2D>();
        foreach (Collider2D col in FindObjectsOfType<Collider2D>())
        {
            if (col.gameObject != gameObject && col.gameObject.tag == "Untagged")
            {
                Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }

    protected override void Update()
    {
        if (target == null) return;

        base.Update();

        glitchAnimator.SetBool("IsTP", isTP);
        glitchAnimator.SetBool("IsDead", isDead);
        glitchAnimator.SetBool("IsAttak", isAttak);
        glitchAnimator.SetBool("IsPulling", isPulling);

        if (!isAggroed) return;

        tpTimer += Time.deltaTime;
        if (tpTimer >= tpDelay)
        {
            tpTimer = 0f;
            StartCoroutine(DoTpGlitch());
        }
        {
            distToTarget = Vector2.Distance(velocityPosition, target.position);
        }
    }

    private System.Collections.IEnumerator DoTpGlitch()
    {
        isTP = true;

        float glitchTime = 0.3f;
        float t = 0f;
        Vector3 orig = transform.position;

        while (t < glitchTime)
        {
            transform.position = orig + (Vector3)(Random.insideUnitCircle * 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = orig;
        isTP = false;

        DoTeleport();
    }

    void DoTeleport()
    {
        if (target == null) return;

        float angle = Random.Range(0f, 2f * Mathf.PI);
        float randDist = Random.Range(2f, maxDist);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randDist;

        velocityPosition = (Vector2)target.position + offset;
        rb.MovePosition(velocityPosition);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttak = false;
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            player.TakeDamege(10);
            isAttak = true;
            StartCoroutine(ResetAttackAnimation());
            StartCoroutine(DelayedTeleportAfterAttack(0.35f));
            lastDamageTime = Time.time;
        }
    }

    private System.Collections.IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        isAttak = false;
    }

    protected override void AnimationCorrector() {}

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;
        base.Die();
    }
    private System.Collections.IEnumerator DelayedTeleportAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(DoTpGlitch());
    }
}
