using UnityEngine;

public class AngelScript : BaseGhost
{
    private Animator animatooor;

    protected override void Start()
    {
        base.Start();
        GhostType = "Angel";
        HardGhost = false;        
    }

    protected override void Update()
    {
        animatooor = GetComponent<Animator>();
        animatooor.SetBool("AngelPanic", isPulling);
        animatooor.SetBool("AngelDead", isDying);
        if (!Alive)
        {
            Die();
        }
        if (target == null) return;

        if (isPulling)
        {
            Debug.Log("Moving away");
            MoveToTarget();
        }
        else
        {
            Wander();   
        }
    }
    protected override void AnimationCorrector()
    {

    }

}
