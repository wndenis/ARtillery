using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.PlayerLoop;

public class EnemyUFO : Enemy
{
    [Space]
    public Transform chargeParticles;
    public float chargeDuration;
    public Enemy minion;
    public int minionCount = 3;

    private void Update()
    {
        transform.Rotate(transform.up, 22 * Time.deltaTime);
    }

    protected override void PrepareToDie()
    {
        scoreCost = 10;
        for (var i = 0; i < minionCount; i++)
        {
            var enemyInstance = Instantiate(minion, transform.parent);
            enemyInstance.player = player;
            enemyInstance.transform.position = transform.position + Random.insideUnitSphere * 0.09f;
        }
    }

    protected override IEnumerator Brain()
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
                var charge = Instantiate(chargeParticles, transform.parent);
                charge.position = gunTip.position;
                yield return new WaitForSeconds(chargeDuration);
                
                var obj = Instantiate(gunParticles, transform.parent);
                obj.transform.position = gunTip.position;
                obj.transform.rotation = gunTip.rotation;
                
                var b = Instantiate(bullet, transform.parent) as BulletUFO;
                b.speed = bulletSpeed;
                b.target = player;
                b.direction = player.position - gunTip.position;
                b.transform.position = gunTip.position;
                
                _navMeshAgent.SetDestination(transform.parent.position + Random.insideUnitSphere * 0.42f);
                lastDestinationTime = Time.time;
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }
}
