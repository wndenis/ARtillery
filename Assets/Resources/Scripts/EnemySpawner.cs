using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemy;
    public EnemyUFO enemyUFO;
    public Transform spawnParticles;
    public float interval = 7f;
    public int ufoCount = 15;
    private int ufoCounter;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            var offset = Random.insideUnitCircle * 0.15f;
            var pos = transform.position + new Vector3(offset.x, 0, offset.y);
            var obj = Instantiate(spawnParticles, transform.parent);
            obj.position = pos;
            
            yield return new WaitForSeconds(0.05f);
            Enemy enemyInstance;
            if (ufoCounter++ >= ufoCount)
            {
                ufoCounter = 0;
                enemyInstance = Instantiate(enemyUFO, transform.parent);
            }
            else
            {
                enemyInstance = Instantiate(enemy, transform.parent);
            }
            
            enemyInstance.player = player;
            enemyInstance.transform.position = pos;
            
            
            yield return new WaitForSeconds(interval);
            
            if (interval < 0.65f) continue;
            if (Random.Range(0, 1) < 0.15f)
                interval -= 0.1f;
            interval -= 0.05f;
        }
    }
}
