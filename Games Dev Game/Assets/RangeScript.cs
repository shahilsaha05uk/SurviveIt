using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeScript : MonoBehaviour
{
    Quaternion rotation;
    public GameObject parent;


    void Start()
    {
        rotation = Quaternion.Euler(90, 0, 0);

    }

    void LateUpdate()
    {
        transform.rotation = rotation;
    }

}
