using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class XRSpray : XRGrabInteractable
{
    [Header("Spray State")]
    public bool IsDrawing = false;
    public bool CanDraw = false;
    [Space]
    [Header("Board Properties")]
    public LayerMask BoardLayer;
    [Space]
    [Header("Spray Properties")]

    [Tooltip("The quality of painting speed")]
    public float Tolerance = .02f;
    public float SprayWidth = .2f;

    public Transform TipTransform;
    public Vector3 DrawOffset;

    public GameObject Decal;

    [Tooltip("The material of the LineRenderer")]
    public Material SprayColor;
    public Color CurrentColor;




    private int lastSortIndex = 0;
    private Color lastColor;
    private Vector3 lastDrawPos;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        CanDraw = true;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        if (!CanDraw) return;
        IsDrawing = true;
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        IsDrawing = false;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        CanDraw = false;
    }
    private void Update()
    {
        Draw();
    }
    //<summary>
    //This method for drawing on the board. Only on the board!
    //<summary>
    private void Draw()
    {
        SprayColor.color = CurrentColor;
        if (Physics.Raycast(TipTransform.position, TipTransform.forward, out RaycastHit hit, BoardLayer))
        {
            // We will check if we are drawing and near to the board.
            if (hit.distance > .7f || !IsDrawing || Mathf.Abs((hit.point.z - BoardManager.Instance.transform.position.z)) > .12f) return;
            // We will check if we are painting circles in different position and avoiding overlap
            if (Mathf.Abs((lastDrawPos - hit.point).magnitude) >Tolerance)
            {
                Vector3 newpos = hit.point - DrawOffset;
                SpawnCircle(newpos, Quaternion.LookRotation(-hit.normal));
            }
        }
    }
    /// <summary>
    /// This method for spawning spray circle and mixing the circles already on the board
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    private void SpawnCircle(Vector3 pos, Quaternion dir)
    {

        // We will check if we are painting on spray circle or not
        if(BoardManager.Instance.CheckNearestCircle(pos))
        {
            // When we found spray circle face the paint position we will change the color to the new color we paint
            ChangeColor(pos);
            return;
        }

        // we Will make sure the circle is on the correct angle of the board
        if (dir.eulerAngles.y != -180) dir.eulerAngles = new Vector3(0,-180,0);
        // Spawning spray circle to projection direction
        GameObject Spray = Instantiate(Decal, pos, dir);
        // We will give the circle scale
        Spray.transform.localScale = Vector3.one * SprayWidth;
        // For making sure is the SpriteRenderer component is exist we will use TryGetComponenet :)
        if(Spray.TryGetComponent<SpriteRenderer>(out SpriteRenderer sprite))
        {
            sprite.color = CurrentColor;
            // We will check if we are drawing diffrent color to avoid overlaping
            if(lastColor != CurrentColor)
            {
                lastSortIndex++;
            }
            // We will give the circle layer
            sprite.sortingOrder = lastSortIndex;

        }
        // We will take position for making sure, we won't get overlap
        lastDrawPos = Spray.transform.position;
        // Finally we add the circle to BoardManager to get fully control
        BoardManager.Instance.AddSpray(Spray);
        // We will get the color for making sure we won't draw new color on top of them
        lastColor = CurrentColor;
    }
    /// <summary>
    /// This method for getting access to BoardManager for changing existing spray circle
    /// </summary>
    /// <param name="pos"></param>
    private void ChangeColor(Vector3 pos)
    {

        BoardManager.Instance.ChangeCircleColor(pos, CurrentColor);
    }
    /// <summary>
    /// Change the spray color
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        CurrentColor = color;
    }
}
