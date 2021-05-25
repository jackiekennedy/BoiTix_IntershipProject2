
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAIControl : MonoBehaviour
{
    // �����, ������� ������ ����
    [SerializeField] private Player myPlayer;
    [SerializeField] private float botHitTime;

    // �������, ������� ����� �������, ����� ���� ������� ������ ��� ��������� � �� �����
    private UnityEvent onTransferShouldStart = new UnityEvent();

    [SerializeField]  private List<Cell> allLevelCells;
    [SerializeField]  private List<Cell> botCells;
    [SerializeField]  private List<Cell> otherCells;

    private List<Cell> selectedCells = new List<Cell>();

    public void Start()
    {
        allLevelCells = LevelSettings.Instance.GetAllLevelCells();

        StartCoroutine(BotPlaying());
    }

    private void CellSelect(Cell workCell)
    {
        // ��������� ������ � ������ ����������
        workCell.BotCellSelect();

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
    }

    private IEnumerator BotPlaying()
    {
        while (true)
        {
            yield return new WaitForSeconds(botHitTime);

            foreach (var cell in allLevelCells)
            {
                if (cell.GetOwner() != null && cell.GetOwner().Equals(myPlayer))
                    botCells.Add(cell);
                else
                    otherCells.Add(cell);
            }

            Cell botRandomCell = GetRandomCell(botCells);
            Cell otherRandomCell = GetRandomCell(otherCells);

            if (botRandomCell != null && otherRandomCell != null)
            {
                CellSelect(botRandomCell);
                TransferToChosenCell(otherRandomCell);
                CellsUnselect();
            }

            botCells.Clear();
            otherCells.Clear();
        }
    } 
    
    private Cell GetRandomCell(List<Cell> cells)
    {
        Cell cell;

        if (cells.Count != 0)
        {
            if (cells.Count > 1)
                cell = cells[Random.Range(0, cells.Count)];
            else
                cell = cells[0];

            return cell;
        }
        else
            return null;  
    }
}
