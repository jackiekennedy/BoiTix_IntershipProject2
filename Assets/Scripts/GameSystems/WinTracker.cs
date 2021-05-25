using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WinTracker : Singleton<WinTracker>
{
    private List<Cell> allLevelCells;
    private LevelProgress levelProgress;

    void Start()
    {
        levelProgress = LevelSettings.Instance.GetLevelProgress();
        allLevelCells = LevelSettings.Instance.GetAllLevelCells();

        //PlayerPrefs.SetInt("LevelProgress" + levelProgress.GetLevelID(), false ? 1 : 0);

        levelProgress.LoadLevelProgressStatus();
    }

    public void CheckForSomebodyWin()
    {
        // Все клетки, за исключением нейтральных
        var allCellsWithoutNeutral = allLevelCells.Where(player => player.GetOwner() != null).ToList();
        // Все ли захваченные клетки имеют одного владельца?
        var isAllHasOneOwner = allCellsWithoutNeutral.All(player => player.GetOwner() == allCellsWithoutNeutral[0].GetOwner());

        if (isAllHasOneOwner)
        {
            var winner = allCellsWithoutNeutral[0].GetOwner();

            // Если победил юзер
            if (winner.Equals(LevelSettings.Instance.GetPlayer()))
            {
                SetLevelComplete();
                LevelMenu.Instance.EnableEndGameMenu(true);
            }
            else
            {
                LevelMenu.Instance.EnableEndGameMenu(false);
            }

        }
    }

    public void SetLevelComplete()
    {
        levelProgress.SetLevelComplete();
        levelProgress.SaveLevelProgressSatus();
    }
}
