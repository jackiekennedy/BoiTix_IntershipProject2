using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelSettings : Singleton<LevelSettings>
{
    [SerializeField] private Player userPlayer;
    [SerializeField] private LevelProgress level;

    private List<Cell> allLevelCells;

    public override void AwakeInit()
    {
        allLevelCells = Object.FindObjectsOfType<Cell>().ToList();
    }

    public List<Cell> GetAllLevelCells()
        => allLevelCells;

    public Player GetPlayer()
        => userPlayer;

    public LevelProgress GetLevelProgress()
        => level;
}
