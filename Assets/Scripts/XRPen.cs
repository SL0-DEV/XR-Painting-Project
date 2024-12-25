using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class XRPen : XRGrabInteractable
{
    [Header("Pen State")]
    public bool isDrawing = false;
    public bool CanDraw = false;
    [Space]
    [Header("Pen Properties")]
    public float DrawWidth = .2f;
    public LayerMask DrawableLayer;
    public Transform TipTransform;
    public Vector3 DrawOffset = Vector3.zero;
    public Color CurrentColor;
    [Space]
    [Header("Pen Visual")]
    public Material DrawMaterial;
    public Material TipMaterial;

    public GameObject DrawPrefab;

    [Space]
    [Header("Pen Menu")]
    public GameObject PenMenu;
    public Image WidthPreview;
    [Space]
    [Header("Hands Input")]
    public InputActionProperty LeftHandPenMenu;
    public InputActionProperty RightHandPenMenu;


    private int lastIndex = 0;
    private int lastSortOrder = 0;

    private bool isUsingLeftHand = false;

    private LineRenderer currentdraw;

    private Vector3 lastDrawPosition;


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        CanDraw = true;
        // We need to check which hand player used to use pen menu !
        isUsingLeftHand = args.interactorObject.handedness == UnityEngine.XR.Interaction.Toolkit.Interactors.InteractorHandedness.Left;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        // We will check if we can draw by holding the pen
        if (!CanDraw) return;
        if (currentdraw == null)
        {
            // Set up the line
            ImplementLineRenderer();
        }
        // We will change the state of drawing to draw lines
        isDrawing = true;
    }
    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        // Change the state of drawing to make sure the pen won't draw lines :)
        isDrawing = false;
        // Before we split the line we will make sure if the line is valid :)
        if (currentdraw == null) return;
        SplitLine();
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // We change the state of drawing ability :)
        CanDraw = false;
    }
    /// <summary>
    /// This method to set new color for the draw and we will call the it by another classes
    /// </summary>
    /// <param name="clr"></param>
    public void SetColor(Color clr)
    {
        CurrentColor = clr;
        TipMaterial.color = clr;
    }
    /// <summary>
    /// This method to set the draw method from another classes
    /// </summary>
    /// <param name="slider"></param>
    public void SetWidth(Slider slider)
    {
        DrawWidth = slider.value;
        WidthPreview.transform.localScale = Vector3.one * (slider.value + .2f / slider.maxValue);
    }
    private void Update()
    {
        Draw();
        PenMenuManager();
    }
    /// <summary>
    /// This method for showing the pen menu to control the draw width
    /// </summary>
    private void PenMenuManager()
    {
        if (isUsingLeftHand)
        {
            if (LeftHandPenMenu.action.IsPressed())
            {
                PenMenu.SetActive(!PenMenu.activeSelf);
            }
        }
        else
        {
            if (RightHandPenMenu.action.IsPressed())
            {
                PenMenu.SetActive(!PenMenu.activeSelf);
            }
        }
    }
    /// <summary>
    /// This method will draw lines on the board.
    /// We will draw using Line Renderer componenet
    /// </summary>
    private void Draw()
    {
        // Before drawing we need to give color of the line
        TipMaterial.color = CurrentColor;
        if (!isDrawing || currentdraw == null) return;
        if (Physics.Raycast(TipTransform.position,TipTransform.forward,out RaycastHit hit,1.2f,DrawableLayer))
        {
            // We make sure the pen is near to the board by checking the distance
            if (hit.distance > .3f) return;
            // before every thing we make sure if the line hase positions
            if (currentdraw.positionCount > 0)
            {
                // We need to make sure is the next line is not like the old line :)
                if (Vector3.Distance(currentdraw.GetPosition(lastIndex), hit.point) < 0.01f) return;

                currentdraw.startColor = CurrentColor;
                currentdraw.endColor = CurrentColor;

                // We need to make sure is the first point of the line renderer is on the board
                if (currentdraw.GetPosition(0) == Vector3.zero)
                {
                    currentdraw.SetPosition(0, hit.point);
                }

                currentdraw.positionCount = lastIndex + 1;
                // This step is for line offset
                Vector3 newpos = hit.point - DrawOffset;
                currentdraw.SetPosition(lastIndex, newpos);
                lastIndex++;
            }
            else
            {
                // First line of the line renderer
                currentdraw.startColor = CurrentColor;
                currentdraw.endColor = CurrentColor;
                currentdraw.positionCount = 1;
                Vector3 newpos = hit.point - DrawOffset;
                currentdraw.SetPosition(lastIndex, newpos);
            }
            // We always give the last draw position for split line method
            lastDrawPosition = transform.position;
        }
    }
    /// <summary>
    /// This method will setup the line for drawing.
    /// </summary>
    private void ImplementLineRenderer()
    {
        // We will adding LineRenderer componenet on the pen to give ability of drawing 
        currentdraw = gameObject.AddComponent<LineRenderer>();
        // Give the LineRenderer properties
        currentdraw.material = DrawMaterial;
        currentdraw.startWidth = DrawWidth;
        currentdraw.endWidth = DrawWidth;
        currentdraw.startColor = CurrentColor;
        currentdraw.endColor = CurrentColor;
        currentdraw.sortingOrder = lastSortOrder;
        currentdraw.numCornerVertices = 12;
    }

    /// <summary>
    /// Split the last line drew on the board and make it a individual line.
    /// After we split the line we will add the line to the Board Manager to give the board fully control
    /// </summary>
    private void SplitLine()
    {
        // We will spawn a gameobject with LineRenderer componenet and give them the same properties of last line drew
        LineRenderer line = Instantiate(DrawPrefab, lastDrawPosition, Quaternion.identity).GetComponent<LineRenderer>();
        line.material = currentdraw.material;
        line.startColor = currentdraw.startColor;
        line.endColor = currentdraw.endColor;
        line.positionCount = currentdraw.positionCount;
        line.numCornerVertices = currentdraw.numCornerVertices;
        line.endWidth = currentdraw.endWidth;
        line.startWidth = currentdraw.startWidth;
        line.sortingOrder = lastSortOrder;
        for(int i =0; i < currentdraw.positionCount; i++)
        {
            // We will load all line positions from last line drew
            line.SetPosition(i, currentdraw.GetPosition(i));
        }
        // We will add the line to BoardManager to get fully control of the lines
        BoardManager.Instance.AddLine(line);
        Destroy(currentdraw);
        currentdraw = null;
        lastIndex = 0;
        // We will increase the sort order to make new layer of line
        lastSortOrder++;
    }
}
