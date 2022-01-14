using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshUpdater : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(UpdateSurface());
    }

    IEnumerator UpdateSurface()
    {
        while (true)
        {


            NavMeshData data = GetComponent<NavMeshSurface>().navMeshData;
            Debug.Log("Nav mesh data name: " + GetComponent<NavMeshSurface>().navMeshData.name);

            GetComponent<NavMeshSurface>().UpdateNavMesh(data);

            yield return new WaitForSeconds(1);
        }
    }
}
