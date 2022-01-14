using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
public class SelectObjectsScript : MonoBehaviour
{

    Renderer hitRenderer;
    private GameManager manager;

    public Material highlightMat;
    public Material[] defaultMat;

    public GameObject selectObject;
    public GameObject focusedObject;

    public static bool objectSelected;
    public static bool baseSelected;
    
    private string itemTag;
    private Outline outline;
    void Start()
    {
        itemTag = "PlayerItems";
        objectSelected = false;
        selectObject = null;
        focusedObject = null;
        manager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (objectSelected)
        {
            this.gameObject.GetComponent<UpgradeUI>().oldBuilding = selectObject;
        }
    }
    public void Selection(Ray ray, RaycastHit hitinfo)
    {
        if (Physics.Raycast(ray, out hitinfo) && !ShopScriptController.relocate)
        {
            focusedObject = hitinfo.transform.gameObject;

            if (Input.GetMouseButtonDown(0))
            {
                if(focusedObject.gameObject.CompareTag("PlayerBase"))
                {
                    selectObject = focusedObject;
                }

                if (!objectSelected && focusedObject.CompareTag(itemTag))
                {
                    selectObject = focusedObject;

                    bool b = selectObject.TryGetComponent<Outline>(out outline);
                    if(b)
                    {
                        outline.enabled = true;
                    }
                    objectSelected = true;
                }
                else if (objectSelected && focusedObject.CompareTag(itemTag))
                {
                    //Disable outline on the selected object
                    bool b = selectObject.TryGetComponent<Outline>(out outline);
                    if (b)
                    {
                        outline.enabled = false;
                    }

                    selectObject = focusedObject;
                    b = selectObject.TryGetComponent<Outline>(out outline);
                    if (b)
                    {
                        outline.enabled = true;
                    }

                }
                else if (objectSelected && !focusedObject.CompareTag(itemTag))
                {
                    objectSelected = false;
                    bool b = selectObject.TryGetComponent<Outline>(out outline);
                    if (b)
                    {
                        outline.enabled = false;
                    }
                }

            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Object dematerialise");
                    bool b = selectObject.TryGetComponent<Outline>(out outline);
                    if (b)
                    {
                        objectSelected = false;
                        outline.enabled = false;
                    }
                }
            }
        }
        else
        {
            focusedObject = null;

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Object dematerialise");

                if(selectObject!= null)
                {
                    bool b = selectObject.TryGetComponent<Outline>(out outline);
                    if (b)
                    {
                        outline.enabled = false;
                    }
                }
                objectSelected = false;
            }

        }
    }
    
}