using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSetup : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool lineSelected;

    public void Start()
    {
        if (gameObject.GetComponent<LineRenderer>() != null)
            lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    public void SelectLine()
        => lineSelected = true;

    public void UnSelectLine()
    {
        lineSelected = false;
        RemoveLine();
    }
        

    public bool GetSelectStatus()
        => lineSelected;

    public void DrawLine(Vector2 curUserPos)
    {
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, curUserPos);
    }

    public void RemoveLine()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = 2;
    }
}
