using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public void ChangeSliderValue(int value)
    {
        GetComponent<Slider>().value = value;
    }
}
