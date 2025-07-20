using UnityEngine;

public class FurryGhost : BaseGhost
{
    private Animator furryAnimatoor;
    protected override void Start()
    {
        base.Start();
        furryAnimatoor = GetComponent<Animator>();
        Speed = 8f;
        WanderSpeed = 2.5f;
        TurningSpeed = 5f;
        invulnerabilityDuration = 0.5f;
        GhostType = "Furry";
        HardGhost = false;
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log(isAttacking);

        furryAnimatoor.SetBool("FurryRunning", isPulling);
        furryAnimatoor.SetBool("FurryAttacking", isAttacking);
        furryAnimatoor.SetBool("FurryDeath", isDying);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            if (Time.time - lastDamageTime > invulnerabilityDuration)
            {
                controller.TakeDamege(1);
                isAttacking = true;
                lastDamageTime = Time.time;
            }
            else return;
        }
        else
        {
            isAttacking = false;
        }
    }


    protected override void AnimationCorrector()
    {
        
    }
}