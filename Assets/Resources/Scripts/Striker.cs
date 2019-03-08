using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.PlayerLoop;

public class Striker : Enemy
{
    private bool shooting;
    private void Update()
    {
        if (shooting)
        {
            var rot = Quaternion.LookRotation(player.position - gun.position);
            gun.rotation = Quaternion.Lerp(gun.rotation, rot, 0.055f);
        }
    }
    
    protected override void PrepareToDie()
    {
        scoreCost = 3;
    }

    private IEnumerator LogicCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            shooting = !shooting;
            if (!shooting)
            {
                for (var t = 0f; t < 1f; t += Time.deltaTime)
                {
                    var rot = Quaternion.LookRotation(transform.forward);
                    gun.rotation = Quaternion.Lerp(gun.rotation, rot, t);
                    yield return null;
                }
            }
        }
    }

    private IEnumerator VerticalMovement()
    {
        var minOffset = _navMeshAgent.baseOffset;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 4f));

            var duration = Random.Range(1.2f, 3f);
            var oldOffset = _navMeshAgent.baseOffset;
            var newOffset = Random.Range(minOffset, minOffset * 5.5f);
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                _navMeshAgent.baseOffset = Mathf.Lerp(oldOffset, newOffset, t / duration);
                yield return null;
            }
        }
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            _navMeshAgent.SetDestination(transform.parent.position + Random.insideUnitSphere * 0.42f);
            yield return new WaitForSeconds(_navMeshAgent.remainingDistance / _navMeshAgent.speed + 0.1f);
        }
    }
    
    protected override IEnumerator Brain()
    {
        yield return new WaitForSeconds(0.1f);
        _navMeshAgent.enabled = true;
        StartCoroutine(Movement());
        StartCoroutine(LogicCycle());
        StartCoroutine(VerticalMovement());
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (shooting)
            {
                var obj = Instantiate(gunParticles, transform.parent);
                obj.transform.position = gunTip.position;
                obj.transform.rotation = gunTip.rotation;

                var b = Instantiate(bullet, transform.parent);
                b.speed = bulletSpeed;
                b.direction = player.position - gunTip.position;
                b.transform.position = gunTip.position;
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }
}
