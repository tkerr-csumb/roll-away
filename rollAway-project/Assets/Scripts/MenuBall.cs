using UnityEngine;
using UnityEngine.AI;

public class MenuBall : MonoBehaviour
{
    [SerializeField] float wanderRadius = 20f;
    [SerializeField] float ballRadius = 0.5f;
    [SerializeField] Transform ballVisual;

    NavMeshAgent agent;
    bool isPickingDestination = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 1f;
        SetNewDestination();
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 velocity = agent.velocity;
        if (velocity.magnitude > 0.1f)
        {
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, velocity.normalized);
            float rotationAmount = (velocity.magnitude * Time.deltaTime * Mathf.Rad2Deg * 1f) / ballRadius;
            ballVisual.Rotate(rotationAxis, rotationAmount, Space.World);
        }

        if (!isPickingDestination && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isPickingDestination = true;
            Invoke(nameof(SetNewDestination), 0.5f);
        }
    }

    void SetNewDestination()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        isPickingDestination = false;
    }
}