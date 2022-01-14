using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShopScriptController : MonoBehaviour
{
    private GameManager manager;
    public Camera cam;
    static public bool itemBought;
    public static bool relocate;
    public int itemCost;
    public LayerMask mask;
    private RaycastHit hitinfo;

    //private SelectObjectsScript select;
    private SelectObjectsScript select;
    public GameObject buyPanel;
    public GameObject instantiatedObject = null;
    public GameObject territoryRef;
    private Ray playerRay;

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        playerRay = manager.playerRay;
        if (!relocate)
        {
            select.Selection(playerRay, hitinfo);
        }
        Relocation();
    }

    void Init()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        select = manager.GetComponent<SelectObjectsScript>();
    }
    public void SpawnBuildings(GameObject obj)
    {
        GameObject tempObj = Instantiate(obj);
        instantiatedObject = tempObj;

        relocate = true;
    }
     public GameObject objToMove;
    void Relocation()
    {
        if(relocate && instantiatedObject != null)
        {
            objToMove = instantiatedObject;
            GameManager.canvasOpened = false;
        }
        else if(relocate && select.selectObject != null)
        {
            objToMove = select.selectObject;

            GameManager.canvasOpened = false;
            manager.upgradeUIPanel.SetActive(false);
        }

        if (objToMove != null)
        {
            objToMove.GetComponent<Rigidbody>().isKinematic = true;
            if (Physics.Raycast(playerRay, out hitinfo, Mathf.Infinity, mask))
            {

                Vector3 pos = new Vector3(hitinfo.point.x, hitinfo.point.y + objToMove.transform.position.y, hitinfo.point.z);
                objToMove.transform.position = pos;

                // Rotating the object on instantiate
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    objToMove.transform.Rotate(Vector3.up * 2f, Space.World);
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    objToMove.transform.Rotate(Vector3.down * 2f, Space.World);
                }
            }
        }
    }


}
