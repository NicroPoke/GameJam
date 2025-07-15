using UnityEngine;

public class GlitchGhost : BaseGhost
{
    private float maxDist = 10f;
    private float distToTarget;
    private float tpDelay = 2f;
    private float tpTimer = 0f;

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

        if (!isAggroed) return;

        tpTimer += Time.deltaTime;

        if (tpTimer >= tpDelay)
        {
            tpTimer = 0f;
            StartCoroutine(DoTpGlitch());
        }
        else
        {
            distToTarget = Vector2.Distance(velocityPosition, target.position);
        }
    }

    private System.Collections.IEnumerator DoTpGlitch()
    {
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

    protected override void OnCollisionStay2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (col.gameObject.TryGetComponent(out PlayerController player))
        {
            player.TakeDamege(10);
            isAttacking = true;
            Debug.Log("GlitchGhost атакует игрока (урон 10).");
            StartCoroutine(DoTpGlitch());
            lastDamageTime = Time.time;
        }
    }
}