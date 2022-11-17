using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InforPanel : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] Text[] _enemyCount;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        _enemyCount[0].text = "X " + _gameManager.enemyCntA.ToString();
        _enemyCount[1].text = "X " + _gameManager.enemyCntB.ToString();
        _enemyCount[2].text = "X " + _gameManager.enemyCntC.ToString();
    }
}
