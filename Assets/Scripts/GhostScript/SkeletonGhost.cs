using UnityEngine;

public class SkeletonGhost : BaseGhost
{
    private bool isCorpse = false;
    private float corpseCooldown = 3.5f;
    private float corpseTimer = 0f;

    protected override void Start()
    {
        base.Start();

        Speed = 2f;
        WanderSpeed = 1.5f;
        GhostType = "Skeleton";
        isPulling = false;
        HardGhost = true;
    }

    protected override void Update()
    {
        base.Update();

        if (isCorpse)
        {
            Alive = false;
            corpseTimer += Time.deltaTime;

            if (corpseTimer >= corpseCooldown)
            {
                
                Alive = true;
                isCorpse = false;

                corpseTimer = 0f;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet") && !isCorpse)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            isCorpse = true;
        }
        if (other.gameObject.CompareTag("Bullet") && isCorpse)
        {
            Debug.Log("Triggered");
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            Alive = false;
            Instantiate(Bullet, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    protected override void Die()
    {
        isCorpse = true;
    }
}
