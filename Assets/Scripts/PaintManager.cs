using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public XRPen Pen;
    public XRSpray Spray;

    //<summary>
    // This method for changing the color of the handling pen or spray
    //<summary>
    public void SetPaintColor()
    {
        if (Pen.CanDraw)
        {
            Pen.SetColor(ColorMaker.Instance.CurrentColor);
        }
        if (Spray.CanDraw)
        {
            Spray.SetColor(ColorMaker.Instance.CurrentColor);
        }
    }
}
