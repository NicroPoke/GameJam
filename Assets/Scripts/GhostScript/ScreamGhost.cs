using UnityEngine;

public class ScreamGhost : BaseGhost
{
    private Animator animatoooor;
    public GameObject Scream;

    private float screamCooldownBase = 5f;
    private float screamCooldownRange = 0.75f;
    private float currentScreamCooldown;

    private float screamActiveDuration = 1f;
    private float screamTimer = 0f;
    private bool screamActive = false;

    protected override void Start()
    {
        base.Start();
        Speed = 2.5f;
        animatoooor = GetComponent<Animator>();
        GhostType = "Scream";
        isPulling = false;
        Scream.SetActive(false);
        ResetScreamCooldown();
        screamTimer = currentScreamCooldown; 
        HardGhost = true;
    }

    protected override void Update()
    {
        base.Update();
        animatoooor.SetBool("isPulling", isPulling);
        animatoooor.SetBool("isDead", isDying);
        animatoooor.SetBool("isScreaming", screamActive);
        screamTimer += Time.deltaTime;

        if (!screamActive && screamTimer >= currentScreamCooldown)
        {
            Speed = 4f;
            Scream.SetActive(true);
            screamActive = true;
            screamTimer = 0f;
        }
        else if (screamActive && screamTimer >= screamActiveDuration)
        {
            Speed = 2.5f;
            Scream.SetActive(false);
            screamActive = false;
            ResetScreamCooldown();
            screamTimer = 0f;
        }
    }

    private void ResetScreamCooldown()
    {
        currentScreamCooldown = screamCooldownBase + Random.Range(-screamCooldownRange, screamCooldownRange);
    }

    protected override void OnTriggerStay2D(Collider2D collision) { }

    protected override void AnimationCorrector() { }
}
