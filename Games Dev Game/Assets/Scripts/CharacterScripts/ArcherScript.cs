using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

[SelectionBase]
public class ArcherScript : Characters
{
    public GameObject arrow; 
    GameObject tempArrow;
    private void Update()
    {
        if(tempArrow!= null)
        {
            tempArrow.transform.Translate(Vector3.forward * 20 * Time.deltaTime);
        }


    }

    public override void AddEnemyToTheList(GameObject triggeredObject)
    {
        base.AddEnemyToTheList(triggeredObject);
    }
    public override void Behave()
    {
        base.Behave();
    }
    public override void LaunchParticle(bool b)
    {
        base.LaunchParticle(b);
    }
    public override IEnumerator Prepare()
    {
        return base.Prepare();
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
                tempArrow = Instantiate(arrow);
                tempArrow.GetComponent<Rigidbody>().AddForce(shotPoint.transform.forward * 5000f, ForceMode.Force);



                health.Damage(-10);
                Debug.Log("Character: Health is: " + health.Health());

                yield return new WaitForSeconds(2);
                Destroy(tempArrow, 4f);
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
    public override IEnumerator Wait()
    {
        return base.Wait();
    }
    public override Quaternion LookTowardsTarget(bool b)
    {
        Quaternion lerpRot = Quaternion.Lerp(transform.localRotation, RotationClass.LookAtTarget(transform, target.transform, 'y'),1f);
        return lerpRot;
    }

}
