using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpellSword : Weapon
{
    private Player player;
    private Transform[] curPos;
    [SerializeField] GameObject spell;
    [SerializeField] Transform[] spellPos;
    [SerializeField] BoxCollider _meleeArea = null;
    public TrailRenderer _trailEffect;
    public int _dmg = 50;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public override void Use() {
        if (player._stamina.Value >= 10)
        {
            player._stamina.Value -= 10;
            curPos = spellPos;
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    public override void UpGrade()
    {
        level++;
    }

    IEnumerator Shot()
    {
        yield return new WaitForSeconds(0.1f);
        _meleeArea.enabled = true;
        _trailEffect.enabled = true;

        yield return new WaitForSeconds(0.4f);
        _meleeArea.enabled = false;

        yield return new WaitForSeconds(0.1f);
        _trailEffect.enabled = false;

        for (int i = 0; i < level; i++)
        {
            int pos = i % spellPos.Length;
            GameObject instantsword = Instantiate(spell, curPos[pos].position, curPos[pos].rotation);
            Bullet newSword = instantsword.GetComponent<Bullet>();
            newSword.UpGradeBullet(level);
            Rigidbody bulletRigid = instantsword.GetComponent<Rigidbody>();
            bulletRigid.velocity = player.gameObject.transform.forward * 20;
            yield return new WaitForSeconds(0.2f);
        }
        
    }
}
