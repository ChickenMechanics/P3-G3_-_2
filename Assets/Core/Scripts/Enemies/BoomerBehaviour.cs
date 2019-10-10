using UnityEngine;
using UnityEngine.AI;

public class BoomerBehaviour : DefaultGroundEnemyBehaviour
{
    private NavMeshAgent agent;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        MoveTowardsPlayer(transform, agent);

        if (distanceToPlayer < 4)
        {
            GetComponent<Renderer>().material.color = Color.red;
            //Explode
        }
    }
}
