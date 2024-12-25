using UnityEngine;
using System.Collections.Generic;
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board Properties")]
    public Transform BoardTransform;
    public Color BoardColor = Color.white;

    private MeshRenderer boardMesh;

    protected List<LineRenderer> linesRenderer = new List<LineRenderer>();
    protected List<GameObject> sprayCircles = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boardMesh = GetComponent<MeshRenderer>();
        boardMesh.material.color = BoardColor;
    }
    /// <summary>
    /// Setting board color by another classes
    /// </summary>
    /// <param name="color"></param>
    public void SetBoardColor(Color color)
    {
        BoardColor = color;
        boardMesh.material.color = BoardColor;
    }
    /// <summary>
    /// This method for getting all lines have drawn on the board.
    /// </summary>
    /// <param name="line"></param>
    public void AddLine(LineRenderer line)
    {
        linesRenderer.Add(line);
    }
    /// <summary>
    /// This method to add spray circle to a list for getting fully control of all circles drew
    /// </summary>
    /// <param name="Spray"></param>
    public void AddSpray(GameObject Spray)
    {
        sprayCircles.Add(Spray);
    }
    /// <summary>
    /// This method for erasing nearest lineposition, but we won't remove the whole line. We only remove specific line.
    /// </summary>
    /// <param name="pos"></param>
    public void EraseNearestLinePosition(Vector3 pos)
    {
        if (linesRenderer.Count == 0) return;
        foreach (var ls in linesRenderer)
        {
            if (ls.positionCount == 0)
            {
                linesRenderer.Remove(ls);
                Destroy(ls.gameObject);
                return;
            }

            List<Vector3> linespos = new List<Vector3>();
            Vector3[] lp = new Vector3[ls.positionCount];
            ls.GetPositions(lp);
            for (int i = 0; i < lp.Length; i++)
            {
                linespos.Add(lp[i]);
            }
            foreach (var ps in linespos)
            {
                if (Vector3.Distance(pos, ps) < .2f)
                {
                    ls.Simplify(0);
                    linespos.Remove(linespos[linespos.IndexOf(ps)]);
                    ls.positionCount = linespos.Count;
                    //linespos.Sort();
                    ls.SetPositions(linespos.ToArray());
                }
            }
        }




    }

    /// <summary>
    /// Erase the whole line by finding relative lineposition
    /// Erase nearest spray circle
    /// </summary>
    /// <param name="pos"></param>
    public void EraseAllLine(Vector3 pos)
    {
        EraseNearestCircle(pos);
        if (linesRenderer.Count == 0) return;
        foreach (var ls in linesRenderer)
        {
            if (ls.positionCount == 0)
            {
                linesRenderer.Remove(ls);
                Destroy(ls.gameObject);
                return;
            }
            // We need array to take all positions on line renderer
            Vector3[] lp = new Vector3[ls.positionCount];
            ls.GetPositions(lp);
            foreach (var ps in lp)
            {
                // We will find nearest position to remove whole line
                if (Vector3.Distance(pos, ps) < .2f)
                {
                    linesRenderer.Remove(ls);
                    Destroy(ls.gameObject);
                }
            }
        }
    }
    /// <summary>
    /// Erase nearest circle to the target position
    /// </summary>
    /// <param name="pos"></param>
    public void EraseNearestCircle(Vector3 pos)
    {
        if (sprayCircles.Count == 0) return;
        foreach (var sp in sprayCircles)
        {
            if (sp == null) return;
            if (Mathf.Abs((pos - sp.transform.position).magnitude) < .22f)
            {
                sprayCircles.Remove(sp);
                Destroy(sp.gameObject);
            }
        }

    }
    /// <summary>
    /// Change the nearest circle to the target position to new color
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="color"></param>
    public void ChangeCircleColor(Vector3 pos, Color color)
    {
        if (sprayCircles.Count == 0) return;
        foreach (var sp in sprayCircles)
        {
            if (Mathf.Abs((pos - sp.transform.position).magnitude) < .02f && sp.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
            {
                if (renderer.color == color) return;
                renderer.color = color;
            }
        }
    }
    /// <summary>
    /// Check if we have nearerst circle on the board
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckNearestCircle(Vector3 pos)
    {
        if (sprayCircles.Count == 0) return false;
        foreach (var sp in sprayCircles)
        {
            if (sp == null) return false;
            if (Mathf.Abs((pos - sp.transform.position).magnitude) < .02f)
            {
                return true;
            }
        }
        return false;
    }
}
