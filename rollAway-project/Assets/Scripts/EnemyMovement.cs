using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
          if (player != null)
       {    
           navMeshAgent.SetDestination(player.position);
       }
    }
}
