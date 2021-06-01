using UnityEngine;
using UnityEngine.UI;   

public class BlockScreen : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Image blocker;

    /*
     * MonoBehaviour
     */
    private void Awake()
    {
        StopParticles();
    }

    /*
     * Methods
     */
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
