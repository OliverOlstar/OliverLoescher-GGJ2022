using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementQueue : MonoBehaviour
{
    public static EnemyMovementQueue Instance;
    public List<EnemyMovement> enemyMovements = new List<EnemyMovement>();

    [SerializeField, Range(1, 60)] private float updatesPerSecond = 60;
    private float inverseUpdatesPerSecond;
    private float updateTime = 0.0f;
    private int queueIndex = 0;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("[EnemyMovementQueue.cs] Multiple Instances, destroying self", gameObject);
            Destroy(this);
        }

        inverseUpdatesPerSecond = 1 / updatesPerSecond;
    }

    public void AddToQueue(EnemyMovement pMovement)
    {
        enemyMovements.Add(pMovement);

        // Update timer if queue stopped being empty
        if (enemyMovements.Count == 1)
            updateTime = Time.time + inverseUpdatesPerSecond;
    }

    public void RemoveFromQueue(EnemyMovement pMovement)
    {
        enemyMovements.Remove(pMovement);
    }

    private void Update() 
    {
        if (enemyMovements.Count > 0)
        {
            // Limit framerate of update
            if (Time.time >= updateTime)
            {
                updateTime += inverseUpdatesPerSecond;

                queueIndex++;
                if (queueIndex >= enemyMovements.Count)
                    queueIndex = 0;

                enemyMovements[queueIndex].CalculatePath();
            }
        }
    }
}
