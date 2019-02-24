using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class BulletUFO : Bullet
{
    public Transform target;

    // Update is called once per frame
    protected override void Init()
    {
        StartCoroutine(Align());
    }

    private IEnumerator Align()
    {
        while (true)
        {
            direction = target.position + target.forward * 0.04f - transform.position;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
