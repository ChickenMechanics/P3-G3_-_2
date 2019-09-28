using UnityEngine;
using UnityEngine.AI;

public class BoomerBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;

    private DefaultGroundEnemyBehaviour m_DefaultGroundEnemyBehaviour;

    // Start is called before the first frame update
    private void Start()
    {
        m_DefaultGroundEnemyBehaviour = gameObject.AddComponent<DefaultGroundEnemyBehaviour>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_DefaultGroundEnemyBehaviour.MoveTowardsPlayer(transform, agent);

        if (m_DefaultGroundEnemyBehaviour.GetDistanceToPlayer() < 4)
        {
            GetComponent<Renderer>().material.color = Color.red;
            //Explode
        }
    }
}
