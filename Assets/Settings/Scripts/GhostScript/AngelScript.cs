using UnityEngine;

public class AngelScript : BaseGhost
{

    protected override void Update()
    {
        if (!Alive)
        {
            Die();
        }
        if (target == null) return;

        Wander();
    }
}
