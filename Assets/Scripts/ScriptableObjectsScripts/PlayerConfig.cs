using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "My Menu/new PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public int PlayerId;
    public Color CellCenterColor;
}
