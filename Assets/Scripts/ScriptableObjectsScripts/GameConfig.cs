using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "GameConfig", menuName = "My Menu/new GameConfig")]
public class GameConfig : ScriptableObject
{
    // Лист всех прогрессов уровней
    [SerializeField] private List<LevelProgress> levelsProgress;


    public void LoadGame()
    {
        if (levelsProgress.Count > 0)
            foreach (var levelProgress in levelsProgress)
            {
                var levelId = levelProgress.GetLevelID();
                levelProgress.SetLevelStatus(PlayerPrefs.GetInt("LevelProgress" + levelId) != 0);
            }
    }

    public int GetLevelsCount()
        => levelsProgress.Count;

    public List<LevelProgress> GetListOfAllLevels()
        => levelsProgress;

    public int GetLastCompleteLevel()
    {
        int lastCompleteLevel = default;
        
        foreach(var level in levelsProgress)
        {
            if (level.GetLevelProgressStatus() != false)
                lastCompleteLevel = level.GetLevelID();
        }

        return lastCompleteLevel;
    }
}
