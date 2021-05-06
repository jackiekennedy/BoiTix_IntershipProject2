using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    // ���������� �� ��������� ��� ������ ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // ������������ ������
    [SerializeField] private CellConfig cellConfig;

    // ������ ������ (���������, ����������)
    [Space]
    [SerializeField] private CellStates.States defaultCellState;

    // �������� �� ���������, ���� ������ ���������� ���������
    [SerializeField] private Player defaultCellOwner;

    // ������� ������ � ����� ��������� ��� ��������� �� ���� ���� � ���������
    [Space]
    [SerializeField] private SpriteRenderer cellCenter;
    [SerializeField] private SpriteRenderer selectionCircle;
    [SerializeField] private TextMeshPro pointsCountText;
    

    // ����������� ���� �� ���������
    [Space]
    private readonly Color cellNeutralColor = default;

    // ������������ ���������� ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // ������ ������
    private CellStates.States cellState;

    // �������� ������
    private Player cellOwner;

    // ����
    private int pointsCount;
    private int maxPointsCount;

    // �������, ���� ������ ������ ���������� �����
    private readonly UnityEvent onPointsChanged = new UnityEvent();

    // ������� �� ������
    private bool isSelected;


    public void Start()
    {
        SetDefualtState();
        onPointsChanged.AddListener(PointVizualization);
    }

    // ��������� ��������� ������ �� ������ ������ ����
    private void SetDefualtState()
    {
        // ���� ������ �� ��������� ���������
        if (defaultCellState == CellStates.States.Captured)
        {
            // �������� �� ��, �������� �� �������� � ����������
            if (defaultCellOwner != null)
                cellOwner = defaultCellOwner;
            else throw new System.Exception("Cell has 'Captured' state, but owner is not assigned");

            ReColorCell(cellOwner.CellCenterColor);
        }
        else
        {
            cellOwner = null;
        }

        // ������������� ��������� ������
        cellState = defaultCellState;

        // ��������� ����� �� ���������
        pointsCount = cellConfig.DefaultPointsCount;
        maxPointsCount = cellConfig.MaxPointsCount;

        // ������������� ���-�� �����
        pointsCountText.text = pointsCount.ToString(); 
    }

    // �������� ������
    private void ReColorCell(Color cellCenterColor)
    {
        this.cellCenter.color = cellCenterColor;
    }

    // ������� ������ � ����������� ���������
    private void ToNeutralState()
    {
        cellState = CellStates.States.Neutral;
        cellOwner = null;
        ReColorCell(cellNeutralColor);
    }

    // ������� ������ � ����������� ���������
    private void ToCaptureState(Player playerInvader)
    {
        cellState = CellStates.States.Captured;
        cellOwner = playerInvader;
        ReColorCell(cellOwner.CellCenterColor);
    }

    // ������������ ���-�� �����
    private void PointVizualization()
    {
        pointsCountText.text = pointsCount.ToString();
    }

    public void Tick()
    {
        // �������� �� ��, ��������� �� ������
        if (cellState == CellStates.States.Captured)
        {
            // ���������� ����, ���� �� ������, ��� ���������� ��������, ����� ��������
            if (pointsCount < maxPointsCount)
                pointsCount += 1;
            else if (pointsCount > maxPointsCount) 
                pointsCount -= 1;

            onPointsChanged?.Invoke();
        }
    }

    //////////////////////////////////// ��������� ���������� ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Player GetOwner()
        => cellOwner;

    public bool GetSelectStatus()
        => isSelected;


    //////////////////////////////////// ������� ������ //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void CellSelect()
    {
        isSelected = true;
        selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void CellUnselect()
    {
        isSelected = false;
        selectionCircle.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void StartTransfer(Cell cellRecipient)
    {
        if (pointsCount >= 2)
        {
            var transferPointsCount = pointsCount / 2;
            pointsCount = pointsCount / 2;
            onPointsChanged?.Invoke();

            cellRecipient.GetPoints(transferPointsCount);
        }   
    }

    public void GetPoints(int pointsCountToGet)
    {
        pointsCount += pointsCountToGet;
        onPointsChanged?.Invoke();
    }


}
