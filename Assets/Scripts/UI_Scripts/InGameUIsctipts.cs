using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor;

public class InGameUIsctipts : MonoBehaviour
{
    private List<String> colors = new List<string>()
    {
        "#FF0000", "#00FF00", "#0000FF", "#FFFF00",
        "#FF00FF", "#00FFFF", "#FFFFFF", "#000000"
    };

    private string basicColor = "#6B5353";
    public Sprite[] sprites;
    public Sprite[] greyedSprites;

    public Sprite baseSprite;

    void Awake()
    {

    }

    public void ChangeSlotAmount(int slot, int value)
    {
        string childName = "Text" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);
    }

    public void SetSelected(int slot, bool isMoving)
    {
        string childName = "Ghost" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);

        if (target != null)
        {
            var renderer = target.GetComponent<SpriteRenderer>();

            renderer.sprite = greyedSprites[slot];
        }
    }

    public void SetSelected(int slot)
    {
        string childName = "Ghost" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);

        if (target != null)
        {
            var renderer = target.GetComponent<SpriteRenderer>();

            renderer.sprite = sprites[slot];
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        foreach (Sprite sprite in sprites)
        {
            if (sprite != null && sprite.name == name)
                return sprite;
        }

        return null;
    }

    public void ReturnToBaseColor(int slot)
    {
        string childName = "Ghost" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);

        if (target != null)
        {
            var renderer = target.GetComponent<SpriteRenderer>();

            renderer.sprite = baseSprite;
        }
    }

    GameObject GetChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject result = GetChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
