using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelProgress", menuName = "My Menu/new LevelProgress")]
public class LevelProgress : ScriptableObject
{
    [SerializeField] private int levelID;
    [SerializeField] private bool levelProgressStatus;

    public bool GetLevelProgressStatus()
        => levelProgressStatus;

    public int GetLevelID()
        => levelID;

    public void SetLevelComplete()
        => levelProgressStatus = true;

    public void SetLevelStatus(bool levelStatus)
        => levelProgressStatus = levelStatus;

    public void SaveLevelProgressSatus()
        => PlayerPrefs.SetInt("LevelProgress" + levelID, levelProgressStatus ? 1 : 0);

    public void LoadLevelProgressStatus()
        => SetLevelStatus(PlayerPrefs.GetInt("LevelProgress" + levelID) != 0);
}
