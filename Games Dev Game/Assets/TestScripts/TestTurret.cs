using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTurret : MonoBehaviour
{
    private Vector3 shootPosition;
    public GameObject arrowPrefab;
    private void Awake()
    {
        shootPosition = transform.Find("shootPoint").position;        
    }



    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(arrowPrefab, Input.mousePosition, Quaternion.identity);
        }
    }
}
