using UnityEngine;
using UnityEngine.UI;
public class ColorMaker : MonoBehaviour
{
    public static ColorMaker Instance;

    [Header("Color Properties")]
    public float Red;
    public float Green;
    public float Blue;

    public Color CurrentColor;
    [Space]

    [Header("Color Visual")]
    public Image ColorPreview;



    /// <summary>
    /// When slider value changed we will change Red Value
    /// </summary>
    /// <param name="slider"></param>
    public void ChangeRed(Slider slider)
    {
        Red = slider.value;
    }
    /// <summary>
    /// When slider value changed we will change Blue Value
    /// </summary>
    /// <param name="slider"></param>
    public void ChangeBlue(Slider slider)
    {
        Blue = slider.value;
    }
    /// <summary>
    /// When slider value changed we will change Green Value
    /// </summary>
    /// <param name="slider"></param>
    public void ChangeGreen(Slider slider)
    {
        Green = slider.value;
    }
    /// <summary>
    /// This method will call from unity event or button event to get the button image color
    /// </summary>
    /// <param name="img"></param>
    public void SetColorByImage(Image img)
    {
        CurrentColor = img.color;
        ColorPreview.color = img.color;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        // We will divide the values by 255 to make sure we are adding maximum value of RGB
        CurrentColor = new Color(Red / 255, Green / 255, Blue / 255);
        // We will update the color preview on the UI
        ColorPreview.color = CurrentColor;
    }
}
