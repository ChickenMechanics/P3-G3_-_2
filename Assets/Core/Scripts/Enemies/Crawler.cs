using UnityEngine;
using UnityEngine.AI;

public class Crawler : MonoBehaviour
{
    public NavMeshAgent agent;

    private DefaultGroundEnemyBehaviour m_DefaultGroundEnemyBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        m_DefaultGroundEnemyBehaviour = gameObject.AddComponent<DefaultGroundEnemyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        m_DefaultGroundEnemyBehaviour.MoveTowardsPlayer(transform,agent);
    }
}
