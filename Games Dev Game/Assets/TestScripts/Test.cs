using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { WALK, ATTACK, DEAD, IDLE, BEGIN };

[SelectionBase]
public class Test : MonoBehaviour
{
    public bool alive;
    public static EnemyState state;
    public NavMeshAgent agent;
    public Animator anim;
    public Transform playerBase;
    public string targetName;
    public string hitColliderName;
    public Transform target;
    Ray ray;
    RaycastHit hit;
    public NavMeshPathStatus status;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        alive = true;
        target = playerBase.transform;
        
        StartCoroutine(PathCheck());

    }

/*    private IEnumerator PathCheck()
    {
        while (alive)
        {
            ray = new Ray(transform.position, transform.forward);
            Debug.DrawRay(ray.origin, ray.direction*100, Color.red);
            bool rayCast = Physics.Raycast(ray, out hit, 10f);

            if (rayCast)
            {
                if (hit.collider.CompareTag("PlayerItems"))
                {
                    Debug.Log("Agent destination is set to player item");

                    agent.ResetPath();
                    agent.SetDestination(hit.collider.transform.position);
                }
                if(!hit.collider.CompareTag("PlayerItems"))
                {
                    Debug.Log("Agent destination is set to player base");
                    agent.ResetPath();
                    agent.SetDestination(playerBase.position);
                }
            }

            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Path Complete");
                state = EnemyState.ATTACK;
                StartCoroutine(DestroyTarget(hit.collider.transform));
            }
            else
            {
                Debug.Log("Path Incomplete");
                state = EnemyState.WALK;
            }
            targetName = agent.transform.name;

            yield return null;
        }
    }
*/    
    
    private IEnumerator PathCheck()
    {
        agent.SetDestination(target.position);
        while (alive)
        {
            Vector3 direction = (playerBase.position - transform.position);
            ray = new Ray(transform.position, direction);

            Debug.DrawRay(ray.origin, ray.direction * 500, Color.red);

            if (Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, target.position)))
            {
                Debug.Log("Hits the collider: " + hit.collider.name);

                if(hit.collider.transform != target.transform)
                {
                    agent.ResetPath();
                    target = hit.transform;
                }
                else
                {
                    agent.ResetPath();
                    target = playerBase;
                }
            }
            else
            {
                Debug.Log("No collider in the way");
                agent.ResetPath();
                target = playerBase;
            }
            StartCoroutine(ReachDestination(hit.collider.transform));

            yield return null;

        }
    }

    IEnumerator ReachDestination(Transform destination)
    {
        agent.SetDestination(destination.position);

        while (agent.hasPath)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                state = EnemyState.IDLE;
            }
            else
            {
                state = EnemyState.WALK;
            }
            yield return null;
        }
    }
    IEnumerator DestroyTarget(Transform target)
    {
        ObjHealth health;
        target.TryGetComponent<ObjHealth>(out health);
        
        if(health!= null)
        {
            while (health.Health()!= 0)
            {
                health.Damage(-5);
                yield return new WaitForSeconds(3f);
            }
        }

    }

    private void Update()
    {

        status = agent.pathStatus;
        if (hit.collider == null)
        {
            hitColliderName = "null";
        }
        else
        {
            hitColliderName = hit.collider.name;
        }

        if (alive)
        {
            switch (state)
            {
                case EnemyState.IDLE:
                    anim.SetBool("Walk", false);
                    anim.SetBool("Attack", false);
                    anim.SetBool("Idle", true);

                    break;
                case EnemyState.WALK:
                    anim.SetBool("Walk", true);
                    anim.SetBool("Attack", false);
                    anim.SetBool("Idle", false);

                    break;
                case EnemyState.ATTACK:
                    anim.SetBool("Walk", false);
                    anim.SetBool("Attack", true);
                    break;
                case EnemyState.DEAD:
                    anim.SetBool("Walk", false);
                    anim.SetBool("Attack", false);
                    anim.SetBool("isDead", true);
                    break;
            }
        }
    }



}
