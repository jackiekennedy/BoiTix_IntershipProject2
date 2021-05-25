using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererSystem : MonoBehaviour
{  
    // —писок всех лайнов, которые нужно отрисовывавать
    [SerializeField] private List<LineSetup> lineSetups = new List<LineSetup>();

    private Player myPlayer;
    private bool isMovedBefore;

    public void Start()
    {
        myPlayer = LevelSettings.Instance.GetPlayer();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch playerTouch = Input.GetTouch(0);
            Vector2 curUserPos = Camera.main.ScreenToWorldPoint(playerTouch.position);
            RaycastHit2D rayHit = Physics2D.Raycast(curUserPos, Vector2.zero);

            if (playerTouch.phase == TouchPhase.Moved)
            {
                PlayerTouchMoved(rayHit, curUserPos);
            }

            if (playerTouch.phase == TouchPhase.Ended)
            {
                if (isMovedBefore)
                {
                    RemoveAllLines();
                }

                isMovedBefore = false;
            }
        }
    }

    private void PlayerTouchMoved(RaycastHit2D rayHit, Vector2 curUserPos)
    {
        if (rayHit.collider != null && rayHit.collider.tag == "Cell")
        {
            var workLine = rayHit.collider.GetComponent<LineSetup>();
            var workCell = rayHit.collider.GetComponent<Cell>();

            if (workCell.GetOwner() != null && workCell.GetOwner().Equals(myPlayer))
                SelectLine(workLine);
        }

        isMovedBefore = true;
        DrawLines(curUserPos);
    }

    private void SelectLine(LineSetup workLine)
    {
        if (!workLine.GetSelectStatus())
        {
            workLine.SelectLine();
            lineSetups.Add(workLine);
        }    
    }

    private void DrawLines(Vector2 curUserPos)
    {
        foreach (var line in lineSetups)
            if (line.GetSelectStatus())
                line.DrawLine(curUserPos);
    }

    private void RemoveAllLines()
    {
        foreach (var line in lineSetups)
            line.UnSelectLine();
            
        lineSetups.Clear();
    }
}
