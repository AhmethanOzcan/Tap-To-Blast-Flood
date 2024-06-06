using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour
{
    ParticleSystem _particleSystem;

    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    
    void Update()
    {
        if(_particleSystem && !_particleSystem.IsAlive())
            Destroy(this.gameObject);
    }
}
