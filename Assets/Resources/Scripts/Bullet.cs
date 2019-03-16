using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject deathParticles;
    public Transform trailParticles;
    public float explosionRange = 0.04f;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;

    protected Rigidbody rb;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * speed;
        Destroy(gameObject, 20f);
        Init();
    }

    protected virtual void Init()
    {
        
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (deathParticles)
            Instantiate(deathParticles, transform.parent).transform.position = transform.position;
        var explosion = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (var elem in explosion)
        {
            if (elem.CompareTag("Enemy"))
            {
                var enemy = elem.GetComponent<Enemy>();
                if (enemy)
                    enemy.Damage();
            }
            else if (elem.CompareTag("Bullet"))
            {
                Destroy(elem.gameObject);
            }
            else if (elem.CompareTag("MainCamera"))
            {
                var player = elem.GetComponent<Player>();
                if (player)
                    player.Damage();
            }
        }

        if (trailParticles)
        {
            trailParticles.transform.parent = transform.parent;
            trailParticles.localScale = Vector3.one;
            var particleSystems = trailParticles.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                var emission = ps.emission;
                main.loop = false;
                main.stopAction = ParticleSystemStopAction.Destroy;
                emission.enabled = false;
            }
        }

        Destroy(gameObject);
    }
}
