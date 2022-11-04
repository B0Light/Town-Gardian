using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public enum SceneType
    {
        Login, 
        Lobby, 
        Camp,
        Game,
        Test,
        Shop
    }

    public SceneType SType;
//Cam    
    public GameObject menuCam;
    public GameObject gameCam;

//Pos
    public Transform startPos;
    public Transform[] enemySpawn;
    public Transform chestPos;

    //Obj    
    protected Player player;
    protected UiManager _uiManager;
    public Boss boss;
    
    public GameObject clearShip;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public GameObject[] enemies;
    public GameObject[] rewardchests;
    public GameObject[] stageWall;

    public List<int> enemyList;

//InGame    
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

//GameData    
    protected GameDataStore m_DataStore;
    public static GameManager instance;

    protected int curScore;
    protected int rewardValue;
    protected void Awake()
    {
        instance = this;
        if (SceneManager.GetActiveScene().name == "MainCamp") SType = SceneType.Camp;
        if (SceneManager.GetActiveScene().name == "GameScene") SType = SceneType.Game;
        if (SceneManager.GetActiveScene().name == "TestScene") SType = SceneType.Test;
        /*
        switch (SType)
        {
            case SceneType.Game:
                enemyList = new List<int>(); 
                maxScoText.text = string.Format("{0:n0}",PlayerPrefs.GetInt("MaxScore"));
                break;
        }
        */
    }

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>();
        player.transform.position = startPos.position;
        _uiManager = FindObjectOfType<UiManager>();
        
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);
        _uiManager.menuPanel.SetActive(false);
        _uiManager.gamePanel.SetActive(true);
        
        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        StopAllCoroutines();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _uiManager.gamePanel.SetActive(false);
        _uiManager.overPanel.SetActive(true);
        _uiManager.curScoreText.text = _uiManager.scoText.text;

        int maxSco = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxSco)
        {
            _uiManager.bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore",player.score);
        }
    }

    public void Restart()
    {
        Destroy(player.gameObject);
        SceneManager.LoadScene(0);
    }
    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemySpawn)
            zone.gameObject.SetActive(true);

        curScore = player.score;
        isBattle = true;
        StartCoroutine(InBattle());
    }
    public virtual void StageEnd()
    {
        _uiManager.enemyGroup.SetActive(false);
        _uiManager.BossGroup.SetActive(false);
        
        player.transform.position = startPos.position;
        player.transform.LookAt(Vector3.zero);
        player.transform.rotation = startPos.rotation;
        if (stage < 10)
        {
            itemShop.SetActive(true);
            weaponShop.SetActive(true);
            startZone.SetActive(true);
            if(stage <= stageWall.Length)
            {
                stageWall[stage-1].SetActive(false);
            }
        }
        else if(stage == 10)
        {
            _uiManager.stageText.text = "CLEAR";
            ClearStage();
        }

        rewardValue = (player.score - curScore) / 1500;
        rewardValue = rewardValue >= 5 ? 4 : rewardValue;
         
        GameObject instantChest = Instantiate(rewardchests[rewardValue],
                            chestPos.position, chestPos.rotation);
        
   
        
        foreach (Transform zone in enemySpawn)
            zone.gameObject.SetActive(false);
        
        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        _uiManager.enemyGroup.SetActive(true);
        _uiManager.BossGroup.SetActive(true);
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3],
                enemySpawn[0].position, enemySpawn[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.Upgrade(stage);
            enemy.target = player.transform;
            boss = instantEnemy.GetComponent<Boss>();
            enemy.manager = this;
        }

        if (stage != 5)
            for (int i = 0; i < 2+stage; i++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);
                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
        
        while (enemyList.Count > 0){
            int ranZone = Random.Range(0, enemySpawn.Length) % stage;
            GameObject instantEnemy = Instantiate(enemies[enemyList[0]],
                enemySpawn[ranZone].position, enemySpawn[ranZone].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.Upgrade(stage);
            enemy.target = player.transform;
            enemy.manager = this;
            enemyList.RemoveAt(0);
            yield return new WaitForSeconds(1f);
        }
        

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        if (enemyCntA + enemyCntB + enemyCntC + enemyCntD == 0)
        {
            yield return new WaitForSeconds(2f);
            boss = null;
            StageEnd();
        }
    }

    protected void Update()
    {
        if (isBattle) playTime += Time.deltaTime;
    }

    void ClearStage()
    {
        clearShip.SetActive(true);
    }
    /*
    public int GetStarsForLevel(string levelId)
    {
        if (!levelList.ContainsKey(levelId))
        {
            Debug.LogWarningFormat("[GAME] Cannot check if level with id = {0} is completed. Not in level list", levelId);
            return 0;
        }

        return m_DataStore.GetNumberOfStarForLevel(levelId);
    }
    */
}
