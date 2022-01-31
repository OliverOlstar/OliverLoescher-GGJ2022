using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private LightAndDark lightAndDark = null;

    [SerializeField] private GameObject prefab = null;

    [SerializeField] private AnimationCurve oddsGraph = new AnimationCurve();
    [SerializeField] private float maxTime = 10.0f;

    [Header("Location")]
    [SerializeField] private Transform target = null;
    [SerializeField] private float disFromTarget = 8.0f;

    private int enemyCount = 0;
    private float spawnTime = 0.0f;

    private void Update() 
    {
        if (Time.time < spawnTime)
            return;
        float minValue = Mathf.Clamp01((enemyCount - 2) * 0.1f);
        float delay = oddsGraph.Evaluate(Random.Range(minValue, 1.0f)) * maxTime;
        spawnTime = Time.time + delay;

        if (lightAndDark.mode == LightAndDark.Mode.Light && Random.value < 0.95f)
            return;

        if (!TryGetSpawnLocation(out Vector3 pos))
            return;

        GameObject enemy = Instantiate(prefab);
        enemy.transform.position = pos;
        enemy.SetActive(true);
        enemyCount++;
        Enemy e = enemy.GetComponent<Enemy>();
        e.health.onValueOut.AddListener(EnemyDied);     
    }

    private void EnemyDied()
    {
        enemyCount--;
    }

    private bool TryGetSpawnLocation(out Vector3 pPosition)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = target.position + (new Vector3(Random.value - 0.5f, 0.0f, Random.value - 0.5f).normalized * disFromTarget);
            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 6.0f, NavMesh.AllAreas))
            {
                pPosition = hit.position;
                return true;
            }
        }
        pPosition = Vector3.zero;
        return false;
    }
}
