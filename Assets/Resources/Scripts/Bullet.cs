using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject deathParticles;
    public float explosionRange = 0.04f;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;
    

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 20f);
        Init();
    }

    protected virtual void Init()
    {
        
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
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
        Destroy(gameObject);
    }
}
