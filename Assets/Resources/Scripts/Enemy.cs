using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform gun;
    public Transform gunTip;
    public GameObject gunParticles;
    public float attackInterval = 2f;
    [Space]
    public Bullet bullet;
    public float bulletSpeed;

    public GameObject deathParticles;
        
    [HideInInspector]
    public Transform player;
    
    private NavMeshAgent _navMeshAgent;
    
    // Start is called before the first frame update
    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(Brain());
    }

    private void Update()
    {
        
    }

    public void Kill()
    {
        Instantiate(deathParticles, transform.parent).transform.position = transform.position;
        Destroy(gameObject, 0.1f);
    }

    // Update is called once per frame
    private IEnumerator Brain()
    {
        yield return new WaitForSeconds(0.1f);
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(player.position);
        yield return new WaitForSeconds(0.5f);
        var lastDestinationTime = Time.time;
        while (true)
        {
            var dist = _navMeshAgent.remainingDistance;
            var travelledTime = Time.time - lastDestinationTime;
            if (_navMeshAgent.pathStatus==NavMeshPathStatus.PathComplete && dist < 0.01 && !float.IsPositiveInfinity(dist) || travelledTime > 3 * attackInterval)
            {
                for (var t = 0f; t < 0.5; t += Time.deltaTime)
                {
                    var rot = Quaternion.LookRotation(player.position - transform.position);
                    gun.rotation = Quaternion.Lerp(gun.rotation, rot, t / 0.5f);
                    yield return null;
                }
                gun.rotation = Quaternion.LookRotation(player.position - transform.position);
                
                var obj = Instantiate(gunParticles, transform.parent);
                obj.transform.position = gunTip.position;
                obj.transform.rotation = gunTip.rotation;
                
                var b = Instantiate(bullet, transform.parent);
                b.speed = bulletSpeed;
                b.direction = player.position - gunTip.position;
                b.transform.position = gunTip.position;
                
                yield return new WaitForSeconds(0.25f);
                for (var t = 0f; t < 0.5; t += Time.deltaTime)
                {
                    var rot = Quaternion.LookRotation(transform.forward);
                    gun.rotation = Quaternion.Lerp(gun.rotation, rot, t / 0.5f);
                    yield return null;
                }
                gun.rotation = Quaternion.LookRotation(transform.forward);
                
                _navMeshAgent.SetDestination(transform.parent.position + Random.insideUnitSphere * 0.42f);
                lastDestinationTime = Time.time;
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }
}
