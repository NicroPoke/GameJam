using UnityEngine;

public class ToxicGhost : BaseGhost 
{
    public GameObject Gas;
    private float gasCooldown = 5f; 
    private float gasActiveDuration = 2f; 
    private float gasTimer = 5f;
    private bool gasActive = false;

    protected override void Start()
    {
        base.Start();
        Speed = 3f;
        GhostType = "Toxic";
        isPulling = false;
        Gas.SetActive(false);
        gasTimer = 2f; 
    }

    protected override void Update()
    {
        base.Update();

        gasTimer += Time.deltaTime;

        if (!gasActive && gasTimer >= gasCooldown)
        {   
            Speed = 2f;
            Gas.SetActive(true);
            gasActive = true;
            gasTimer = 0f;
        }
        else if (gasActive && gasTimer >= gasActiveDuration)
        {
            Speed = 3f;
            Gas.SetActive(false);
            gasActive = false;
            gasTimer = 0f;
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {

    }
}