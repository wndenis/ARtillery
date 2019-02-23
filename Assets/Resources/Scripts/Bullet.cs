using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject deathParticles;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(deathParticles, transform.parent).transform.position = transform.position;
        var explosion = Physics.OverlapSphere(transform.position, 0.04f);
        foreach (var elem in explosion)
        {

            if (elem.CompareTag("Enemy"))
            {
                var enemy = elem.GetComponent<Enemy>();
                if (enemy)
                    enemy.Kill();
            }
            else if (elem.CompareTag("Bullet"))
            {
                Destroy(elem.gameObject);
            }
        }
        Destroy(gameObject);
    }
}
