using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControl : MonoBehaviour
{
    // Игрок, которым играет юзер
    [SerializeField]
    private Player myPlayer;

    // Ивенты, на выделение или сбрасывания выделения клетки/клеток
    private UnityEvent onSelect = new UnityEvent();
    private UnityEvent onUnselect = new UnityEvent();
    
    // Событие, которое будет вызвано, когда юзер выберет клетку для трансфера в неё очков
    private UnityEvent onTransferShouldStart = new UnityEvent();

    // Лист выделеных клеток
    private List<Cell> selectedCells = new List<Cell>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 CurMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(CurMousePos, Vector2.zero);

            // Если нажатие на клетку, то обработка, иначе сброс выделения со всех клеток, если имеется
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
        // Если юзер владелец клетки, то...
        if (workCell.GetOwner() == myPlayer)
        {
            // Если клетка до этого не выделена
            if (workCell.GetSelectStatus() != true)
            {
                // Выделяем
                CellSelect(workCell);
            }
            // Если клетка уже была выделена
            else
            {
                // Отправляем очки из всех выделеных в неё
                TransferToChosenCell(workCell);
                CellsUnselect();
            }
            
        }
    }

    private void CellSelect(Cell workCell)
    {
        // Подписываем выделенную клетку в событие выделения, выделяем и чистим список события
        onSelect.AddListener(workCell.CellSelect);
        onSelect?.Invoke();
        onSelect.RemoveAllListeners();

        // Добавляем клетку в лист выделеных клеток
        selectedCells.Add(workCell);
    }

    private void TransferToChosenCell(Cell workCell)
    {
        // Если список выделенных клеток не пуст, то подписываем данные клетки на событие трансфера очков в клетку, на которую тапнули
        if (selectedCells.Count > 0)
            foreach (Cell selectedCell in selectedCells)
                onTransferShouldStart.AddListener(delegate { selectedCell.StartTransfer(workCell); });

        // Вызываем старт трансфера 
        onTransferShouldStart?.Invoke();
    }

    // Сброс выделеных клеток
    private void CellsUnselect()
    {
        // Подписываем все выделеные клетки на событие сброса выделения
        foreach (Cell selectedCell in selectedCells)
            onUnselect.AddListener(selectedCell.CellUnselect);

        // Сбрасываем все выделения клеток
        onUnselect?.Invoke();

        // Чистим событие сброса выделенея клеток, список выделенных клеток и событие клеток готовых к трансферу
        onUnselect.RemoveAllListeners();
        onTransferShouldStart.RemoveAllListeners();
        selectedCells.Clear();
    }
}
