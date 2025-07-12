using UnityEngine;

public class ScreamGhost : BaseGhost
{
    public GameObject Scream;
    private float ScreamCooldown = 5f; 
    private float ScreamActiveDuration = 1f; 
    private float ScreamTimer = 5f;
    private bool ScreamActive = false;

    protected override void Start()
    {
        base.Start();
        Speed = 4f;
        GhostType = "Scream";
        isPulling = false;
        Scream.SetActive(false);
        ScreamTimer = 2f; 
    }

    protected override void Update()
    {
        base.Update();

        ScreamTimer += Time.deltaTime;

        if (!ScreamActive && ScreamTimer >= ScreamCooldown)
        {   
            Speed = 4f;
            Scream.SetActive(true);
            ScreamActive = true;
            ScreamTimer = 0f;
        }
        else if (ScreamActive && ScreamTimer >= ScreamActiveDuration)
        {
            Speed = 5f;
            Scream.SetActive(false);
            ScreamActive = false;
            ScreamTimer = 0f;
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(3);
            isAttacking = true;
            Debug.Log("ScreamGhost атакует игрока (урон 3).");
            lastDamageTime = Time.time;
        }
    }
}
