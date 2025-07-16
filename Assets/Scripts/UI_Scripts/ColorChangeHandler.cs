using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangeHandler : MonoBehaviour
{
    public Sprite[] sprites;
    public Sprite baseSprite;
    private GameObject indicatorBall;
    private Image imageManager;

    void Awake()
    {
        imageManager = GetComponent<Image>();
        indicatorBall = GameObject.Find("Indicator ball");
    }

    public void ChangeColor(string type)
    {
        switch (type)
        {
            case "Contact":
                imageManager.sprite = sprites[0];
                break;
            case "Furry":
                imageManager.sprite = sprites[1];
                break;
            case "Bobj":
                imageManager.sprite = sprites[2];
                break;
            case "Glitch":
                imageManager.sprite = sprites[3];
                break;
            case "Scream":
                imageManager.sprite = sprites[4];
                break;
            case "Toxic":
                imageManager.sprite = sprites[5];
                break;
            case "Electric":
                imageManager.sprite = sprites[6];
                break;
            case "Skeleton":
                imageManager.sprite = sprites[7];
                break;
            case "Angel":
                imageManager.sprite = sprites[8];
                break;
        }
    }

    public void ChangeColor()
    {
        
    }
}
