using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSelectionSctipt : MonoBehaviour
{
    GameManager manager;

    public ShopScriptController shopRef;

    public GameObject hitObject;
    public GameObject lastHitObject;

    [Space(10)][Header("Materialise")]

    public Material highlightMaterial;

    public Material[] highlightMaterialList;
    public Material[] hitObjectMaterial;
    public Material[] defaultMaterial;

    [Space(10)][Header("De-materialise")]
    public Material[] de_defaultMaterial;


    private void Start()
    {
        manager = GetComponent<GameManager>();
        shopRef = GetComponent<ShopScriptController>();
    }




    public void Selection(Ray ray, RaycastHit hit)
    {
        if(Physics.Raycast(ray, out hit, Mathf.Infinity) && !ShopScriptController.relocate)
        {
            hitObject = hit.collider.gameObject;
            if(hitObject.GetComponent<MeshRenderer>()!= null && hit.collider.CompareTag("PlayerItems"))
            {
                hitObjectMaterial = hitObject.GetComponent<MeshRenderer>().materials;
                HighLight(hitObject.GetComponent<MeshRenderer>());


            }

            else if ((hitObject.GetComponent<MeshRenderer>() == null) ||
               (hitObject!= lastHitObject) ||
               (hitObject.GetComponent<MeshRenderer>() != null) && !hitObject.CompareTag("PlayerItems") ||
               (hit.collider == null))
            {
                Debug.Log("Deselect the object");
                GetTheDefaultMaterial();
                SetTheDefaultMaterial();
            }
        }

        lastHitObject = hitObject;
    }


    //DemMaterialise
    public void GetTheDefaultMaterial()
    {
        de_defaultMaterial = defaultMaterial;
    }
    public void SetTheDefaultMaterial()
    {
        lastHitObject.GetComponent<MeshRenderer>().materials = de_defaultMaterial;
    }



    //Materialise
    public void HighLight(MeshRenderer hitObjMatRenderer)
    {
        StoreTheDefaultMaterial(hitObjMatRenderer);
        CreateHighlightMat(hitObjMatRenderer);
        hitObjMatRenderer.materials = highlightMaterialList;
    }
    public void StoreTheDefaultMaterial(MeshRenderer hitObjMatRenderer)
    {
        defaultMaterial = new Material[hitObjMatRenderer.materials.Length];

        //Store the defaultMaterial
        for (int i = 0; i < hitObjMatRenderer.materials.Length; i++)
        {
            defaultMaterial[i] = hitObjMatRenderer.materials[i];
        }
    }
    public void CreateHighlightMat(MeshRenderer hitObjMatRenderer)
    {
        Material[] highlightMaterialList = new Material[hitObjMatRenderer.materials.Length];

        for (int i = 0; i < highlightMaterialList.Length; i++)
        {
            highlightMaterialList[i] = highlightMaterial;
        }
        this.highlightMaterialList = highlightMaterialList;
    }

}