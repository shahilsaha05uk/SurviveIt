using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

[Serializable]
public struct TargetDetails
{
    public GameObject obj;
    public Transform objTransform;
    public float distance;
}

//[ExecuteInEditMode]
public class testCube2 : MonoBehaviour
{
    List<GameObject> obstacleList = new List<GameObject>();
    GameObject playerBase;
    Ray ray;
    RaycastHit[] hitObjs;
    public List<TargetDetails> targetDetails = new List<TargetDetails>();
    private bool alive = true;

    private TestState currentState;


    #region AI
    private NavMeshAgent agent;
    public static EnemyState enemyState;
    private Animator anim;
    public GameObject setTarget;

    public float remainingDistance;
    public bool destinationReached;

    private int health;

    #endregion

    void EnemySetup()
    {
        targetDetails.Clear();
        obstacleList.Clear();

        playerBase = GameObject.Find("GameManager").GetComponent<GameManager>().playerBaseRef;
        agent = GetComponent<NavMeshAgent>();

        Vector3 directionToPlayerBase = (playerBase.transform.position - transform.position);

        ray = new Ray(transform.position, directionToPlayerBase);
        hitObjs = Physics.RaycastAll(ray, directionToPlayerBase.magnitude);

        for (int i = 0; i < hitObjs.Length; i++)
        {
            if (hitObjs[i].collider.CompareTag("PlayerItems") || hitObjs[i].collider.CompareTag("PlayerBase"))
            {
                TargetDetails details = new TargetDetails();
                details.obj = hitObjs[i].collider.gameObject;
                details.objTransform = hitObjs[i].collider.transform;
                details.distance = Vector3.Distance(hitObjs[i].collider.transform.position, transform.position);

                targetDetails.Add(details);
                Debug.Log("Hit obj names: " + hitObjs[i].collider.name);
            }
        }

    }

    private void Awake()
    {
        EnemySetup();
        anim = GetComponent<Animator>();
        destinationReached = false;
        enemyState = EnemyState.IDLE;
        StartCoroutine(EnemyMove());

    }

    private void RemoveTargetFromTheList(GameObject destroyedTarget)
    {
        for (int i = 0; i < targetDetails.Count; i++)
        {
            if (targetDetails[i].obj != destroyedTarget)
            {
                Debug.Log("Object cannot be found");
            }
            else
            {
                targetDetails.RemoveAt(i);
            }
        }
    }
    private Transform NextTarget()
    {
        Transform nextDestination = targetDetails[0].objTransform;
        float minDistance = targetDetails[0].distance;

        foreach (TargetDetails t in targetDetails)
        {
            if(t.distance < minDistance)
            {
                minDistance = t.distance;
                nextDestination = t.objTransform;
            }
        }

        setTarget = nextDestination.gameObject;

        Debug.Log("Obj target: " + nextDestination.gameObject.name);
        Debug.Log("Obj position: " + nextDestination.position);
        return nextDestination;
    }

    private IEnumerator EnemyMove()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Enemy waiting finished!! start walking towards the direction");

        enemyState = EnemyState.IDLE;
        EnemyAnimState();
    }

    void EnemyAnimState()
    {
        switch (enemyState)
        {
            case EnemyState.WALK:
                anim.SetBool("Walk", true);
                anim.SetBool("Idle", false);
                anim.SetBool("Attack", false);

                StartCoroutine(EnemyWalk());

                break;
            case EnemyState.ATTACK:
                anim.SetBool("Walk", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Attack", true);

                StartCoroutine(EnemyAttack());
                break;
            case EnemyState.DEAD:
                anim.SetBool("Walk", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Attack", false);

                anim.SetBool("isDead", true);


                break;
            case EnemyState.IDLE:

                anim.SetBool("Walk", false);
                anim.SetBool("Attack", false);
                anim.SetBool("Idle", true);

                StartCoroutine(EnemyIdle());


                break;
        }
    }
    IEnumerator EnemyIdle()
    {
        Vector3 nextPos = NextTarget().position;

        yield return new WaitForSeconds(4);

        agent.SetDestination(nextPos);
        enemyState = EnemyState.WALK;
        EnemyAnimState();

    }
    IEnumerator EnemyAttack()
    {
        ObjHealth health = setTarget.GetComponent<ObjHealth>();
        Quaternion lookRot = Quaternion.LookRotation(setTarget.transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 0.5f);

        while (health.Health()>0)
        {
            health.Damage(-10);
            Debug.Log("Health: " + health);
            yield return new WaitForSeconds(2f);
        }


        if (health.Health() <= 0)
        {
            RemoveTargetFromTheList(setTarget);
            DestroyScript.UpdateNavmeshSurface();
            enemyState = EnemyState.IDLE;
            EnemyAnimState();
        }
    }
    IEnumerator EnemyWalk()
    {
        while (true)
        {
            float remainingDistance = Vector3.Distance(setTarget.transform.position, agent.transform.position);
            if(remainingDistance <=20f)
            {

                destinationReached = true;
                enemyState = EnemyState.ATTACK;
                EnemyAnimState();

                yield break;
            }
            this.remainingDistance = remainingDistance;

            yield return null;
        }


    }

    private void FixedUpdate()
    {
        if(GetComponent<HealthScript>().Health() <=0)
        {
            enemyState = EnemyState.DEAD;
            EnemyAnimState();

            DestroyScript.DestroyElement(this.gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction*1000, Color.red);
    }
}
