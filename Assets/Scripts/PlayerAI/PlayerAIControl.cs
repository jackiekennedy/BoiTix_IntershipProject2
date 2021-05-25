
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAIControl : MonoBehaviour
{
    // Игрок, которым играет юзер
    [SerializeField] private Player myPlayer;
    [SerializeField] private float botHitTime;

    // Событие, которое будет вызвано, когда юзер выберет клетку для трансфера в неё очков
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
        // Переводим клетку в статус выделенной
        workCell.BotCellSelect();

        // Добавляем клетку в лист выделеных клеток
        selectedCells.Add(workCell);
    }

    // Сброс выделеных клеток
    private void CellsUnselect()
    {
        // Переводим все выделенные клетки в статус невыделенных
        foreach (Cell selectedCell in selectedCells)
            selectedCell.CellUnselect();

        selectedCells.Clear();
    }

    private void TransferToChosenCell(Cell workCell)
    {
        // Еслик клетка моя и в выделенных, то убираем её из списка отдающих очки клеток
        if (selectedCells.Contains(workCell))
        {
            selectedCells.Remove(workCell);
            workCell.CellUnselect();
        }


        // Если список выделенных клеток не пуст и клетка не была перехвачена за время в выделенном состоянии, то подписываем данные клетки на событие трансфера очков в клетку, на которую тапнули
        if (selectedCells.Count > 0)
            foreach (Cell selectedCell in selectedCells)
                if (selectedCell.GetSelectStatus())
                    onTransferShouldStart.AddListener(delegate { selectedCell.StartTransfer(workCell); });

        // Вызываем старт трансфера 
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
