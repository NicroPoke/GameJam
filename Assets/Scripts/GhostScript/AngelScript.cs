using UnityEngine;

public class AngelScript : BaseGhost
{
    private Animator animatooor;

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

        Wander();
    }
    protected override void AnimationCorrector()
    {

    }

}
