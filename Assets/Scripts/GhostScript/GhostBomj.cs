using UnityEngine;

public class GhostBomj : BaseGhost
{
    public GameObject BlueWater;
    private float BlueWaterCooldown = 3.3f;
    private float BlueWaterActiveDuration = 2f;
    private float BlueWaterTimer = 5f;
    private bool BlueWaterActive = false;

    protected override void Start()
    {
        base.Start();
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

        BlueWaterTimer += Time.deltaTime;

        if (!BlueWaterActive && BlueWaterTimer >= BlueWaterCooldown)
        {
            Speed = 2f;
            Instantiate(BlueWater, transform.position, transform.rotation);
            BlueWaterActive = true;
            BlueWaterTimer = 0f;
        }
        else if (BlueWaterActive && BlueWaterTimer >= BlueWaterActiveDuration)
        {
            Speed = 3f;
            BlueWaterActive = false;
            BlueWaterTimer = 0f;
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {

    }
}
