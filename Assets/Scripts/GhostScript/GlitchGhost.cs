using UnityEngine;

public class GlitchGhost : BaseGhost
{
    private float MaxRadius = 5f;
    private float Radius;
    private float TeleportInterval = 3f;
    private float TeleportTimer = 0f;

    protected override void Start()
    {
        Radius = MaxRadius;
        base.Start();
        Speed = 2f;
        GhostType = "Glitch";
        isPulling = false;
        TeleportTimer = 0f;
        Collider2D ownCollider = GetComponent<Collider2D>();
        foreach (Collider2D col in FindObjectsOfType<Collider2D>())
        {
            if (col.gameObject != gameObject && col.gameObject.tag == "Untagged")
            {
                Physics2D.IgnoreCollision(ownCollider, col);
            }
        }
    }

    protected override void Update()
    {
        if (target == null) return;

        TeleportTimer += Time.deltaTime;

        if (TeleportTimer >= TeleportInterval)
        {
            TeleportTimer = 0f;
            RandomTeleport();
        }
        else
        {
            base.Update();
            Radius = Vector2.Distance(velocityPosition, target.position);
        }
    }

    void RandomTeleport()
    {
        if (target == null) return;

        float angle = Random.Range(0, 2 * Mathf.PI);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Radius;
        velocityPosition = (Vector2)target.position + offset;
        rb.MovePosition(velocityPosition);
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(10);
            isAttacking = true;
            Debug.Log("ScreamGhost атакует игрока (урон 10).");
            RandomTeleport();
            lastDamageTime = Time.time;
        }
    }
}
