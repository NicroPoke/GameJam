using System.Collections;
using UnityEngine;

public class SkeletonGhost : BaseGhost
{
    public Animator skeletonAnimator;
    [HideInInspector] public bool isRespawning;
    private float respawnCD = 0.5f;
    private float respaenTimer = 0f;
    protected override void Start()
    {
        base.Start();
        skeletonAnimator = GetComponent<Animator>();
        Speed = 2f;
        WanderSpeed = 1.5f;
        GhostType = "Skeleton";
        isPulling = false;
        HardGhost = true;

        respaenTimer = Time.time;
    }

    protected override void Update()
    {
        if (isRespawning && Time.time - respaenTimer >= respawnCD)
        {
            isRespawning = false;
        }

        Debug.Log(isRespawning);

        base.Update();

        skeletonAnimator.SetBool("SkelenotRunning", isPulling);
        skeletonAnimator.SetBool("SkeletonDeath", isDying);
        skeletonAnimator.SetBool("SkeletonAttacking", isAttacking);
        skeletonAnimator.SetBool("SkeletonRespawning", isRespawning);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!Alive) return;

        if (other.gameObject.CompareTag("Bullet"))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            Alive = false;

            StartCoroutine(StartCorpse());
        }
    }
    
    IEnumerator StartCorpse()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Ingigigigigigigigigigigigigigi");

        Instantiate(Bullet, transform.position, transform.rotation);
    }


    protected override void AnimationCorrector()
    {

    }
}
