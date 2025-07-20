using UnityEngine;

public class BossSetup : MonoBehaviour
{
    public GameObject bossBarr;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Passed through");
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.parent.gameObject.GetComponent<BossHandler>().isLightningInstantiated = true;

            bossBarr.SetActive(true);
        }
    }
}
