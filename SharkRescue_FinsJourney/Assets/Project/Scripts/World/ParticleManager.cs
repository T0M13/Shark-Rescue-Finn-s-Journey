using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem bubbleParticleSystem;

    [SerializeField] private float particleSpeed = 50;


    public static ParticleManager Instance { get; private set; }

    private void Start()
    {
        AdjustParticleSpeed(0);
    }

    public void AdjustParticleSpeed(float particleSpeedMultiplicator)
    {
        var main = bubbleParticleSystem.main;
        main.startSpeed = particleSpeed * (1f + 0.1f * particleSpeedMultiplicator);
    }


}
