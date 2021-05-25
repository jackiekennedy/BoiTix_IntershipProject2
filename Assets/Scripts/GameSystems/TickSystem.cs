using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class TickSystem : MonoBehaviour
{
    // Интервал Тика
    [SerializeField] private float timeToTick;

    // Событие, которое вызывается каждый тик
    private UnityEvent onTick = new UnityEvent();

    void Start()
    {
        // Получаем все объекты помеченые тегом "Клетка"
        var allCells = GameObject.FindGameObjectsWithTag("Cell").ToList();

        // Подписываем все клетки на событие Тик
        if (allCells != null)
            foreach (var x in allCells)
                onTick.AddListener(x.GetComponent<Cell>().Tick);

        // Если кто-то установил время Тика меньше/равно нуля
        if (timeToTick <= 0f)
        {
            timeToTick = 1f;
            Debug.LogWarning("Tick time is less than zero, this is incorrect. Tick time changed to 1f");
        }

        // Запускаем Тик
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
