using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] FloorManager floorManagerPrefab;
    [SerializeField] SpawningManager spawningManagerPrefab;
    [SerializeField] NavMeshSurface navMeshSurfacePrefab;
    [SerializeField] GameObject playerPrefab;
    GameObject player;
    SpawningManager spawningManager;
    NavMeshSurface navMeshSurface;
    FloorManager floorManager;
    bool needsRebuild = false;
    float rebuildCooldown = 1f;
    float lastRebuildTime = -5f;
     Queue<GameObject> tilesToNotify = new Queue<GameObject>();
    void Awake()
    {
        floorManager = Instantiate(floorManagerPrefab);
        spawningManager = Instantiate(spawningManagerPrefab);
        navMeshSurface = Instantiate(navMeshSurfacePrefab);
        player = Instantiate(playerPrefab);
        navMeshSurface.BuildNavMesh();
        // SceneManager.LoadScene("Background"); SceneManager.LoadScene("Level1");
        // Scene backgroundScene = SceneManager.GetSceneByName("Background"); Scene levelScene = SceneManager.GetSceneByName("Level1");
        // SceneManager.MergeScenes(backgroundScene, levelScene);
    }
    void Update()
    {
        if (needsRebuild && Time.time - lastRebuildTime > rebuildCooldown)
        {
            StartCoroutine(RebuildNavMeshAndEnableAgents());
            needsRebuild = false;
        }
    }

    void Start()
    {
        StartCoroutine(Spawning(floorManager));
        floorManager.OnTileReachedBottom += HandleTileReachedBottom;
        floorManager.OnTileFinishedMovingUp += HandleTileFinishedMovingUp;
    }
     void HandleTileReachedBottom(Vector3 position)
    {
        GameObject tile = floorManager.GetTileAtPosition(position);
        if (tile != null)
        {
            GameObject enemy = spawningManager.SpawnEnemyAtTile(tile);
            if (enemy != null)
            {
                EnemyController ec = enemy.GetComponent<EnemyController>();
                if (ec != null)
                {
                    ec.SetPlayer(player.transform);
                }
            }
        }
    }

    void HandleTileFinishedMovingUp(GameObject tile)
    {
        tilesToNotify.Enqueue(tile);
        needsRebuild = true;
    }

    IEnumerator RebuildNavMeshAndEnableAgents()
    {
        var buildOperation = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

        while (!buildOperation.isDone)
            yield return null;

        while (tilesToNotify.Count > 0)
        {
            GameObject tile = tilesToNotify.Dequeue();
            spawningManager.NotifyTileFinished(tile);
        }
    }


    IEnumerator Spawning(FloorManager floorManager)
    {
        yield return null;
        while (spawningManager.Spawning())
        {
            floorManager.SelectRandomFloor();
            needsRebuild = true;
            yield return new WaitForSeconds(2f);
        }
    }
}
