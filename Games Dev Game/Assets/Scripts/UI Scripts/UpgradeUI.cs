using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpgradeUI : MonoBehaviour
{
    [SerializeField]private GameManager manager;
    public static SelectObjectsScript select;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject upgradeUIPanel;
    RaycastHit hit;
    Ray ray;
    Ray objectRay;
    public LayerMask mask;
    GameObject upgradedObj;
    public int upgradeCost;
    public GameObject oldBuilding;
    public bool objectUpgraded;





    private void Start()
    {
        objectUpgraded = false;
        select = this.gameObject.GetComponent<SelectObjectsScript>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        upgradeUIPanel.SetActive(false);
    }

    void Update()
    {
        ray = manager.playerRay;

        if(manager.upgradeUIPanel.activeInHierarchy)
        {
            manager.txt_buyMenuText.text = select.selectObject.GetComponent<getObjType>().objName;

            foreach (var item in manager.upgradableObjects)
            {
                if(item.txt_name == select.selectObject.GetComponent<getObjType>().objName)
                {
                    manager.txt_upgradeCost.text = $"£{item.txt_upgradeCost}";
                    break;
                }
            }
        
        }

    }
    public void Move()
    {
        ShopScriptController.relocate = true;
    }    
    public void Upgrade()
    {
        GameObject oldBuilding = select.selectObject;
        GameObject newBuilding = SearchForUpgradeBuilding(oldBuilding);




        //Upgrade the object
        if (newBuilding != null)
        {
            if (newBuilding.GetComponent<getObjType>().objType == ObjectType.TOWNHALL)
            {
                manager.playerBaseRef = newBuilding;
            }

            upgradedObj = Instantiate(newBuilding);
            upgradedObj.transform.rotation = oldBuilding.transform.localRotation;

            objectRay = new Ray(upgradedObj.transform.position, -upgradedObj.transform.up * 100);

            if (Physics.Raycast(upgradedObj.transform.position, -upgradedObj.transform.up, out hit, 500f, mask))
            {
                Vector3 pos = new Vector3(oldBuilding.transform.position.x, hit.point.y + upgradedObj.transform.position.y, oldBuilding.transform.position.z);
                upgradedObj.transform.position = pos;
            }

            objectUpgraded = true;
            GameManager.playerGold -= upgradeCost;


            Destroy(oldBuilding.gameObject);
            GameManager.canvasOpened = false;
            SelectObjectsScript.objectSelected = false;
        }
        else
        {
            Debug.Log("You Reached the Max Upgrade");
            GameManager.canvasOpened = false;
            SelectObjectsScript.objectSelected = false;

        }
        manager.upgradeUIPanel.SetActive(false);
    }

    public Vector3 ObjectScaling(GameObject newBuilding)
    {

        Bounds rangeBounds = oldBuilding.GetComponent<Renderer>().bounds;
        Bounds sphereColliderBounds = newBuilding.GetComponent<SphereCollider>().bounds;


        Vector3 scaleDifference = new Vector3(rangeBounds.size.x / sphereColliderBounds.size.x,
                                               rangeBounds.size.y / sphereColliderBounds.size.y,
                                               rangeBounds.size.z / sphereColliderBounds.size.z);

        float resizeX = scaleDifference.x * newBuilding.transform.localScale.x;
        float resizeY = scaleDifference.y * newBuilding.transform.localScale.y;
        float resizeZ = scaleDifference.z * newBuilding.transform.localScale.z;


        sphereColliderBounds.center = rangeBounds.center;
        Vector3 newScale = new Vector3(resizeX, resizeY, resizeZ);

        return newScale;
    }

    public string oldBuildingName;
    public string oldBuildingType;
    public string scBuildingName;
    public string matchFound;
    public List<GameObject> sameTypeList;

    private GameObject SearchForUpgradeBuilding(GameObject oldBuilding)
    {
        bool b = false;
        string name = null;
        GameObject upgradedObject = null;

        oldBuildingName = oldBuilding.GetComponent<getObjType>().objName;
        oldBuildingType = oldBuilding.GetComponent<getObjType>().objType.ToString();

        //find all the buildings of this type in the sc list
        sameTypeList = new List<GameObject>();
        foreach (var item in manager.upgradableObjects)
        {
            if(item.GetObjType().ToString() == oldBuildingType)
            {
                sameTypeList.Add(item.item);
            }
        }

        //Check if the object name matches with the old building name
        foreach (var item in manager.upgradableObjects)
        {
            if (oldBuildingName == item.txt_name)
            {
                if (item.itemUpgrade != null)
                {
                    b = true;
                    matchFound = item.item.name;

                    scBuildingName = item.itemUpgrade.name;
                    upgradedObject = item.itemUpgrade;
                    upgradeCost = System.Int32.Parse(item.txt_upgradeCost);
                    break;
                }
                else
                {
                    StartCoroutine(manager.InfoDisplay("Building Upgrade limit reached", 4f));
                    return null;
                }
            }
            else
            {
                b = false;
            }
        }

            if (b)
            {
                Debug.Log("Found the old building in the list!! the upgradable item name is: " + name);
            }
            else
            {
                Debug.Log("Could not find the upgradable item ");
            }
        return upgradedObject;
    }



}
