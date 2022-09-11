using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        A,
        B,
        C
    };

    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAtk;
    
    private Rigidbody rbody;
    private BoxCollider _boxCollider;
    //private Material mat;
    private MeshRenderer[] meshs;
    private NavMeshAgent nav;
    private Animator anim;
    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        
        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        if(nav.enabled)
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
        float targetRadius = 1.5f;
        float targetRange = 3f;

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
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
        if (rayHits.Length > 0 && !isAtk && curHealth > 0)
        {
            StartCoroutine(Atk());
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
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.dmg;
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
            isChase = false;
            nav.enabled = false;
            
            if(isGrenade)
            { 
                reactVec += Vector3.up * 3;
                rbody.freezeRotation = false;
                rbody.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            
            rbody.AddForce(reactVec * 5, ForceMode.Impulse);
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
        
    }
}
