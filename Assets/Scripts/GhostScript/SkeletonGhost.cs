using UnityEngine;

public class SkeletonGhost : BaseGhost
{
    protected override void Start()
    {
        base.Start();

        Speed = 2f;
        WanderSpeed = 1.5f;
        GhostType = "Skeleton";
        isPulling = false;
        HardGhost = true;
    }
}
