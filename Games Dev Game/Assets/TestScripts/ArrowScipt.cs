using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScipt : MonoBehaviour
{
    void Update()
    {
        transform.Translate(transform.forward * 20 * Time.deltaTime);
    }

}
