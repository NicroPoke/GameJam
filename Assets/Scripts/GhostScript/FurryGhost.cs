using UnityEngine;

public class FurryGhost : BaseGhost
{
    protected override void Start()
    {
        base.Start();
        Speed = 8f;
        WanderSpeed = 2.5f;
        TurningSpeed = 5f;
        invulnerabilityDuration = 0.5f;
        GhostType = "Furry";
        HardGhost = false;
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(1);
            isAttacking = true;
            lastDamageTime = Time.time;
        }
    }
}