using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Objects", menuName = "Items", order = 1)]
public class ScriptableObjects : ScriptableObject
{
    public ObjectType type;
    public string txt_name;
    public string txt_cost;
    public string txt_upgradeCost;
    public GameObject item;
    public GameObject itemUpgrade;
    public Vector3 range;
    public int health;
    public ObjectType GetObjType() { return type; }
}
public enum ObjectType
{
    NONE,
    BARRIGADE,
    TURRET,
    WALLS,
    TOWNHALL,
    GUNNER,
    CANNON,
    WATCHTOWER
}
