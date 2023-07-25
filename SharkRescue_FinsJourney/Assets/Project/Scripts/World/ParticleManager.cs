using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem bubbleParticleSystem;

    [SerializeField] private float particleSpeed = 50;

    public void AdjustParticleSpeed()
    {
        var main = bubbleParticleSystem.main;
        main.startSpeed = particleSpeed;
    }
}
