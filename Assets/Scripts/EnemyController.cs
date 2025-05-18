using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;
    bool finishedSpawning;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    void Update()
    {
        if (finishedSpawning && player!= null)
        {
            agent.SetDestination(player.position);
        }
    }
    public void FinishedSpawning()
    {
        finishedSpawning = true;
        agent.enabled = true;
    }
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
