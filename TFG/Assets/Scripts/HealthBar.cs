using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public void setCurrentHealth(float currentHealt)
    {
        slider.value = currentHealt;
    }
    public void setMaxHealth(float maxHealt)
    {
        slider.maxValue = maxHealt;
    }
}
