using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage<T>
{
    void Damage(T damageObject);
}
public interface IHealth
{
    int Health();
}