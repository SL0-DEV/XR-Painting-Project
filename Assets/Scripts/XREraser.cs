using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class XREraser : XRGrabInteractable
{
    public LayerMask BoardLayer;
    public bool isErasing = false;


    protected override void OnActivated(ActivateEventArgs args)
    {
        isErasing = true;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        isErasing = false;
    }

    // Update is called once per frame
    void Update()
    {
        Erasing();
    }

    /// <summary>
    /// We will erase by projection direction, if we found nearest line or circle we will remove them from the board
    /// </summary>
    private void Erasing()
    {
        if (!isErasing) return;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, BoardLayer))
        {
            if (hit.distance <= .4f)
            {
                BoardManager.Instance.EraseAllLine(hit.point);
            }
        }
    }
    
}
