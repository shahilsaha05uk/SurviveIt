using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ScriptableObjectButtonScript : MonoBehaviour
{
    public event Action<ScriptableObjects> scriptableObjEvent;

    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_NameTxt;
    [SerializeField] private TextMeshProUGUI m_Cost;

    [HideInInspector] public GameObject item;
    [HideInInspector] public GameObject upgradedItem;

    private ScriptableObjects scriptableObjects;


    public void Init(ScriptableObjects scObjects)
    {
        scriptableObjects = scObjects;

        m_NameTxt.text = scriptableObjects.txt_name;
        m_Cost.text = "£"+ scriptableObjects.txt_cost;
        item = scriptableObjects.item;
        upgradedItem = scriptableObjects.itemUpgrade;

    }

    public void BuyItem()
    {
        scriptableObjEvent?.Invoke(scriptableObjects);
    }

    public void OnCursorEnter()
    {
        Debug.Log("Cost of the Item: " + scriptableObjects.txt_cost);
    }

}
