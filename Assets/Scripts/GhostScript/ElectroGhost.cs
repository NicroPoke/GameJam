using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class ElectroGhost : BaseGhost
{
    public GameObject ballLightning;
    public float ballSpeed = 3f;
    private float lightningTimer = 0;
    private float lightningCooldown = 3.3f;
    private float lightingLifespan = 2f;

    protected override void Start()
    {
        base.Start();
        Speed = 0.5f;
        WanderSpeed = 1.5f;
        GhostType = "Electric";
        isPulling = false;
    }

    protected override void Update()
    {
        base.Update();

        lightningTimer += Time.deltaTime;

        if (isAggroed && lightningTimer >= lightningCooldown)
        {
            Debug.Log("Shot");
            Speed = 0.5f;
            ShotTowardsPlayer();
            lightningTimer = 0f;
        }
    }

    void ShotTowardsPlayer()
    {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

        Vector2 direction = (targetRb.position - (Vector2)transform.position).normalized;
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

        GameObject lightingBall = Instantiate(ballLightning, transform.position, rotation);
        lightingBall.GetComponent<Rigidbody2D>().linearVelocity = direction * ballSpeed;
    }
}
