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
        C2,
        D,
        D2
    };

    public Type enemyType;
    public float maxHealth;
    public float curHealth;
    public int score;

    public Gauge<float> _health;

    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] rewards;
    
    public bool isChase;
    public bool isAtk;
    public bool isDead;
    
    protected Rigidbody rbody;
    protected BoxCollider _boxCollider;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;
    protected Animator anim;
    protected int deathCoin;
    protected int takenDmg;
    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if((enemyType != Type.D || enemyType != Type.D2))
            Invoke("ChaseStart", 2);
    }

    public void Upgrade(int stage)
    {
        _health = new Gauge<float>(_health.GetMaxValue() + stage * 50);
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
        if(nav.enabled && (enemyType != Type.D || enemyType != Type.D2))
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
        if (!isDead && !(enemyType == Type.D || enemyType == Type.D2))
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
                case Type.C2:
                    targetRadius = 1f;
                    targetRange = 100f;
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
        if((enemyType != Type.D || enemyType != Type.D2))
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
                rbody.AddForce(transform.forward * 40, ForceMode.Impulse);
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
                rbodyBullet.velocity = transform.forward * UnityEngine.Random.Range(20,20+manager.stage*3);
                
                yield return new WaitForSeconds(2f);
                break;
            case Type.C2:
                yield return new WaitForSeconds(0.5f);
                Vector3 GrenadePos = Vector3.zero;
                GrenadePos.y = 15;
                GameObject instantGrenade = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rbodyGrenade = instantGrenade.GetComponent<Rigidbody>();
                rbodyGrenade.velocity = transform.forward * 20;
                
                rbodyGrenade.AddForce(GrenadePos, ForceMode.Impulse);
                rbodyGrenade.AddTorque(Vector3.back * 15, ForceMode.Impulse);

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
        if (isDead) return;
        if (other.tag == "Melee")
        {
            Melee weapon = other.GetComponent<Melee>();
            takenDmg = weapon._dmg;
            StartCoroutine(OnDmg(reactVec, false));
        }else if (other.tag == "SpellSword")
        {
            SpellSword weapon = other.GetComponent<SpellSword>();
            takenDmg = weapon._dmg;
            StartCoroutine(OnDmg(reactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            takenDmg = bullet.dmg;
            if(bullet.isMelee == false)
                Destroy(other.gameObject); // 총알 관통 불가
            StartCoroutine(OnDmg(reactVec, false));
        }
        
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        if (isDead) return;
        _health.Value -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDmg(reactVec, true));
    }

    IEnumerator OnDmg(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _health.Value -= takenDmg;
        if (_health.Value < 0) _health.Value = 0;
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;
        if(_health.Value > 0)
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
            
            Player player = target.GetComponent<Player>();
            player.score += score;
            if (!isDead)
            {
            switch (enemyType)
                {
                    case Type.A:
                        manager.enemyCntA--;
                        deathCoin = UnityEngine.Random.Range(0, rewards.Length);
                        //player.coin += 100;
                        player._coin.Value += 100;
                        break;
                    case Type.B:
                        manager.enemyCntB--;
                        deathCoin = UnityEngine.Random.Range(0, rewards.Length);
                        //player.coin += 1000;
                        player._coin.Value += 1000;
                        break;
                    case Type.C:
                        manager.enemyCntC--;
                        deathCoin = UnityEngine.Random.Range(0, rewards.Length);
                        //player.coin += 500;
                        player._coin.Value += 500;
                        break;
                    case Type.C2:
                        manager.enemyCntA--;
                        deathCoin = UnityEngine.Random.Range(0, rewards.Length);
                        //player.coin += 800;
                        player._coin.Value += 800;
                        break;
                    case Type.D:
                        manager.enemyCntD--;
                        deathCoin = UnityEngine.Random.Range(0, rewards.Length);
                        //player.coin += 5000;
                        player._coin.Value += 5000;
                        break;
                }
                GameObject coin = Instantiate(rewards[deathCoin], transform.position, Quaternion.identity);
                Rigidbody rcoin = coin.GetComponent<Rigidbody>();
                rcoin.AddForce(Vector3.up * 5, ForceMode.Impulse);
            }
            isDead = true;
            
            //rcoin.AddTorque(Vector3.forward * 20, ForceMode.Impulse);
            
            if(isGrenade)
            { 
                reactVec += Vector3.up * 3;
                rbody.freezeRotation = false;
                rbody.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            
            rbody.AddForce(reactVec * 5, ForceMode.Impulse);
            Destroy(gameObject, 4f);
            this.enabled = false;
            
        }
        
    }
}
