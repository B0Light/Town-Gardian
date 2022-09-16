using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        A,
        B,
        C,
        D
    };

    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score;

    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    
    public bool isChase;
    public bool isAtk;
    public bool isDead;
    
    protected Rigidbody rbody;
    protected BoxCollider _boxCollider;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;
    protected Animator anim;
    protected int deathCoin;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if(nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rbody.velocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;
        }
        
    }

    void Targeting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 1.5f;
            float targetRange = 1f;
    
            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 0.5f;
                    targetRange = 1f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !isAtk)
            {
                StartCoroutine(Atk());
            }
        }
        
    }

    IEnumerator Atk()
    {
        isChase = false;
        isAtk = true;
        anim.SetBool("isAtk",true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rbody.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rbody.velocity = Vector3.zero;
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rbodyBullet = instantBullet.GetComponent<Rigidbody>();
                rbodyBullet.velocity = transform.forward * 20;
                
                yield return new WaitForSeconds(2f);
                break;
        }
        
        
        isChase = true;
        isAtk = false;
        anim.SetBool("isAtk",false);
    }
    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 reactVec = transform.position - other.transform.position;
        if (other.tag == "Melee")
        {
            Melee weapon = other.GetComponent<Melee>();
            curHealth -= weapon._dmg;
            StartCoroutine(OnDmg(reactVec, false));
        }else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.dmg;
            Destroy(other.gameObject); // 총알 관통 불가
            StartCoroutine(OnDmg(reactVec, false));
        }
        
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDmg(reactVec, true));
    }

    IEnumerator OnDmg(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;
        if(curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;
            anim.SetTrigger("doDie");
            isDead = true;
            isChase = false;
            nav.enabled = false;
            
            Player player = target.GetComponent<Player>();
            player.score += score;
            
            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    deathCoin = 0;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    deathCoin = 1;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    deathCoin = 1;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    deathCoin = 2;
                    break;
            }
            Instantiate(coins[deathCoin],transform.position, Quaternion.identity);
            
            
            if(isGrenade)
            { 
                reactVec += Vector3.up * 3;
                rbody.freezeRotation = false;
                rbody.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            
            rbody.AddForce(reactVec * 5, ForceMode.Impulse);
            Destroy(gameObject, 4f);
            
        }
        
    }
}
