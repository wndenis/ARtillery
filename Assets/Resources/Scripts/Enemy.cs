using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float hp = 1;
    public float maxHp;
    public Transform gun;
    public Transform gunTip;
    public GameObject gunParticles;
    public float attackInterval = 2f;
    [Space]
    public Bullet bullet;
    public float bulletSpeed;

    public GameObject deathParticles;
        
    //[HideInInspector]
    public Transform player;
    
    protected NavMeshAgent _navMeshAgent;
    protected int scoreCost = 1;
    
    // Start is called before the first frame update
    protected void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        maxHp = hp;
        StartCoroutine(Brain());
    }

    public void Damage(float amount = 1f)
    {
        if (hp > 0)
        {
            hp -= amount;
            if (hp <= 0)
                Die();
        }
    }

    protected void Die()
    {
        PrepareToDie();
        player.GetComponent<Player>().score += scoreCost;
        Instantiate(deathParticles, transform.parent).transform.position = transform.position;
        Destroy(gameObject, 0.15f);
    }


    protected virtual void PrepareToDie()
    {
        
    }

    // Update is called once per frame
    protected virtual IEnumerator Brain()
    {
        yield return new WaitForSeconds(0.1f);
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(player.position);
        yield return new WaitForSeconds(0.2f);
        var lastDestinationTime = Time.time;
        while (true)
        {
            var dist = _navMeshAgent.remainingDistance;
            var travelledTime = Time.time - lastDestinationTime;
            if (_navMeshAgent.pathStatus==NavMeshPathStatus.PathComplete && dist < 0.01 && !float.IsPositiveInfinity(dist) || travelledTime > 3 * attackInterval)
            {
                for (var t = 0f; t < 0.35; t += Time.deltaTime)
                {
                    var rot = Quaternion.LookRotation(player.position - gun.position);
                    gun.rotation = Quaternion.Lerp(gun.rotation, rot, t / 0.5f);
                    yield return null;
                }
                gun.rotation = Quaternion.LookRotation(player.position - gun.position);
                
                var obj = Instantiate(gunParticles, transform.parent);
                obj.transform.position = gunTip.position;
                obj.transform.rotation = gunTip.rotation;
                
                var b = Instantiate(bullet, transform.parent);
                b.speed = bulletSpeed;
                b.direction = player.position - gunTip.position;
                b.transform.position = gunTip.position;
                
                yield return new WaitForSeconds(0.25f);
                for (var t = 0f; t < 0.35; t += Time.deltaTime)
                {
                    var rot = Quaternion.LookRotation(transform.forward);
                    gun.rotation = Quaternion.Lerp(gun.rotation, rot, t / 0.5f);
                    yield return null;
                }
                gun.rotation = Quaternion.LookRotation(transform.forward);
                
                _navMeshAgent.SetDestination(transform.parent.position + Random.insideUnitSphere * 0.2f);
                lastDestinationTime = Time.time;
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }
}
