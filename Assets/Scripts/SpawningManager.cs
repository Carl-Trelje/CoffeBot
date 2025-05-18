using System.Collections.Generic;
using UnityEngine;

public class SpawningManager : MonoBehaviour
{
    [SerializeField] private ObjectPool enemyPool;

    private Dictionary<GameObject, GameObject> tileToEnemy = new Dictionary<GameObject, GameObject>();

    public GameObject SpawnEnemyAtTile(GameObject tile)
    {
        Vector3 spawnPos = tile.transform.position + Vector3.up * 3f;
        GameObject enemy = enemyPool.GetFromPool(spawnPos, Quaternion.identity);
        enemy.transform.SetParent(transform);

        tileToEnemy[tile] = enemy;
        return enemy;
    }

    public void NotifyTileFinished(GameObject tile)
    {
        if (tileToEnemy.TryGetValue(tile, out GameObject enemy))
        {
            var controller = enemy.GetComponent<EnemyController>();
            controller?.FinishedSpawning();
        }
    }

    public void DespawnEnemy(GameObject enemy)
    {
        enemyPool.ReturnToPool(enemy);
    }

    public bool Spawning()
    {
        return true;
    }
}
