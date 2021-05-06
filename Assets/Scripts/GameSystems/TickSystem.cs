using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class TickSystem : MonoBehaviour
{
    // �������� ����
    [SerializeField] private float timeToTick;

    // �������, ������� ���������� ������ ���
    private UnityEvent onTick = new UnityEvent();

    void Start()
    {
        // �������� ��� ������� ��������� ����� "������"
        var allCells = GameObject.FindGameObjectsWithTag("Cell").ToList();

        // ����������� ��� ������ �� ������� ���
        if (allCells != null)
            foreach (var x in allCells)
                onTick.AddListener(x.GetComponent<Cell>().Tick);

        // ���� ���-�� ��������� ����� ���� ������/����� ����
        if (timeToTick <= 0f)
        {
            timeToTick = 1f;
            Debug.LogWarning("Tick time is less than zero, this is incorrect. Tick time changed to 1f");
        }

        // ��������� ���
        StartCoroutine(TickSecond());
    }

    private IEnumerator TickSecond()
    {
        while (true)
        {
            onTick?.Invoke();
            yield return new WaitForSeconds(timeToTick);
        }
    }
}
