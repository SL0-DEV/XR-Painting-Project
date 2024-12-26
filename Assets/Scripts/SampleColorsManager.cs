using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class SampleColorsManager : MonoBehaviour
{
    private Image[] colorSamples;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorSamples = transform.GetComponentsInChildren<Image>();
        RandmoizeColors();
    }

    public void RandmoizeColors()
    {
        for (int i = 0; i < colorSamples.Length; i++)
        {
            colorSamples[i].color = Random.ColorHSV(.2f,1);
        }
    }
}
