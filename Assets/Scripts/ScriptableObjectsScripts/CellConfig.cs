using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellConfig", menuName = "My Menu/new CellConfig")]
public class CellConfig : ScriptableObject
{
    public int MaxPointsCount;
    public int DefaultPointsCount;
}
