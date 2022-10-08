using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatisUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider StaminaSlider;

    public GameObject player;

    private Player _player;
    // player -> player gameObj // _player = playerScript

    private void Awake()
    {
        // player -> player gameObj // _player = playerScript
        _player = player.GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = (_player.health / _player.maxHealth) * 100;
        StaminaSlider.value = (_player.stamina / _player.maxStamina) * 100;
    }
}
