using UnityEngine;
using System.Collections;

public class GhostBomj : BaseGhost
{
    private Animator animatoor;
    public GameObject BlueWater;
    public AudioSource BlueWaterSound;

    private float blueWaterCooldownBase = 3.3f;
    private float blueWaterCooldownRange = 0.75f;
    private float currentBlueWaterCooldown;

    private float blueWaterActiveDuration = 1f;
    private float blueWaterTimer = 0f;
    private bool blueWaterActive = false;

    protected override void Start()
    {
        base.Start();
        animatoor = GetComponent<Animator>();
        Speed = 2f;
        WanderSpeed = 1.5f;
        GhostType = "Bobj";
        isPulling = false;
        ResetBlueWaterCooldown();
        blueWaterTimer = currentBlueWaterCooldown;
        HardGhost = false;
    }

    protected override void Update()
    {
        base.Update();
        animatoor.SetBool("SpewerSpew", blueWaterActive);
        animatoor.SetBool("SpewerPanic", isPulling);
        animatoor.SetBool("SpewerDying", isDying);
        blueWaterTimer += Time.deltaTime;

        if (!blueWaterActive && blueWaterTimer >= currentBlueWaterCooldown)
        {
            BlueWaterSound.Play();
            blueWaterActive = true;
            Speed = 2f;
            StartCoroutine(SpewBlueWater());
            blueWaterTimer = 0f;
        }
        else if (blueWaterActive && blueWaterTimer >= blueWaterActiveDuration)
        {
            Speed = 3f;
            blueWaterActive = false;
            ResetBlueWaterCooldown();
            blueWaterTimer = 0f;
        }
    }

    private void ResetBlueWaterCooldown()
    {
        currentBlueWaterCooldown = blueWaterCooldownBase + Random.Range(-blueWaterCooldownRange, blueWaterCooldownRange);
    }

    IEnumerator SpewBlueWater()
    {
        yield return new WaitForSeconds(0.8f);
        Instantiate(BlueWater, transform.position, transform.rotation);
    }

    protected override void AnimationCorrector() { }

    protected override void OnTriggerStay2D(Collider2D collision) { }
}
