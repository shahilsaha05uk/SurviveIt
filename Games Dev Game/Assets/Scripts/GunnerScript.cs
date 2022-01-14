using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using MyUtility;
using cakeslice;
[SelectionBase]
public class GunnerScript : Characters
{
    public Transform go_baseRotation;
    public Transform go_GunBody;
    public Transform go_barrel;

    public MeshRenderer[] testMat;

    public float barrelRotationSpeed;
    float currentRotationSpeed;

    public float firingRange;

    public ParticleSystem muzzelFlash;

    bool canFire = false;

    void Start()
    {
        this.GetComponent<SphereCollider>().radius = firingRange;
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        destroyed = false;

        states = States.WAIT;
        Behave();

        StartCoroutine(FindClosestTarget());

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


    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
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
            LookTowardsTarget();
        }
    }
    private void Update()
    {
        AimAndFire();

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            GetMaterials();
        }
    }
    public override void AddEnemyToTheList(GameObject triggeredObject)
    {
        if (!enemyObjects.Contains(triggeredObject))
        {
            enemyObjects.Add(triggeredObject);
        }
    }
    public override IEnumerator FindClosestTarget()
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
    public override void LookTowardsTarget()
    {
        // aim at enemy
        Vector3 baseTargetPostition = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
        Vector3 gunBodyTargetPostition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);

        go_baseRotation.transform.LookAt(baseTargetPostition);
        go_GunBody.transform.LookAt(gunBodyTargetPostition);


    }
    public override void Behave()
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
    public override void LaunchParticle(bool b)
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
    public override void SoundEffect(bool b)
    {
        if (b)
        {
            GetComponent<AudioSource>().Play();
        }
    }
    public override void RemoveTargetFromTheList()
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
    public override void UpdateTargetList()
    {

        foreach (GameObject t in enemyObjects.ToList())
        {
            if (t == null)
            {
                enemyObjects.Remove(t);
            }
        }

    }

    public override IEnumerator Shoot()
    {
        HealthScript health;
        bool foundHealthComp = target.TryGetComponent<HealthScript>(out health);
        Debug.Log("Character: inside shoot");
        if (foundHealthComp)
        {
            while (health.Health() > 0)
            {
                Debug.Log("Machine gun shoot Character: inside shoot");
                canFire = true;

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
            canFire = false;
            RemoveTargetFromTheList();
            states = States.WAIT;
            Behave();
        }
    }
    public override IEnumerator Wait()
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
    public override IEnumerator Prepare()
    {
        states = States.SHOOT;
        Behave();
        yield return null;
    }

    public void GetMaterials()
    {
        testMat = GetComponentsInChildren<MeshRenderer>();
    }

    void AimAndFire()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (canFire)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
            }

        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);

            // stop the particle system
            if (muzzelFlash.isPlaying)
            {
                muzzelFlash.Stop();
            }
        }
    }
}
