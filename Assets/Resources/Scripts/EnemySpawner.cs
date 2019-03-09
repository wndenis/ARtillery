using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemy;
    public EnemyUFO enemyUFO;
    public Enemy striker;
    public Transform spawnParticles;
    public float interval = 6f;
    public int ufoCount = 15;
    public int strikerCount = 5;
    private int ufoCounter;
    private int strikerCounter;
    public Transform player;
    public Transform spawnLine;
    public Transform pointA;
    public Transform pointB;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawner());
    }

    private void Spawn(Enemy origin)
    {
        var offset = Random.insideUnitCircle * 0.1125f;
        var pos = transform.position + new Vector3(offset.x, 0, offset.y);
        Spawn(origin, pos);
    }

    private void Spawn(Enemy origin, Vector3 pos)
    {
        var obj = Instantiate(spawnParticles, transform.parent);
        obj.position = pos;
        
        var enemyInstance = Instantiate(origin, transform.parent);
        enemyInstance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        enemyInstance.player = player;
        enemyInstance.transform.position = pos;
    }

    private IEnumerator Spawner()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (ufoCounter++ >= ufoCount)
            {
                ufoCounter = 0;
                Spawn(enemyUFO);
            }
            else if (strikerCounter++ >= strikerCount)
            {
                strikerCounter = 0;
                Spawn(striker);
            } 
            else
            {
                if (Random.Range(0f, 1f) < 0.085f)
                {
                    const int count = 6;
                    spawnLine.rotation = Quaternion.Euler(0, Random.Range(0f, 180f), 0);
                    for (var i = 0f; i < count; i++)
                    {
                        var pos = Vector3.Lerp(pointA.position, pointB.position, i / count);
                        Spawn(enemy, pos);
                        yield return new WaitForSeconds(0.125f);
                    }
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    Spawn(enemy);
                }
            }
            yield return new WaitForSeconds(interval);
            
            if (interval < 1.5f) continue;
            if (Random.Range(0f, 1f) < 0.15f)
                interval -= 0.01f;
            interval -= 0.025f;
        }
    }
}
