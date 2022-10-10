using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHearthUI : MonoBehaviour
{
    public Slider healthSlider;
    public GameObject enemyObj;

    private Enemy _enemy;
    // player -> player gameObj // _player = playerScript

    private void Awake()
    {
        // player -> player gameObj // _player = playerScript
        _enemy = enemyObj.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = (_enemy.curHealth / _enemy.maxHealth) * 100;
    }
}