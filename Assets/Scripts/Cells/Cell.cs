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
    [SerializeField] private SpriteRenderer selectionTarger;
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
    [SerializeField]
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
                pointsCount -= 2;

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
        // ���� � ������ ��������� �������
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
        // ���� ������� ��� �� �������, ��� � ������, �� ��������� ����, ����� ������
        if (particle.GetParticleOwner() == cellOwner)
            pointsCount++;
        else
            pointsCount--;
    }

    public void ChangeOwner(Particle particle)
    {
        cellOwner = particle.GetParticleOwner();
        ReColorCell(cellOwner.CellCenterColor);

        // ���� ������ ���� �������� �������, �� � �����������, �� �������� ���������
        if (isSelected)
            CellUnselect();

        // ���� � ������ ������������� ���� �������� �� ����� ���������, �� �������
        if (gameObject.GetComponent<LineSetup>().GetSelectStatus())
            gameObject.GetComponent<LineSetup>().UnSelectLine();

        // �������� �� ���
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
