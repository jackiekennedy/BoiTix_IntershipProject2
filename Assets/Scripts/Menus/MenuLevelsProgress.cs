using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class MenuLevelsProgress : Singleton<MenuLevelsProgress>
{
    // Лист всех баттонов уровней
    [SerializeField] private List<GameObject> levelButtons;
    // Лист всех прогрессов уровней
    [SerializeField] private List<LevelProgress> levelsProgress;

    public void SetLevelButtonsStatus()
    {
        LoadGame();
        levelsProgress = GetListOfAllLevels();

        var lastCompleteLevel = GetLastCompleteLevel();

        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (i < lastCompleteLevel + 1)
            {
                levelButtons[i].GetComponent<Button>().interactable = true;
                levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = levelsProgress[i].GetLevelID().ToString();
            }
            else
            {
                levelButtons[i].GetComponent<Button>().interactable = false;
                levelButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "?";
            }
        }
    }

    public void LoadGame()
    {
        if (levelsProgress.Count > 0)
            foreach (var levelProgress in levelsProgress)
            {
                var levelId = levelProgress.GetLevelID();
                levelProgress.SetLevelStatus(PlayerPrefs.GetInt("LevelProgress" + levelId) != 0);
            }
    }

    public List<LevelProgress> GetListOfAllLevels()
        => levelsProgress;

    public int GetLastCompleteLevel()
    {
        int lastCompleteLevel = default;

        foreach (var level in levelsProgress)
        {
            if (level.GetLevelProgressStatus() != false)
                lastCompleteLevel = level.GetLevelID();
        }

        return lastCompleteLevel;
    }
}
