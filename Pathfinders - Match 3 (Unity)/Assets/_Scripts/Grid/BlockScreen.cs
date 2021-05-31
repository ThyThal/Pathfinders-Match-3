using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class BlockScreen : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Image blocker;

    private void Awake()
    {
        StopParticles();
    }

    public void StartParticles()
    {
        particles.Play();
        blocker.enabled = true;
    }

    public void StopParticles()
    {
        particles.Stop();
        blocker.enabled = false;
    }
}
