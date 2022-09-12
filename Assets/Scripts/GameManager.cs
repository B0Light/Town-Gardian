using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
//Cam    
    public GameObject menuCam;
    public GameObject gameCam;
//Obj    
    public Player player;
    public Boss boss;

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    public Transform[] enemySpawn;

    public GameObject[] enemies;

    public List<int> enemyList;
//InGame    
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
//UI
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoText;
    public Text scoText;
    public Text stageText;
    public Text playTimeText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
    public Image Weapon1Img;
    public Image Weapon2Img;
    public Image Weapon3Img;
    public Image WeaponRImg;
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text curScoreText;
    public Text bestText;
    private void Awake()
    {
        enemyList = new List<int>();
        maxScoText.text = string.Format("{0:n0}",PlayerPrefs.GetInt("MaxScore")); 
    }
    
    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);
        
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        
        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoText.text;

        int maxSco = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxSco)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore",player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemySpawn)
            zone.gameObject.SetActive(true);
        
        isBattle = true;
        StartCoroutine(InBattle());
    }
    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        
        foreach (Transform zone in enemySpawn)
            zone.gameObject.SetActive(false);
        
        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3],
                enemySpawn[0].position, enemySpawn[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            boss = instantEnemy.GetComponent<Boss>();
            enemy.manager = this;
        }
        else
        {
            for (int i = 0; i < stage; i++)
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
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]],
                    enemySpawn[ranZone].position, enemySpawn[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }
    private void Update()
    {
        if (isBattle) playTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        // UI 
        scoText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeText.text =  string.Format("{0:00}", hour) + ":" 
                          + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);
        
        // Player UI
        playerHealthText.text = player.health + "/" + player.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", player.ammo);
        if (player.equipWeapon == null)
            playerAmmoText.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + player.ammo;
        else
            playerAmmoText.text = player.equipWeapon.curAmmo + "/" + player.ammo;

        //Weapon UI
        Weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        Weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        Weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        WeaponRImg.color = new Color(1, 1, 1, player.hasGrendes > 0 ? 1 : 0);

        //Cnt Enemy UI
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //Boss Health UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
            
    }
}
