using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ObjHealth : Characters, IHealth, IDamage<int>
{
    public int health = 100;
    public float damage;
    private void Start()
    {
        StartCoroutine(ObjDestroyed());
    }

    IEnumerator ObjDestroyed()
    {
        while (health>=0)
        {
            if(health ==0 && !this.gameObject.CompareTag("PlayerItems") && this.gameObject.GetComponent<getObjType>().objType != ObjectType.TOWNHALL)
            {
                Destroy(this.gameObject, 4f);
            }
            else
            {
                destroyed = true;
            }
            yield return null;
        }
    }
    public int Health()
    {
        return health;
    }
    public void Damage(int damageObject)
    {
        health -= damageObject;
    }
}
