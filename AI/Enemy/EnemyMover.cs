using System;
using System.Collections;
using System.Collections.Generic;
using Manicomio.AI;
using Manicomio.Animation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour
{
    [Tooltip("Array of areas that this agent has to patrol")]
    [SerializeField] GameObject[] areasToExplore;
    Vector3[] m_patrolPath; 
    List<Vector3> m_auxPatrolPath;

    NavMeshAgent m_agent;

    Animator m_animator;

    private Vector3 m_destination;

    private bool m_shouldMove = true;

    System.Random m_rnd;

    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_patrolPath = MapAnalyzer.GetPath(areasToExplore);
        m_auxPatrolPath = new List<Vector3>(m_patrolPath);
        m_animator = GetComponent<Animator>();
        m_rnd = new System.Random();
        PickNextDestination(true);
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if(!m_shouldMove) return;

        Vector3 currPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 dstToCompare = new Vector3(m_destination.x, 0f, m_destination.z);

        if (Vector3.Distance(currPos, dstToCompare) <= Mathf.Epsilon)
        {
            PickNextDestination();
        }
    }

    private void PickNextDestination(bool avoidExploring = false)
    {
        if(!avoidExploring) DoExploring();

        if(m_auxPatrolPath.Count == 0) m_auxPatrolPath.InsertRange(0, m_patrolPath);

        m_destination = m_auxPatrolPath[m_rnd.Next(m_auxPatrolPath.Count)];
        m_auxPatrolPath.Remove(m_destination);

        m_agent.SetDestination(m_destination);
    }

    private void DoExploring()
    {
        m_shouldMove = false;
        m_agent.isStopped = true;
        if (!MapAnalyzer.CheckArea(m_destination, (int)NavMeshArea.CORRIDOR)) {
            m_animator.Play(AnimationConstants.ENEMY_EXPLORE);
        }
    }

    // Called by an animation event in Animacion/Enemy/Exploration.anim. Watch out if renaming
    private void OnFinishExploring() {
        m_shouldMove = true;
        m_agent.isStopped = false;
        m_animator.Play(AnimationConstants.ANIMATION_GENERIC_IDLE);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_destination, 0.5f);
    }

    // private void OnTriggerEnter(Collider other) {
    //     IActionableObject collidedActionableObject;
    //     AgentDetector collidedAgentDetector;

    //     Debug.Log("Collider hit");

    //     if(other.TryGetComponent<AgentDetector>(out collidedAgentDetector)) {
    //         m_agent.isStopped = true;
    //         m_shouldMove = false;

    //         collidedActionableObject = collidedAgentDetector.GetActionableObject();
    //         if(collidedActionableObject.IsInteractiveByEnemies()) {
    //             collidedActionableObject.Interact();
    //             Debug.Log("Starting Coroutine");
    //             StartCoroutine(WaitAnimationFinish(collidedActionableObject));
    //         }
    //     }
    // }

    // private IEnumerator WaitAnimationFinish(IActionableObject animated) {
    //     Debug.Log("Waiting for animation to finish");
    //     // yield return new WaitUntil(() => !animated.IsAnimationPlaying());
    //     yield return new WaitForSeconds(2f);
    //     Debug.Log("Animation Finished");

    //     m_agent.isStopped = false;
    //     m_shouldMove = true;
    // }
}
