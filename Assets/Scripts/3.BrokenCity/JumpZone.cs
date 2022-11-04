using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    public GameManager manager;
    private Player player;
    public int jumpValue;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            player = FindObjectOfType<Player>();
            player.jumpPower = jumpValue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.jumpPower = 10;
        }
    }
}
