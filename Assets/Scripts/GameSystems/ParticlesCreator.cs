using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticlesCreator : Singleton<ParticlesCreator>
{
    [SerializeField] private GameObject particlePrefab;

    public void ParticleInstantiation(Player particleOwner, GameObject cellSender, GameObject cellRecipient, int particleCount)
    {
        StartCoroutine(ParticleSoftCreator(particleOwner, cellSender, cellRecipient, particleCount));
    }

    IEnumerator ParticleSoftCreator(Player particleOwner, GameObject cellSender, GameObject cellRecipient, int particleCount)
    {
        for (var i = 0; i < particleCount; i++)
        {
            var particle = GameObject.Instantiate(particlePrefab, GetRandomParticlePos(cellSender), Quaternion.identity);
            particle.GetComponent<Particle>().AttachInformation(cellRecipient, particleOwner);
            particle.transform.DOMove(cellRecipient.transform.position, 10);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private Vector2 GetRandomParticlePos(GameObject cellSender)
    {
        var cellSenderRadius = cellSender.GetComponent<CircleCollider2D>().radius;
        return new Vector2(cellSender.transform.position.x + Random.Range(0, cellSenderRadius), cellSender.transform.position.y + Random.Range(0, cellSenderRadius));
    }
}
