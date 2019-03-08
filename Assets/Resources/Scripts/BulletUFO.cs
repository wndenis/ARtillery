using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class BulletUFO : Bullet
{
    public Transform target;
    public float delay = 1.1f;

    // Update is called once per frame
    protected override void Init()
    {
        StartCoroutine(Align());
    }

    private IEnumerator Align()
    {
        direction = Random.insideUnitSphere;
        direction.y = Mathf.Abs(direction.y) + 0.01f;
        yield return new WaitForSeconds(delay + Random.Range(0, delay) * 0.25f);
        while (true)
        {
            direction = target.position + target.forward * 0.04f - transform.position;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
