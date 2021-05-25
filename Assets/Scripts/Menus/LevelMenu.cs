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
    [Header("�������� ���� ����� ����")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button[] buttons;

    [Space]
    [Header("������� �� ����� �������")]
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
        if (LevelSettings.Instance.GetLevelProgress().GetLevelID() == 3 || !win) // ��� ����((( (���-�� ������� �����)
            buttons[1].gameObject.SetActive(false); // ��� ���������� ������ "��������� �������" � ��� ������ ������, �� ������� �������� ����((

        foreach (var b in buttons)
            b.interactable = false;

        if (win)
            titleText.text = "������";
        else
            titleText.text = "��������";

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
