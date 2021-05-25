using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControl : MonoBehaviour
{
    // Игрок, которым играет юзер
    private Player myPlayer;
    
    // Событие, которое будет вызвано, когда юзер выберет клетку для трансфера в неё очков
    private UnityEvent onTransferShouldStart = new UnityEvent();

    // Лист выделеных клеток
    [SerializeField] private List<Cell> selectedCells = new List<Cell>();

    // Флаг, для разделения логики тапа и драга в фазе Ended
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

        //    // Если нажатие на клетку, то обработка, иначе сброс выделения со всех клеток, если имеется
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

        // ОБЯЗАТЕЛЬНО УПРОСТИТЬ ЭТИ ЛЕСЕНКИ ИЗ ИФОВ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
        // Если юзер владелец клетки, то...
        if (workCell.GetOwner() == myPlayer)
        {
            // Если клетка до этого не выделена
            if (workCell.GetSelectStatus() != true)
            {
                // Выделяем
                CellSelect(workCell);
            }
        }
    }

    private void TapCellTreatment(Cell workCell)
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
        // Если юзер, не владеет клеткой
        else
        {
            // Отправляем очки из всех выделеных в неё
            TransferToChosenCell(workCell);
            CellsUnselect();
        }
    }

    private void CellSelect(Cell workCell)
    {
        // Переводим клетку в статус выделенной
        workCell.CellSelect();

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

        // Отрисовываем "Цель" на выбранной клетке
        if (selectedCells.Count > 0)
            workCell.DrawTarget();
    }
}
