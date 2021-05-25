using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelMenu : Singleton<LevelMenu>
{
    [SerializeField] private Canvas levelMenu;
    [SerializeField] private Canvas endMenu;

    [Space]
    [Header("Ёлементы меню конца игры")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button[] buttons;

    [Space]
    [Header("—обытие на показ рекламы")]
    [SerializeField] public UnityEvent adEvent;

    public void MenuButtonClick()
    {
        EnableCanvas(levelMenu);
    }

    public void ExitLevelMenu()
    {
        DisableCanvas(levelMenu);
    }

    public void EnableEndGameMenu(bool win)
    {
        if (LevelSettings.Instance.GetLevelProgress().GetLevelID() == 3 || !win) // Ёто тоже((( (кол-во уровней всего)
            buttons[1].gameObject.SetActive(false); // Ёто магическа€ кнопка "следующий уровень" и так делать нельз€, но времени осталось мало((

        foreach (var b in buttons)
            b.interactable = false;

        if (win)
            titleText.text = "ѕобеда";
        else
            titleText.text = "ѕроигрыш";

        StartCoroutine(adShowCoroutine());

        EnableCanvas(endMenu);   
    }

    private IEnumerator adShowCoroutine()
    { 
        adEvent?.Invoke();

        yield return new WaitForSecondsRealtime(2);

        foreach (var b in buttons)
            b.interactable = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(LevelSettings.Instance.GetLevelProgress().GetLevelID() + "_Level");
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ToNextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene($"{LevelSettings.Instance.GetLevelProgress().GetLevelID() + 1}_Level");
    }

    private void EnableCanvas(Canvas canvas)
    {
        canvas.enabled = true;
        Time.timeScale = 0;
    }

    private void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
        Time.timeScale = 1;
    } 
}
