using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using cakeslice;
public class GameManager : MonoBehaviour
{

    public bool isEnabled;

    public GameObject crosshair;

    public static int playerGold;
    public int countdownTimer;
    public int enemyDefeatCount;

    [Space(10)][Header("References")]
    public GameObject playerBaseRef;
    public GameObject healthbarRef;
    public GameObject territoryRef;

    [Space(10)][Header("Panel References")]
    public GameObject pauseUIPanel;
    public GameObject optionUIPanel;
    public GameObject upgradeUIPanel;
    public GameObject buyUIPanel;
    public GameObject hudUIPanel;
    public GameObject restartUIPanel;

    [Space(10)][Header("Text References")]
    public TextMeshProUGUI[] txt_Gold;
    public TextMeshProUGUI txt_Health;
    public TextMeshProUGUI txt_Level;
    public TextMeshProUGUI txt_LevelComplete;
    public TextMeshProUGUI txt_EnemyKills;
    public TextMeshProUGUI txt_levelStartInfo;
    public TextMeshProUGUI txt_gameOver;
    public TextMeshProUGUI txt_InfoText;
    public TextMeshProUGUI txt_buyMenuText;
    public TextMeshProUGUI txt_upgradeCost;

    public int levelCount;

    [Space(10)][Header("Script References")]
    public GamePlayUIScript gamePlayUI;
    private UpgradeUI upgradeUI;
    private ShopScriptController shop;

    [Space(10)][Header("Levels")]
    public List<GameObject> enemyType;
    

    public int level;
    public bool levelCompleted = true;

    public GameObject tempPlayerBase;
    public GameObject tempEnemy;

    [HideInInspector] public int enemyCount;
    [HideInInspector] public int enemyKilled;
    [HideInInspector] public static int life;
    //[HideInInspector] public static int money;
    public Transform playerBaseSpawnPoint;

    public List<GameObject> enemyList;
    public bool playerBaseDestroyed;
    public static bool gameOver;
    public static bool canvasOpened;

    public static bool gameIsPaused;
    public List<ScriptableObjects> upgradableObjects;

    [Space(10)][Header("Player Refs")]
    public GameObject playerPrefab;
    public Camera playerCam;
    public Transform playerSpawnPoint;
    public Ray playerRay;
    void Init()
    {

        gameOver = false;
        gamePlayUI = GetComponent<GamePlayUIScript>();
        upgradeUI = GetComponent<UpgradeUI>();
        shop = GetComponent<ShopScriptController>();


        playerGold = 2000;
        enemyDefeatCount = 0;
        countdownTimer = 0;
        pauseUIPanel.SetActive(false);
        optionUIPanel.SetActive(false);
        buyUIPanel.SetActive(false);
        upgradeUIPanel.SetActive(false);
        hudUIPanel.SetActive(true);
        restartUIPanel.SetActive(false);
        txt_InfoText.gameObject.SetActive(false);

        gameIsPaused = false;

    }

    private void Awake()
    {
        Init();
        //CursorMode(CursorLockMode.Locked, false);
    }


    public IEnumerator InfoDisplay(string text, float timer)
    {
        txt_InfoText.gameObject.SetActive(true);
        txt_InfoText.text = text;
        yield return new WaitForSeconds(timer);
        txt_InfoText.text = "";
        txt_InfoText.gameObject.SetActive(false);
    }
    private void Update()
    {
        playerRay = playerPrefab.GetComponent<Movements>().ray;
        playerRay = playerCam.ScreenPointToRay(crosshair.transform.position);
        Debug.DrawRay(playerRay.origin, playerRay.direction * 1000, Color.red);

        UIControls();
        PauseGame();

        if(GameManager.gameOver)
        {
            playerBaseDestroyed = true;
            levelCompleted = false;
            GameManager.life = 0;
        }

        if (canvasOpened)
        {
            gameIsPaused = true;
            CursorMode(CursorLockMode.Confined, true);
        }
        else
        {
            gameIsPaused = false;
            CursorMode(CursorLockMode.Locked, true);
        }
    }
    public void Destroyer(GameObject obj)
    {
        if (obj.CompareTag("Enemy") && obj!= null)
        {
            enemyCount--;
            Debug.Log("Enemy count: " + enemyCount);
            playerGold += obj.GetComponent<EnemyScript>().goldReward;
        }
        Destroy(obj);
    }
    public static void CursorMode(CursorLockMode mode, bool visible)
    {
        Cursor.lockState = mode;
        Cursor.visible = visible;
    }
    public void UIControls()
    {
        if (ShopScriptController.relocate && Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(shop.instantiatedObject);
            canvasOpened = false;
        }
        //Upgrading
        if (Input.GetMouseButtonDown(0) && ShopScriptController.relocate)
        {
            Debug.Log("Coming to reloacte");
            ShopScriptController.relocate = false;
            shop.objToMove.transform.SetParent(territoryRef.transform);


            playerGold -= shop.itemCost;
            shop.objToMove = null;

            upgradeUIPanel.SetActive(false);
            canvasOpened = false;
        }
        //Buy Panel
        if (Input.GetKeyDown(KeyCode.B) && !buyUIPanel.activeInHierarchy && !canvasOpened)
        {
            buyUIPanel.SetActive(true);
            canvasOpened = true;
        }
        else if (Input.GetKeyDown(KeyCode.B) && buyUIPanel.activeInHierarchy)
        {
            buyUIPanel.SetActive(false);
            canvasOpened = false;
        }
        //Pause Panel
        if (Input.GetKeyDown(KeyCode.P) && !pauseUIPanel.activeInHierarchy && !canvasOpened)
        {
            pauseUIPanel.SetActive(true);
            canvasOpened = true;
        }
        else if (Input.GetKeyDown(KeyCode.P) && pauseUIPanel.activeInHierarchy)
        {
            pauseUIPanel.SetActive(false);
            canvasOpened = false;
        }

        //Option Panel
        if (Input.GetKeyDown(KeyCode.Escape) && !optionUIPanel.activeInHierarchy && !canvasOpened && !ShopScriptController.relocate)
        {
            optionUIPanel.SetActive(true);
            canvasOpened = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && optionUIPanel.activeInHierarchy)
        {
            optionUIPanel.SetActive(false);
            canvasOpened = false;
        }


        if (SelectObjectsScript.objectSelected == true && !upgradeUIPanel.activeInHierarchy && !canvasOpened && !ShopScriptController.relocate)
        {
            upgradeUIPanel.SetActive(true);
            canvasOpened = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && SelectObjectsScript.objectSelected == true && upgradeUIPanel.activeInHierarchy)
        {
            cakeslice.Outline outline;
            bool b = GetComponent<SelectObjectsScript>().selectObject.TryGetComponent<cakeslice.Outline>(out outline);
            if (b)
            {
                outline.enabled = false;
            }

            SelectObjectsScript.objectSelected = false;
            upgradeUIPanel.SetActive(false);
            canvasOpened = false;
        }


        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            life = 0;
        }

        if (Input.GetKeyDown(KeyCode.Return) && levelCompleted)
        {
            levelCompleted = false;
            txt_levelStartInfo.gameObject.SetActive(false);

            gamePlayUI.LevelIncrease(true);
        }
    }
    public void PauseGame()
    {
        if (canvasOpened)
        {
            GameManager.gameIsPaused = true;
            Debug.Log("Game Paused");
            Time.timeScale = 0;
        }
        else
        {
            GameManager.gameIsPaused = false;
            Debug.Log("Game Resumed");
            Time.timeScale = 1;
        }
    }

    #region Panel

    public void OnResumeBtnClick()
    {
        optionUIPanel.SetActive(false);
        GameManager.canvasOpened = false;
    }
    public void OnExitBtnClick()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    #endregion


}