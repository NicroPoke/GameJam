using UnityEngine;

public class Sound : MonoBehaviour
{
    private GameObject player;
    private InventoryScroll inventoryScroll;
    public AudioSource sosalka;
    public AudioSource off;
    public AudioSource switcher;
    private float maxVolume = 1.0f;
    private float volumeSpeed = 0.75f;
    private bool wasPlaying = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            inventoryScroll = player.GetComponent<InventoryScroll>();
            if (inventoryScroll == null)
            {
                Debug.LogWarning("Соси хуй.");
            }
        }
        else
        {
            Debug.LogWarning("Соси хуй.");
        }

        if (sosalka == null)
        {
            Debug.LogWarning("Соси хуй.");
        }
    }

    void Update()
    {
        if (inventoryScroll != null && sosalka != null)
        {
            if (inventoryScroll.isPulled && !inventoryScroll.isOverheat)
            {
                if (!sosalka.isPlaying)
                {
                    sosalka.Play();
                    if (switcher != null) switcher.Play();
                }

                sosalka.volume = Mathf.MoveTowards(sosalka.volume, maxVolume, volumeSpeed * Time.deltaTime);
                wasPlaying = true;
            }
            else
            {
                if (wasPlaying)
                {
                    if (switcher != null) switcher.Play();
                    if (off != null) off.Play();
                }

                sosalka.volume = 0f;
                sosalka.Stop();
                wasPlaying = false;
            }
        }
    }
}
