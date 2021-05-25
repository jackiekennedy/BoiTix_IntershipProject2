using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerConfig PlayerConfig;

    public int PlayerId { get; private set; }
    public Color CellCenterColor { get; private set; }

    public void Awake()
    {
        this.PlayerId = PlayerConfig.PlayerId;
        this.CellCenterColor = PlayerConfig.CellCenterColor;
    }

    public override bool Equals(object other)
    {
        var otherPlayer = other as Player;
        return PlayerId == otherPlayer.PlayerId ? true : false;
    }

    public override int GetHashCode()
        => PlayerId;
}
