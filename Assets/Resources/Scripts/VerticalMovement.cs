using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VerticalMovement : MonoBehaviour
{
    public float maxOffset = 0.35f;
    private NavMeshAgent _navMeshAgent;
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        yield return new WaitForSeconds(0.2f);
        var minOffset = _navMeshAgent.baseOffset;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 4f));

            var duration = Random.Range(1.2f, 3f);
            var oldOffset = _navMeshAgent.baseOffset;
            var newOffset = Random.Range(minOffset, minOffset + maxOffset);
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                _navMeshAgent.baseOffset = Mathf.Lerp(oldOffset, newOffset, t / duration);
                yield return null;
            }
        }
    }
}
