using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour, IHealth, IDamage<int>
{
    public int health;
    public int damage;

    public void Damage(int damageObject)
    {
        health += damageObject;
    }

    public int Health()
    {
        return health;
    }
}
