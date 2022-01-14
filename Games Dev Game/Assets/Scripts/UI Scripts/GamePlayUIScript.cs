using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AI;

public class GamePlayUIScript : MonoBehaviour
{
    GameManager manager;
 
    private void Awake()
    {
        manager = GetComponent<GameManager>();
        manager.txt_LevelComplete.gameObject.SetActive(false);
        manager.txt_gameOver.gameObject.SetActive(false);
        GameManager.life = manager.playerBaseRef.GetComponent<ObjHealth>().Health();
        manager.level = 0;
        manager.levelCompleted = true;

        StartCoroutine(OnBaseDestroy());
    }
    private void Update()
    {
        UpdateHudText();
    }

    public void UpdateHudText()
    {
        manager.txt_EnemyKills.text = "EnemyLeft: " + (manager.enemyCount - manager.enemyKilled);
        manager.txt_Health.text = "Life: " + GameManager.life;
        manager.txt_Level.text = manager.level.ToString();
        UpdateGold(GameManager.playerGold);


        if(manager.levelCompleted)
        {
            manager.txt_levelStartInfo.gameObject.SetActive(true);
        }
    }



    public void UpdateGold(int goldValue)
    {
        foreach (TextMeshProUGUI t in manager.txt_Gold)
        {
            t.text = goldValue.ToString();
        }
    }
    public void EnemyLeft(int count)
    {
        manager.txt_EnemyKills.text = count.ToString();
        manager.enemyCount = count;

        if(manager.enemyCount ==0)
        {
            StartCoroutine(LevelComplete());
        }
    }
    public void Health(int health)
    {
        manager.txt_Health.text = health.ToString();
    }

    #region Level


    public IEnumerator LevelManager()
    {

        while (true)
        {
            if (manager.levelCompleted)
            {
                yield return StartCoroutine(LevelComplete());
                yield break;

            }
            if (manager.enemyKilled == manager.enemyCount && !manager.playerBaseDestroyed)
            {
                manager.levelCompleted = true;
            }

            if (GameManager.gameOver)
            {
                manager.enemyList.Clear();
                manager.restartUIPanel.SetActive(true);
            }

            yield return null;
        }
    }
    public void LevelIncrease(bool b)
    {
        if(b)
        {
            manager.level++;

        }
        else
        {
            manager.level = 1;
        }
        manager.enemyCount = manager.level;
        manager.enemyKilled = 0;
        manager.enemyList = new List<GameObject>();
        Debug.Log("Level: " + manager.level);

        for (int i = 0; i < manager.level; i++)
        {
            int randDir = Random.Range(0, 4);
            Vector2 getPos = enemyDirSpawn((Direction)randDir);
            int enemyType = Random.Range(0, manager.enemyType.Count);

            GameObject tempEnemy = Instantiate(manager.enemyType[enemyType]);

            NavMeshAgent agent = tempEnemy.GetComponent<NavMeshAgent>();
            //Vector3 spawnPos = new Vector3(getPos.x, 1, getPos.y);
            //agent.Warp(spawnPos);
            tempEnemy.transform.position = new Vector3(getPos.x, 2, getPos.y);
            tempEnemy.transform.rotation = Quaternion.identity;
            manager.enemyList.Add(tempEnemy);
        }

        StartCoroutine(OnBaseDestroy());
        StartCoroutine(LevelManager());
    }
    public IEnumerator LevelComplete()
    {
        manager.txt_LevelComplete.text = "LEVEL UP!!";
        manager.txt_LevelComplete.gameObject.SetActive(true);
        yield return StartCoroutine(WaitBeforeLevelChange(5));
        manager.txt_LevelComplete.gameObject.SetActive(false);
        Debug.Log("Level Complete");
    }
    public IEnumerator OnBaseDestroy()
    {
        while (true)
        {
            if (GameManager.life <= 0)
            {
                GameManager.gameIsPaused = true;
                GameManager.gameOver = false;   
                GameManager.life = 0;
                manager.txt_gameOver.gameObject.SetActive(true);
                yield return new WaitForSeconds(5);
                GameManager.canvasOpened = true;

                manager.txt_gameOver.gameObject.SetActive(false);
                manager.restartUIPanel.SetActive(true);
                yield break;
            }
            yield return null;
        }
    }


    #endregion


    #region RestartCanvas
    public void OnRestartButtonClick()
    {
        GameManager.canvasOpened = false;
        GameManager.gameOver = false;
        GameManager.gameIsPaused = false;
        GameManager.life = 100;
        manager.level = 0;

        manager.playerCam.transform.position = manager.playerSpawnPoint.transform.position;
        LevelIncrease(false);

        manager.restartUIPanel.SetActive(false);
    }
    public void OnQuitButtonClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    #endregion


    IEnumerator WaitBeforeLevelChange(int timer)
    {
        yield return new WaitForSeconds(timer);
    }



    #region EnemySpawn Position Methods
    public enum Direction
    {
        NORTH, EAST,WEST, SOUTH
    };

    public Direction dir;
    public Vector2 enemyNorth()
    {
        float xPos = Random.Range(873, 990);
        float zPos = Random.Range(-80, 1037);
        return new Vector2(xPos, zPos);
    }
    public Vector2 enemySouth()
    {
        float xPos = Random.Range(-105,1000);
        float zPos = Random.Range(-70, 70);
        return new Vector2(xPos, zPos);
    }
    public Vector2 enemyWest()
    {
        float xPos = Random.Range(-1010, 50);
        float zPos = Random.Range(-70, 1020);
        return new Vector2(xPos, zPos);
    }
    public Vector2 enemyEast()
    {
        float xPos = Random.Range(-118, 1000);
        float zPos = Random.Range(790, 1000);
        return new Vector2(xPos, zPos);
    }

    public Vector2 enemyDirSpawn(Direction d)
    {
        Vector2 dir = new Vector2();
        switch (d)
        {
            case Direction.NORTH:
                dir= enemyNorth();
                break;
            case Direction.EAST:
                dir = enemyEast();
                break;
            case Direction.WEST:
                dir = enemyWest();
                break;
            case Direction.SOUTH:
                dir= enemySouth();
                break;
        }
        return dir;
    }

    #endregion
}
