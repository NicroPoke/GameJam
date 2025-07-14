using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUIsctipts : MonoBehaviour
{
    private List<String> colors = new List<string>()
    {
        "#FF0000", "#00FF00", "#0000FF", "#FFFF00",
        "#FF00FF", "#00FFFF", "#FFFFFF", "#000000"
    };

    private string basicColor = "#6B5353";

    public void ChangeSlotAmount(int slot, int value)
    {
        string childName = "Text" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);

        if (target != null)
        {
            var text = target.GetComponent<TextMeshProUGUI>();

            text.text = Convert.ToString(value);
        }

    }

    public void SetSelected(int slot)
    {
        string childName = "Ghost" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);

        if (target != null)
        {
            var renderer = target.GetComponent<Image>();

            if (ColorUtility.TryParseHtmlString(colors[slot], out Color col))
            {
                renderer.color = col;
            }
            else
            {
                Debug.Log("Failed to do so");
            }
        }
    }

    public void ReturnToBaseColor(int slot)
    {
        string childName = "Ghost" + (slot + 1);
        GameObject target = GetChildRecursive(transform, childName);

        if (target != null)
        {
            var renderer = target.GetComponent<Image>();

            if (ColorUtility.TryParseHtmlString(basicColor, out Color col))
            {
                renderer.color = col;
            }
            else
            {
                Debug.Log("Failed to do so");
            }
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
