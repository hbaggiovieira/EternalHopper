using UnityEngine;

public class HandleParticles : MonoBehaviour
{
    public ParticleSystem starsParticles;

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.RunningSpeedLevel >= 2)
        {
            if (!starsParticles.isPlaying)
            {
                starsParticles.Play();
            }
        }
        else
        {
            if (starsParticles.isPlaying)
            {
                starsParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
