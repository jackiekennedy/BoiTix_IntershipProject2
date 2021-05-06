using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControl : MonoBehaviour
{
    // �����, ������� ������ ����
    [SerializeField]
    private Player myPlayer;

    // ������, �� ��������� ��� ����������� ��������� ������/������
    private UnityEvent onSelect = new UnityEvent();
    private UnityEvent onUnselect = new UnityEvent();
    
    // �������, ������� ����� �������, ����� ���� ������� ������ ��� ��������� � �� �����
    private UnityEvent onTransferShouldStart = new UnityEvent();

    // ���� ��������� ������
    private List<Cell> selectedCells = new List<Cell>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 CurMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(CurMousePos, Vector2.zero);

            // ���� ������� �� ������, �� ���������, ����� ����� ��������� �� ���� ������, ���� �������
            if (rayHit.collider != null && rayHit.collider.tag == "Cell")
            {
                var workCell = rayHit.collider.GetComponent<Cell>();
                ChosenCellTreatment(workCell);
            }
            else
            {
                CellsUnselect();
            }
        }
    }

    private void ChosenCellTreatment(Cell workCell)
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
    }

    private void CellSelect(Cell workCell)
    {
        // ����������� ���������� ������ � ������� ���������, �������� � ������ ������ �������
        onSelect.AddListener(workCell.CellSelect);
        onSelect?.Invoke();
        onSelect.RemoveAllListeners();

        // ��������� ������ � ���� ��������� ������
        selectedCells.Add(workCell);
    }

    private void TransferToChosenCell(Cell workCell)
    {
        // ���� ������ ���������� ������ �� ����, �� ����������� ������ ������ �� ������� ��������� ����� � ������, �� ������� �������
        if (selectedCells.Count > 0)
            foreach (Cell selectedCell in selectedCells)
                onTransferShouldStart.AddListener(delegate { selectedCell.StartTransfer(workCell); });

        // �������� ����� ��������� 
        onTransferShouldStart?.Invoke();
    }

    // ����� ��������� ������
    private void CellsUnselect()
    {
        // ����������� ��� ��������� ������ �� ������� ������ ���������
        foreach (Cell selectedCell in selectedCells)
            onUnselect.AddListener(selectedCell.CellUnselect);

        // ���������� ��� ��������� ������
        onUnselect?.Invoke();

        // ������ ������� ������ ��������� ������, ������ ���������� ������ � ������� ������ ������� � ���������
        onUnselect.RemoveAllListeners();
        onTransferShouldStart.RemoveAllListeners();
        selectedCells.Clear();
    }
}
