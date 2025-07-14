using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance;

    public bool battleEnded { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!battleEnded)
        {
            var ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            if (ghosts.Length == 0)
            {
                battleEnded = true;
            }
        }
    }

    public void ResetBattleFlag()
    {
        battleEnded = false;
    }
}
