using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth, IDamage<int>
{
    public int health = 100;

    public void Damage(int damageObject)
    {
        health += damageObject;
    }

    public int Health()
    {
        return health;
    }



}
