using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControl : MonoBehaviour
{
    // �����, ������� ������ ����
    private Player myPlayer;
    
    // �������, ������� ����� �������, ����� ���� ������� ������ ��� ��������� � �� �����
    private UnityEvent onTransferShouldStart = new UnityEvent();

    // ���� ��������� ������
    [SerializeField] private List<Cell> selectedCells = new List<Cell>();

    // ����, ��� ���������� ������ ���� � ����� � ���� Ended
    private bool isMovedBefore;

    public void Start()
    {
        myPlayer = LevelSettings.Instance.GetPlayer();
    }

    void Update()
    {
        #region PcClicks
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    Vector2 CurMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D rayHit = Physics2D.Raycast(CurMousePos, Vector2.zero);

        //    // ���� ������� �� ������, �� ���������, ����� ����� ��������� �� ���� ������, ���� �������
        //    if (rayHit.collider != null && rayHit.collider.tag == "Cell")
        //    {
        //        var workCell = rayHit.collider.GetComponent<Cell>();
        //        ChosenCellTreatment(workCell);
        //    }
        //    else
        //    {
        //        CellsUnselect();
        //    }
        //}
        #endregion

        // ����������� ��������� ��� ������� �� ����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Input.touchCount > 0)
        {
            Touch playerTouch = Input.GetTouch(0);
            Vector2 CurMousePos = Camera.main.ScreenToWorldPoint(playerTouch.position);
            RaycastHit2D rayHit = Physics2D.Raycast(CurMousePos, Vector2.zero);

            if (playerTouch.phase == TouchPhase.Moved)
            {
                if (rayHit.collider != null && rayHit.collider.tag == "Cell")
                {
                    var workCell = rayHit.collider.GetComponent<Cell>();
                    DragCellSelect(workCell);
                }

                isMovedBefore = true;
            }

            if (playerTouch.phase == TouchPhase.Ended)
            {
                if (isMovedBefore)
                {
                    if (rayHit.collider != null && rayHit.collider.tag == "Cell")
                    {
                        var workCell = rayHit.collider.GetComponent<Cell>();
                        TransferToChosenCell(workCell);
                        CellsUnselect();
                    }
                }
                else
                {
                    if (rayHit.collider != null && rayHit.collider.tag == "Cell")
                    {
                        var workCell = rayHit.collider.GetComponent<Cell>();
                        TapCellTreatment(workCell);
                    }
                    else
                    {
                        CellsUnselect();
                    }
                }

                isMovedBefore = false;
            }
        }
    }

    private void DragCellSelect(Cell workCell)
    {
        // ���� ���� �������� ������, ��...
        if (workCell.GetOwner() == myPlayer)
        {
            // ���� ������ �� ����� �� ��������
            if (workCell.GetSelectStatus() != true)
            {
                // ��������
                CellSelect(workCell);
            }
        }
    }

    private void TapCellTreatment(Cell workCell)
    {
        // ���� ���� �������� ������, ��...
        if (workCell.GetOwner() == myPlayer)
        {
            // ���� ������ �� ����� �� ��������
            if (workCell.GetSelectStatus() != true)
            {
                // ��������
                CellSelect(workCell);
            }
            // ���� ������ ��� ���� ��������
            else
            {
                // ���������� ���� �� ���� ��������� � ��
                TransferToChosenCell(workCell);
                CellsUnselect();
            }
        }
        // ���� ����, �� ������� �������
        else
        {
            // ���������� ���� �� ���� ��������� � ��
            TransferToChosenCell(workCell);
            CellsUnselect();
        }
    }

    private void CellSelect(Cell workCell)
    {
        // ��������� ������ � ������ ����������
        workCell.CellSelect();

        // ��������� ������ � ���� ��������� ������
        selectedCells.Add(workCell);
    }

    // ����� ��������� ������
    private void CellsUnselect()
    {
        // ��������� ��� ���������� ������ � ������ ������������
        foreach (Cell selectedCell in selectedCells)
            selectedCell.CellUnselect();

        selectedCells.Clear();
    }

    private void TransferToChosenCell(Cell workCell)
    {
        // ����� ������ ��� � � ����������, �� ������� � �� ������ �������� ���� ������
        if (selectedCells.Contains(workCell))
        {
            selectedCells.Remove(workCell);
            workCell.CellUnselect();
        }


        // ���� ������ ���������� ������ �� ���� � ������ �� ���� ����������� �� ����� � ���������� ���������, �� ����������� ������ ������ �� ������� ��������� ����� � ������, �� ������� �������
        if (selectedCells.Count > 0)
            foreach (Cell selectedCell in selectedCells)
                if (selectedCell.GetSelectStatus())
                onTransferShouldStart.AddListener(delegate { selectedCell.StartTransfer(workCell); });

        // �������� ����� ��������� 
        onTransferShouldStart?.Invoke();
        onTransferShouldStart.RemoveAllListeners();

        // ������������ "����" �� ��������� ������
        if (selectedCells.Count > 0)
            workCell.DrawTarget();
    }
}
