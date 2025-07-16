using UnityEngine;
using System.Collections;
public class GhostBomj : BaseGhost
{
    private Animator animatoor;
    public GameObject BlueWater;
    private float BlueWaterCooldown = 3.3f;
    private float BlueWaterActiveDuration = 2f;
    private float BlueWaterTimer = 5f;
    private bool BlueWaterActive = false;

    protected override void Start()
    {
        base.Start();
        animatoor = GetComponent<Animator>();
        Speed = 2f;
        WanderSpeed = 1.5f;
        GhostType = "Bobj";
        isPulling = false;
        BlueWaterTimer = 2f;
        HardGhost = false;

    }

    protected override void Update()
    {
        base.Update();
        animatoor.SetBool("SpewerSpew", BlueWaterActive);
        animatoor.SetBool("SpewerPanic", isPulling);
        animatoor.SetBool("SpewerDying", isDying);
        BlueWaterTimer += Time.deltaTime;

        if (!BlueWaterActive && BlueWaterTimer >= BlueWaterCooldown)
        {
            SpewBlueWater();
        }
        else if (BlueWaterActive && BlueWaterTimer >= BlueWaterActiveDuration)
        {
            Speed = 3f;
            BlueWaterActive = false;
            BlueWaterTimer = 0f;
        }
    }
    protected override void AnimationCorrector()
    {

    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {

    }
    IEnumerator SpewBlueWater()
    {
        Speed = 2f;
        BlueWaterActive = true;
        yield return new WaitForSeconds(1f);
        Instantiate(BlueWater, transform.position, transform.rotation);
        BlueWaterTimer = 0f;
    }
}
