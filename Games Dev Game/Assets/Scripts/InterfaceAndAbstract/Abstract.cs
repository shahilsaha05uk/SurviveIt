using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using MyUtility;
using cakeslice;


[System.Serializable]
public abstract class EnemyMethods : MonoBehaviour
{
#region AI

    public List<GameObject> obstacleList = new List<GameObject>();
    public GameObject playerBase;
    public Ray ray;
    public RaycastHit[] hitObjs;
    public List<TargetDetails> targetDetails = new List<TargetDetails>();
    public bool alive = true;

    public TestState currentState;
    ObjHealth targetHealth;


    public NavMeshAgent agent;
    public static EnemyState enemyState;
    public Animator anim;
    public GameObject setTarget;

    public float remainingDistance;
    public bool destinationReached;

    public int health;
    public GameManager manager;
    public int goldReward;
    public int attackDamage;
    public float enemyYPos;

    #endregion
    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        playerBase = manager.playerBaseRef;
        if(GameManager.gameOver)
        {
            Destroy(this.gameObject,2f);
        }
    }
    public virtual void EnemySetup()
    {
        targetDetails.Clear();
        obstacleList.Clear();

        playerBase = GameObject.Find("GameManager").GetComponent<GameManager>().playerBaseRef;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        Vector3 directionToPlayerBase = (playerBase.transform.position - transform.position);

        ray = new Ray(transform.position, directionToPlayerBase);
        hitObjs = Physics.RaycastAll(ray, directionToPlayerBase.magnitude);

        for (int i = 0; i < hitObjs.Length; i++)
        {
            if (hitObjs[i].collider != null)
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

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerItems"))
        {
            // add the target to the list if its not already there and rearrange them according to the distance
            Transform tempTargetTransform = other.transform;
            bool targetAlreadyInTheList = false;
            foreach (var item in targetDetails)
            {
                if(item.objTransform == tempTargetTransform)
                {
                    targetAlreadyInTheList = true;
                    break;
                }
                else
                {
                    targetAlreadyInTheList = false;
                }
            }
            if(!targetAlreadyInTheList)
            {
                Debug.Log("Target is not in the list: " + tempTargetTransform.name);
                TargetDetails newTarget = new TargetDetails();
                newTarget.distance = Vector3.Distance(transform.position, tempTargetTransform.position);
                newTarget.objTransform = tempTargetTransform;
                newTarget.obj = tempTargetTransform.gameObject;
                targetDetails.Add(newTarget);

            }
            else
            {
                Debug.Log("Target is already in the list: " + tempTargetTransform.name);
            }

            NextTarget();

        }
    }

    public virtual void RemoveTargetFromTheList(GameObject destroyedTarget)
    {
        for (int i = 0; i < targetDetails.Count; i++)
        {
            if (targetDetails[i].obj == destroyedTarget)
            {
                manager.Destroyer(targetDetails[i].obj);
                targetDetails.Remove(targetDetails[i]);
            }
        }

        Debug.Log("After removing the count:" + targetDetails.Count);
    }
    public virtual Transform NextTarget()
    {
        Transform nextDestination;

        nextDestination = targetDetails[0].objTransform;
        float minDistance = targetDetails[0].distance;

        foreach (TargetDetails t in targetDetails)
        {
            if (t.distance < minDistance)
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
    public virtual void EnemyAnimState()
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

    public virtual IEnumerator EnemyAction()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Enemy waiting finished!! start walking towards the direction");

        enemyState = EnemyState.IDLE;
        EnemyAnimState();
    }
    public virtual IEnumerator EnemyIdle()
    {
        Vector3 nextPos = NextTarget().position;

        yield return new WaitForSeconds(4);

        agent.SetDestination(nextPos);

        enemyState = EnemyState.WALK;
        EnemyAnimState();

    }
    public virtual IEnumerator EnemyAttack()
    {
        agent.isStopped = true;
        targetHealth = setTarget.GetComponent<ObjHealth>();
        Quaternion lookRot = Quaternion.LookRotation(setTarget.transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 0.5f);

        while (targetHealth.Health() > 0)
        {
            targetHealth.Damage(attackDamage);
            Debug.Log("Health: " + targetHealth);

            if(targetHealth.gameObject.CompareTag("PlayerBase"))
            {
                GameManager.life = targetHealth.Health();
            }

            yield return new WaitForSeconds(2f);
        }

        if (targetHealth.Health() <= 0 && setTarget.gameObject.GetComponent<getObjType>().objType == ObjectType.TOWNHALL)
        {

            Debug.Log("Player Base Destroyed");
            GameManager.gameOver = true;
        }
        else if(targetHealth.Health() <= 0 && setTarget.gameObject.GetComponent<getObjType>().objType != ObjectType.TOWNHALL)
        {
            RemoveTargetFromTheList(setTarget);
            DestroyScript.UpdateNavmeshSurface();
            enemyState = EnemyState.IDLE;
            EnemyAnimState();

            yield break;
        }

    }
    public virtual IEnumerator EnemyWalk()
    {
        agent.isStopped = false;

        while (true)
        {
            if (setTarget != null && setTarget.CompareTag("PlayerItems"))
            {
                float remainingDistance = Vector3.Distance(setTarget.transform.position, agent.transform.position);
                if (remainingDistance <= 20)
                {
                    destinationReached = true;
                    enemyState = EnemyState.ATTACK;
                    EnemyAnimState();

                    yield break;
                }
                this.remainingDistance = remainingDistance;

            }
            else if (setTarget != null && setTarget.CompareTag("PlayerBase"))
            {
                Debug.Log("Target: " + setTarget.gameObject.name);
                float remainingDistance = Vector3.Distance(setTarget.transform.position, agent.transform.position);
                if (remainingDistance <= 100)
                {
                    destinationReached = true;
                    enemyState = EnemyState.ATTACK;
                    EnemyAnimState();

                    yield break;
                }
                this.remainingDistance = remainingDistance;

            }
            else
            {
                Debug.Log("Waiting for target set");
            }
            yield return null;
        }


    }
    public virtual IEnumerator EnemyChangePath()
    {
        while (health>0)
        {
            Vector3 trackCurrentPosition = transform.position;
            yield return new WaitForSeconds(5);
        }


        yield break;
    }
    public virtual IEnumerator UpdateTargetList()
    {
        while (true)
        {
            
            Debug.Log("Target update list coroutine started");
            foreach (TargetDetails t in targetDetails.ToList())
            {
                if (t.obj == null || t.objTransform==null)
                {
                    targetDetails.Remove(t);
                }
                yield return null;
            }
        }
    }
}

public abstract class Characters : MonoBehaviour
{
    public enum States
    {
        WAIT,
        PREPARE,
        SHOOT
    }
#region Variables
    public GameObject shotPoint;
    public GameObject particle;
    public ParticleSystem.ShapeModule particlePos;
    public GameObject target;
    public States states;
    public bool destroyed;
    public Ray ray;
    public bool lookAtTarget;
    public bool soundAvailable;
    public List<GameObject> enemyObjects = new List<GameObject>();
    public GameObject prefab;
    public GameManager manager;
    public Outline outliner;
    public bool gotOutlinerComp;

    public int dealDamage;
    #endregion
    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        destroyed = false;

        //Add the OutLinerScript
        Transform[] childObjs = GetComponentsInChildren<Transform>();
        foreach (var item in childObjs)
        {
            item.gameObject.AddComponent<Outline>();

            gotOutlinerComp = item.gameObject.TryGetComponent<Outline>(out outliner);
            if (gotOutlinerComp)
            {
                item.gameObject.GetComponent<Outline>().enabled = false;
            }

        }



        states = States.WAIT;
        Behave(); 

        StartCoroutine(FindClosestTarget());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AddEnemyToTheList(other.gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            transform.rotation = LookTowardsTarget(lookAtTarget);
        }
    }

    public virtual void AddEnemyToTheList(GameObject triggeredObject)
    {
        if (!enemyObjects.Contains(triggeredObject))
        {
            enemyObjects.Add(triggeredObject);
        }
    }
    public virtual IEnumerator FindClosestTarget()
    {
        while (!destroyed)
        {
            UpdateTargetList();

            if (enemyObjects.Count > 0)
            {
                float distance = Vector3.Distance(enemyObjects[0].transform.position, transform.position);
                GameObject tempTarget = enemyObjects[0].gameObject;
                for (int i = 0; i < enemyObjects.Count; i++)
                {
                    if (Vector3.Distance(enemyObjects[i].transform.position, transform.position) < distance)
                    {
                        distance = Vector3.Distance(enemyObjects[i].transform.position, transform.position);
                        tempTarget = enemyObjects[i].gameObject;
                    }
                }
                target = tempTarget;
            }
            else
            {
                Debug.Log("Character: Waiting for the targets to arrive");
            }
            yield return new WaitForSeconds(2);
        }
    }
    public virtual Quaternion LookTowardsTarget(bool b)
    {
        Quaternion slerp = Quaternion.identity;
        if (lookAtTarget)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            slerp = Quaternion.Slerp(transform.rotation, RotationClass.LookAtTarget(direction), Time.deltaTime * 2f);

        }
        return slerp;
    }
    public virtual void LookTowardsTarget()
    {

    }
    public virtual void Behave()
    {
        switch (states)
        {
            case States.WAIT:
                Debug.Log("Character Wait");
                StartCoroutine(Wait());
                break;
            case States.PREPARE:
                Debug.Log("Character idle");
                enemyObjects.TrimExcess();
                StartCoroutine(Prepare());
                break;
            case States.SHOOT:
                Debug.Log("Character shoot");
                StartCoroutine(Shoot());
                break;
        }
    }
    public virtual void LaunchParticle(bool b)
    {
        ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
        if (b)
        {
            prefab = Instantiate(this.particle, shotPoint.transform.position, shotPoint.transform.rotation);
            prefab.transform.SetParent(shotPoint.transform);
            SoundEffect(soundAvailable);
            prefab.GetComponent<ParticleSystem>().Play();

        }
        Destroy(prefab, main.duration);
    }
    public virtual void SoundEffect(bool b)
    {
        if (b)
        {
            GetComponent<AudioSource>().Play();
        }
    }
    public virtual void RemoveTargetFromTheList()
    {
        for (int i = 0; i < enemyObjects.Count; i++)
        {
            if (enemyObjects[i] == target)
            {
                manager.Destroyer(enemyObjects[i]);
                //enemyObjects.Remove(enemyObjects[i]);
                enemyObjects.TrimExcess();
            }
        }
        Debug.Log("After removing the count:" + enemyObjects.Count);
    }
    public virtual void UpdateTargetList()
    {

        foreach (GameObject t in enemyObjects.ToList())
        {
            if (t == null)
            {
                enemyObjects.Remove(t);
            }
        }

    }

    public virtual IEnumerator Shoot()
    {
        HealthScript health;
        bool foundHealthComp = target.TryGetComponent<HealthScript>(out health);
        Debug.Log("Character: inside shoot");
        if (foundHealthComp)
        {
            while (health.Health() > 0)
            {
                LaunchParticle(true);
                health.Damage(-dealDamage);
                Debug.Log("Character: Health is: " + health.Health());

                yield return new WaitForSeconds(2);
            }
        }
        else
        {
            Debug.Log("Character: Couldnt find the script");
            yield break;
        }

        if (health.Health() <= 0)
        {
            RemoveTargetFromTheList();
            states = States.WAIT;
            Behave();
        }
    }
    public virtual IEnumerator Wait()
    {
        Debug.Log("Character: Waiting for targets");
        target = null;
        while (!destroyed)
        {
            if (target != null)
            {
                states = States.PREPARE;
                Behave();
                yield break;
            }
            else
            {
                Debug.Log("Target not found yet");
            }
            yield return null;
        }
    }
    public virtual IEnumerator Prepare()
    {
        states = States.SHOOT;
        Behave();
        yield return null;
    }


}

public abstract class Objects : MonoBehaviour
{
    public virtual void Start()
    {
        Transform[] childObjs = GetComponentsInChildren<Transform>();
        foreach (var item in childObjs)
        {
            item.gameObject.AddComponent<Outline>();

            item.GetComponent<Outline>().enabled = false;
        }

    }
}
