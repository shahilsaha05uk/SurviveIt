using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
[SelectionBase]
public class CharacterScript : Characters
{
    public override void AddEnemyToTheList(GameObject triggeredObject)
    {
        base.AddEnemyToTheList(triggeredObject);
    }
    public override void Behave()
    {
        base.Behave();
    }
    public override IEnumerator Prepare()
    {
        return base.Prepare();
    }
    public override IEnumerator Shoot()
    {
        return base.Shoot();
    }
    public override IEnumerator Wait()
    {
        return base.Wait();
    }
    public override Quaternion LookTowardsTarget(bool b)
    {
        return base.LookTowardsTarget(b);
    }
    public override void LaunchParticle(bool b)
    {
        base.LaunchParticle(b);
    }


}
