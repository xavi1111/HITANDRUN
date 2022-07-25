using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public void setCurrentMana(float currentHealt)
    {
        slider.value = currentHealt;
    }
    public float getCurrentMana()
    {
        return slider.value;
    }
    public void setMaxMana(float maxHealt)
    {
        slider.maxValue = maxHealt;
    }
    public float getMaxMana()
    {
        return slider.maxValue;
    }
}
