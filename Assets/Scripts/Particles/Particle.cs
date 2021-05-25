using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private GameObject cellRecipient;
    private Player particleOwner;
    private Color particleColor;

    public void AttachInformation(GameObject cellRecipient, Player particleOwner)
    {
        this.cellRecipient = cellRecipient;
        this.particleOwner = particleOwner;

        particleColor = particleOwner.PlayerConfig.CellCenterColor;
        RecolorParticle(particleColor);
    }

    private void RecolorParticle(Color color)
    {
        Color particleSprite = GetComponent<SpriteRenderer>().color;

        particleSprite = color;
        particleSprite.a = 0.7f;

        GetComponent<SpriteRenderer>().color = particleSprite;
    }

    public Player GetParticleOwner()
        => particleOwner;

    public GameObject GetCellRecipient()
        => cellRecipient;
}
