using UnityEngine;

public class ToxicGhost : BaseGhost
{
    public GameObject Gas;
    public AudioSource gasSound;

    private float gasCooldownBase = 5f;
    private float gasCooldownRange = 0.75f;
    private float currentGasCooldown;

    private float gasActiveDuration = 2f;
    private float gasTimer = 0f;
    private bool gasActive = false;

    [HideInInspector] public Animator toxAnimator;

    protected override void Start()
    {
        base.Start();
        toxAnimator = GetComponent<Animator>();
        Speed = 2.5f;
        WanderSpeed = 1.5f;
        GhostType = "Toxic";
        isPulling = false;
        Gas.SetActive(false);
        ResetGasCooldown();
        gasTimer = currentGasCooldown;
        HardGhost = true;
    }

    protected override void Update()
    {
        base.Update();

        gasTimer += Time.deltaTime;

        if (!gasActive && gasTimer >= currentGasCooldown)
        {
            Speed = 1.5f;
            Gas.SetActive(true);
            gasActive = true;
            gasTimer = 0f;

            if (gasSound != null && !gasSound.isPlaying)
            {
                gasSound.Play();
            }
        }
        else if (gasActive && gasTimer >= gasActiveDuration)
        {
            Speed = 2.5f;
            Gas.SetActive(false);
            gasActive = false;
            ResetGasCooldown();
            gasTimer = 0f;

            if (gasSound != null && gasSound.isPlaying)
            {
                gasSound.Stop();
            }
        }

        toxAnimator.SetBool("ToxicAttack", gasActive);
        toxAnimator.SetBool("ToxicRun", isPulling);
        toxAnimator.SetBool("ToxicDeath", isDying);
    }

    private void ResetGasCooldown()
    {
        currentGasCooldown = gasCooldownBase + Random.Range(-gasCooldownRange, gasCooldownRange);
    }

    protected override void OnTriggerStay2D(Collider2D collision) { }
}
