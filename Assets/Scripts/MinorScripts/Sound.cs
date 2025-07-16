using UnityEngine;

public class Sound : MonoBehaviour
{
    private GameObject player;
    private InventoryScroll inventoryScroll;
    private PlayerController playerController;


    public AudioSource sosalka;
    public AudioSource off;
    public AudioSource switcher;
    public AudioSource overheat;
    public AudioSource walkSound;
    public AudioSource slowWalkSound;
    public AudioSource pop;

    private bool wasOverheated = false;
    private float maxVolume = 1.0f;
    private float volumeSpeed = 0.75f;
    private bool wasPlaying = false;
    private bool isOverheatSoundPlaying = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            inventoryScroll = player.GetComponent<InventoryScroll>();
            playerController = player.GetComponent<PlayerController>();

            if (inventoryScroll == null)
                Debug.LogWarning("InventoryScroll не найден.");
            if (playerController == null)
                Debug.LogWarning("PlayerController не найден.");
        }

        if (sosalka == null) Debug.LogWarning("AudioSource 'sosalka' не назначен.");
        if (overheat == null) Debug.LogWarning("AudioSource 'overheat' не назначен.");
        if (walkSound == null) Debug.LogWarning("AudioSource 'walkSound' не назначен.");
        if (pop == null) Debug.LogWarning("AudioSource 'pop' не назначен.");
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            StopAllSounds();
            return;
        }

        if (inventoryScroll == null || sosalka == null || overheat == null || playerController == null)
            return;

        HandlePullSounds();
        HandleWalkSound();
    }

    void HandlePullSounds()
    {
        if (inventoryScroll.isPulled && !inventoryScroll.isOverheat)
        {
            wasOverheated = false;

            if (!sosalka.isPlaying)
            {
                sosalka.Play();
                switcher?.Play();
            }

            sosalka.volume = Mathf.MoveTowards(sosalka.volume, maxVolume, volumeSpeed * Time.deltaTime);
            wasPlaying = true;

            if (isOverheatSoundPlaying)
            {
                overheat.Stop();
                isOverheatSoundPlaying = false;
            }
        }
        else
        {
            if (inventoryScroll.isOverheat)
            {
                if (!wasOverheated)
                {
                    overheat.Play();
                    switcher?.Play();
                    isOverheatSoundPlaying = true;
                    wasOverheated = true;
                }

                if (sosalka.isPlaying)
                {
                    sosalka.Stop();
                    sosalka.volume = 0f;
                    wasPlaying = false;
                }
            }
            else
            {
                if (inventoryScroll.coolingTime >= 1f) 
                {
                    wasOverheated = false;
                }

                if (wasPlaying)
                {
                    switcher?.Play();
                    off?.Play();
                    wasPlaying = false;
                }

                if (sosalka.isPlaying)
                {
                    sosalka.volume = 0f;
                    sosalka.Stop();
                }

                if (isOverheatSoundPlaying)
                {
                    overheat.Stop();
                    isOverheatSoundPlaying = false;
                }
            }
        }
        }
    void HandleWalkSound()
    {

        float speed = playerController.inputVector.magnitude;
    Debug.Log("Speed: " + speed);
        if (speed > 0.05f && speed < 9f)
        {
            if (!slowWalkSound.isPlaying)
            {
                slowWalkSound.Play();
            }

            if (walkSound.isPlaying)
            {
                walkSound.Stop();
            }
        }
        else if (speed >= 9f)
        {
            if (!walkSound.isPlaying)
            {
                walkSound.Play();
            }

            if (slowWalkSound.isPlaying)
            {
                slowWalkSound.Stop();
            }
        }
        else 
        {
            if (walkSound.isPlaying)
            {
                walkSound.Stop();
            }
            if (slowWalkSound.isPlaying)
            {
                slowWalkSound.Stop();
            }
        }
    }

    void StopAllSounds()
    {
        sosalka?.Stop();
        overheat?.Stop();
        walkSound?.Stop();
        off?.Stop();
        switcher?.Stop();
        pop?.Stop();

        if (sosalka != null) sosalka.volume = 0f;
        isOverheatSoundPlaying = false;
        wasPlaying = false;
    }
}