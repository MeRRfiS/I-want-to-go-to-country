using UnityEngine;

public sealed class DestroyParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private void Update()
    {
        if (!_particleSystem.IsAlive()) Destroy(gameObject);
    }
}
