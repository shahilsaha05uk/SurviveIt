using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPanelScript : MonoBehaviour
{
    [SerializeField] private Transform panelTransform;
    [SerializeField] private ScriptableObjectButtonScript buttonPrefab;

    [SerializeField] private ScriptableObjects[] scriptableObjects;
    private ScriptableObjectButtonScript[] buttonList;

    static public bool itemBought;
    public ShopScriptController shopScriptController;
    public GamePlayUIScript gamePlayUI;
    public GameManager manager;

    public ScriptableObjects sc;

    private void OnEnable()
    {
        buttonList = new ScriptableObjectButtonScript[scriptableObjects.Length];
        shopScriptController = GameObject.Find("GameManager").GetComponent<ShopScriptController>();
        manager = GetComponent<GameManager>();
        gamePlayUI = GameObject.Find("GameManager").GetComponent<GamePlayUIScript>();

        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i] = Instantiate(buttonPrefab);
            buttonList[i].transform.SetParent(panelTransform);

            buttonList[i].Init(scriptableObjects[i]);
            buttonList[i].scriptableObjEvent += OnButtonPressed;
        }
    }
    private void OnDisable()
    {
        for (int i = buttonList.Length - 1; i >= 0; i--)
        {
            buttonList[i].scriptableObjEvent -= OnButtonPressed;
            Destroy(buttonList[i].gameObject);
            GameManager.canvasOpened = false;
        }
    }

    private void OnButtonPressed(ScriptableObjects sc)
    {
        if (GameManager.playerGold < System.Int32.Parse(sc.txt_cost))
        {
            Debug.Log("You cannot buy this item!! Earn More and get back later");
            this.gameObject.SetActive(false);

        }
        else
        {
            Debug.Log("Button Clicked");
            shopScriptController.SpawnBuildings(sc.item);
            ShopScriptController.relocate = true;
            shopScriptController.itemCost = System.Int32.Parse(sc.txt_cost);

            this.gameObject.SetActive(false);
        }


    }


}
