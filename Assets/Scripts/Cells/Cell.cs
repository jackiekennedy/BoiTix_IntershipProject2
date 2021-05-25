using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    // Переменные по умолчанию при старте ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Конфигурация клетки
    [SerializeField] private CellConfig cellConfig;

    // Статус клетки (захвачена, нейтральна)
    [Space]
    [SerializeField] private CellStates.States defaultCellState;

    // Владелец по умолчанию, если клетка изначально захвачена
    [SerializeField] private Player defaultCellOwner;

    // Спрайты центра и круга выделения для перекраса по мере игры и выделения
    [Space]
    [SerializeField] private SpriteRenderer cellCenter;
    [SerializeField] private SpriteRenderer selectionCircle;
    [SerializeField] private SpriteRenderer selectionTarger;
    [SerializeField] private TextMeshPro pointsCountText;
    

    // Нейтральный цвет по умолчанию
    [Space]
    private readonly Color cellNeutralColor = default;

    // Динамические переменные ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Статус клетки
    private CellStates.States cellState;

    // Владелец клетки
    private Player cellOwner;

    // Очки
    [SerializeField]
    private int pointsCount;
    private int maxPointsCount;

    // Событие, если клетка меняет количество очков
    private readonly UnityEvent onPointsChanged = new UnityEvent();

    // Выбрана ли клетка
    private bool isSelected;

    public void Start()
    {
        SetDefualtState();
        onPointsChanged.AddListener(PointVizualization);
    }

    // Настройка состояния клетки на момент старта игры
    private void SetDefualtState()
    {
        // Если клетка по умолчанию захвачена
        if (defaultCellState == CellStates.States.Captured)
        {
            // Проверка на то, назначен ли Владелец в инспекторе
            if (defaultCellOwner != null)
                cellOwner = defaultCellOwner;
            else throw new System.Exception("Cell has 'Captured' state, but owner is not assigned");

            ReColorCell(cellOwner.CellCenterColor);
        }
        else
        {
            cellOwner = null;
        }

        // Устанавливаем состояние клетки
        cellState = defaultCellState;

        // Установка очков по умолчанию
        pointsCount = cellConfig.DefaultPointsCount;
        maxPointsCount = cellConfig.MaxPointsCount;

        // Визуализируем кол-во очков
        pointsCountText.text = pointsCount.ToString(); 
    }

    // Перекрас клетки
    private void ReColorCell(Color cellCenterColor)
    {
        this.cellCenter.color = cellCenterColor;
    }

    // Возврат клетки в нейтральное состояние
    private void ToNeutralState()
    {
        cellState = CellStates.States.Neutral;
        cellOwner = null;
        ReColorCell(cellNeutralColor);
    }

    // Перевод клетки в захваченное состояние
    private void ToCaptureState(Player playerInvader)
    {
        cellState = CellStates.States.Captured;
        cellOwner = playerInvader;
        ReColorCell(cellOwner.CellCenterColor);
    }

    // Визуализация кол-ва очков
    private void PointVizualization()
    {
        pointsCountText.text = pointsCount.ToString();
    }

    public void Tick()
    {
        // Проверка на то, захвачена ли клетка
        if (cellState == CellStates.States.Captured)
        {
            // Наращиваем очки, если их меньше, чем допустимая велечина, иначе отнимаем
            if (pointsCount < maxPointsCount)
                pointsCount += 1;
            else if (pointsCount > maxPointsCount) 
                pointsCount -= 2;

            onPointsChanged?.Invoke();
        }
    }

    //////////////////////////////////// Получение информации ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Player GetOwner()
        => cellOwner;

    public bool GetSelectStatus()
        => isSelected;


    //////////////////////////////////// Игровая логика //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void CellSelect()
    {
        isSelected = true;
        selectionCircle.GetComponent<SpriteRenderer>().enabled = true; 
    }

    public void BotCellSelect()
    {
        isSelected = true;
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

            ParticlesCreator.Instance.ParticleInstantiation(this.cellOwner, this.gameObject, cellRecipient.gameObject, transferPointsCount);
        }   
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // Если в клетку прилетела частица
        if (other.tag == "Particle" && other.GetComponent<Particle>().GetCellRecipient() == this.gameObject)
        {
            var particle = other.GetComponent<Particle>();

            GetParticle(particle);
            GameObject.Destroy(particle.gameObject);
        }
    }

    public void GetParticle(Particle particle)
    {
        if (pointsCount >= 1)
        {
            GetPoint(particle);
        }
        else
        {
            if (cellState == CellStates.States.Neutral)
                cellState = CellStates.States.Captured;

            ChangeOwner(particle);
            GetPoint(particle);
        }

        onPointsChanged?.Invoke();
    }

    private void GetPoint(Particle particle)
    {
        // Если частицы той же команды, что и клетка, то прибавить очко, иначе отнять
        if (particle.GetParticleOwner() == cellOwner)
            pointsCount++;
        else
            pointsCount--;
    }

    public void ChangeOwner(Particle particle)
    {
        cellOwner = particle.GetParticleOwner();
        ReColorCell(cellOwner.CellCenterColor);

        // Если клетка была выделена игроком, но её перехватили, то сбросить выделение
        if (isSelected)
            CellUnselect();

        // Если у клетки отрисовывался лайн рендерер во время перехвата, то убираем
        if (gameObject.GetComponent<LineSetup>().GetSelectStatus())
            gameObject.GetComponent<LineSetup>().UnSelectLine();

        // Проверка на вин
        WinTracker.Instance.CheckForSomebodyWin();
    }

    public void DrawTarget()
    {
        StartCoroutine(DrawTargetCor());
    }

    private IEnumerator DrawTargetCor()
    {
        selectionTarger.enabled = true;
        yield return new WaitForSeconds(1);
        selectionTarger.enabled = false;
    }


}
