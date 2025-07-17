using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ColorChangeHandler : MonoBehaviour
{
    public Sprite[] sprites;
    public Sprite[] bottles;

    public string[] colors = new string[]{
<<<<<<< HEAD
=======
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF",
        "#FFFFFF"
>>>>>>> 003112825dda6d6ebdfe3762e86a331ea8b92ae3
    };

    public Sprite baseSprite;
    private GameObject bottle;
    private GameObject indicatorBall;
    private SpriteRenderer indicatorBottle;
    private SpriteRenderer imageManager;
    private Light2D light2D;

    void Awake()
    {
<<<<<<< HEAD
        bottle = GameObject.Find("bottle2");
=======
        bottle = GameObject.Find("bollte2");
>>>>>>> 003112825dda6d6ebdfe3762e86a331ea8b92ae3
        if (bottle != null) indicatorBottle = bottle.GetComponent<SpriteRenderer>();
        indicatorBall = GameObject.Find("Indicator ball");
        if (indicatorBall != null) imageManager = indicatorBall.GetComponent<SpriteRenderer>();   
    }

    public void ChangeColor(string type)
    {
        bottle.SetActive(true);
        switch (type)
        {
            case "Contact":
                imageManager.sprite = sprites[0];
                indicatorBottle.sprite = bottles[0];
                break;
            case "Furry":
                imageManager.sprite = sprites[1];
                indicatorBottle.sprite = bottles[1];
                break;
            case "Bobj":
                imageManager.sprite = sprites[2];
                indicatorBottle.sprite = bottles[2];
                break;
            case "Glitch":
                imageManager.sprite = sprites[3];
                indicatorBottle.sprite = bottles[3];
                break;
            case "Scream":
                imageManager.sprite = sprites[4];
                indicatorBottle.sprite = bottles[4];
                break;
            case "Toxic":
                imageManager.sprite = sprites[5];
                indicatorBottle.sprite = bottles[5];
                break;
            case "Electric":
                imageManager.sprite = sprites[6];
                indicatorBottle.sprite = bottles[6];
                break;
            case "Skeleton":
                imageManager.sprite = sprites[7];
                indicatorBottle.sprite = bottles[7];
                break;
            case "Angel":
                imageManager.sprite = sprites[8];
                indicatorBottle.sprite = bottles[8];
                break;
        }
    }

    public void ChangeColor()
    {
        imageManager.sprite = baseSprite;
        bottle.SetActive(false);
    }
}