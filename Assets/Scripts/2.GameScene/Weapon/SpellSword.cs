using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpellSword : Weapon
{
    private Player player;
    [SerializeField] GameObject spell;
    [SerializeField] Transform[] spellPos;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public override void Use() {
        if (player.stamina >= 10)
        {
            player.stamina -= 10;
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    public override void UpGrade()
    {
        level++;
    }

    IEnumerator Shot() {
        for (int i = 0; i < spellPos.Length; i++)
        {
            GameObject instantsword = Instantiate(spell, spellPos[i].position, spellPos[i].rotation);
            Bullet newSword = instantsword.GetComponent<Bullet>();
            newSword.UpGradeBullet(level);
            Rigidbody bulletRigid = instantsword.GetComponent<Rigidbody>();
            bulletRigid.velocity = player.gameObject.transform.forward * 20;
            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
