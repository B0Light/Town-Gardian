using System;
using System.Collections;
using System.Collections.Generic;
using Core.UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]

public class Player : MonoBehaviour
{
    private GameManager manager;
    private UiManager _uiManager;
    public Camera followCamera;
    
    private static Player s_Instance = null;
    
    public int score;

    private Vector3 inputVec;

    public State state;

    private int maxAmmo = 9999;
    private int maxCoin = 9999999;
    public float maxHealth = 100;
    public float maxStamina = 100;
    public int maxHasGrendes;

    public int ammo;
    public int coin;
    public float health;
    public float stamina;
    public int hasGrendes = 0;
    public float jumpPower = 10;

    public float speed;
    public GameObject[] weapons;
    public int[] weaponsLv;
    public GameObject[] grenades;
    public bool[] hasWeapons;
    public GameObject grenadeObj;
    public GameObject hammerOrbit;
    public GameObject drone;
    public Vector3 moveVec;
    private Vector3 dodgeVec;
    private bool wDown; 
    private bool jDown;
    private bool dDown;
    private bool iDown;
    
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;
    private bool sDown4;

    private bool fDown;
    private float fireDelay;
    private bool isFireReady;
    private bool gDown;
    
    private bool isBorder;
    
    private bool rDown;
    private bool isRelaod;
    
    private bool isJump;
    private bool isDodge;
    private bool isSwap;
    private bool isDmg;
    private bool isShop;
    public bool isDead;
    
    private Animator anim;
    private Rigidbody rbody;
    private MeshRenderer[] meshs;
    
    private GameObject nearObject;
    // GM approach
    public Weapon equipWeapon;
    private int equipWeaponIndex = -1;
    private float angle;

    //
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
    [SerializeField] float rotateSpeed = 360;
    //Game Data
    public LevelList levelList;
    private GameDataStore m_DataStore;
    
    //
    // Start is called before the first frame update
    void Awake()
    {
        
        if (s_Instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        s_Instance = this;
        DontDestroyOnLoad(gameObject);
        
        anim = GetComponentInChildren<Animator>();
        rbody = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        //PlayerPrefs.SetInt("MaxScore", 199999);
        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
    }

    private void Start()
    {
        isFireReady = true;
        manager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        manager = FindObjectOfType<GameManager>();
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Grenade();
        Reload();
        Dodge();
        Swap();
        Interaction();
        if (weaponsLv[0] >= 10)
        {
            hammerOrbit.SetActive(true);
        }
        if (weaponsLv[1] >= 10)
        {
            drone.SetActive(true);
        }
        if(health <= 0 && !isDead)
        {
            isDead = true;
            OnDie();
        }
    }

    void GetInput()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        dDown = Input.GetButton("Jump");
        jDown = Input.GetKeyDown(KeyCode.F);
        
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");
    }

    void Move()
    {
        if (isDodge) {
            moveVec = dodgeVec;
        }
        else if (isSwap || !isFireReady || isDead) {
            moveVec = Vector3.zero;  //|| !isFireReady
        }
        else {
            inputVec = Camera.main.transform.TransformDirection(inputVec);
            inputVec.y = 0;
            inputVec = inputVec.normalized;


            //이동 방향을 부드럽게 변경한다
            angle = Vector3.Angle(transform.forward, inputVec);
            moveVec = Vector3.Slerp(transform.forward, inputVec, rotateSpeed * Time.deltaTime / angle);

        }

        if (!isBorder)
            transform.position += moveVec * speed * (wDown && stamina>0 ? 2f : 1f) * Time.deltaTime;

        if (wDown && !isBorder && stamina >= 0) stamina -= 30 * Time.deltaTime;
        if (!wDown && stamina < maxStamina) stamina += 10 * Time.deltaTime;    
        
            

        anim.SetBool("isRun", moveVec != Vector3.zero); 
        anim.SetBool("isRunFast", wDown);
        
        
    }

    void Turn()
    {
        // 이동속도가 현저히 적거나 죽은 상태일 경우 회전을 실행하지 않는다.
        if (moveVec.magnitude < 0.5f || isDead) 
        {
            Vector3 lookat = Camera.main.transform.forward;
            lookat.y = 0;
            transform.forward = lookat;
            return;
        }
            
        // 캐릭터를 나아가는 방향으로 보게 한다.
        transform.forward = moveVec;
         //마우스 회전
         /*
         if (!isDead)
         {
             Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
             RaycastHit rayHit;
             if (Physics.Raycast(ray, out rayHit, 100))
             {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
             }
         }
         */
    }

    void Jump()
    {
        if (jDown && !isJump && !isDodge && !isSwap  &&!isShop && !isDead)
        {
            rbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true); 
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if (hasGrendes == 0) return;
        if (gDown && !isRelaod && !isSwap  &&!isShop && !isDead)
        {
            Vector3 GrenadePos = Vector3.zero;
            GrenadePos.y = 15;
            GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
            Rigidbody rbodyGrenade = instantGrenade.GetComponent<Rigidbody>();
            rbodyGrenade.velocity = transform.forward * 15;
            rbodyGrenade.AddForce(GrenadePos, ForceMode.Impulse);
            rbodyGrenade.AddTorque(Vector3.back * 15, ForceMode.Impulse);
            hasGrendes--;
            grenades[hasGrendes].SetActive(false);
            
        }
    }
    void Attack()
    {
        if (equipWeapon == null) return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        // 공격을 실행한다.
        if (fDown && isFireReady && !isDodge && !isSwap && !isRelaod &&!isShop && !isDead)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type  == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }

    }

    void Reload()
    {
        if (equipWeapon == null || equipWeapon.type == Weapon.Type.Melee) return;
        if (ammo == 0) return;
        if (rDown && !isDodge && !isJump && !isSwap && isFireReady  &&!isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isRelaod = true;
            speed *= 0.2f;
            Invoke("ReloadOut", 1.5f);
        }
    }

    void ReloadOut()
    {
        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        Range range = equipWeapon.GetComponent<Range>();
        int leftBullet = range._curAmmo;
        int reAmmo = ammo < range._maxAmo ? ammo : range._maxAmo;
        range._curAmmo = reAmmo;
        ammo -= reAmmo;             
        ammo += leftBullet;         
                                
        speed *= 5f;
        isRelaod = false;
    }
    void Dodge()
    {
        if (dDown && !isJump && !isDodge && !isSwap &&!isShop && !isDead && stamina >= 5)
        {
            stamina -= 35;
            dodgeVec = inputVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            
            Invoke("DodgeOut", .5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) return;
        if (sDown4 && (!hasWeapons[3] || equipWeaponIndex == 3)) return;
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        if (sDown4) weaponIndex = 3;
        if ((sDown1 || sDown2 || sDown3|| sDown4) && !isRelaod && !isDodge && !isJump && !isDead)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            /*
            if (manager.SType == GameManager.SceneType.Game)
            {
                if(equipWeapon.type == Weapon.Type.Range) _uiManager.Aiming();
                else _uiManager.EndAiming();
            }
            */
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }
    
    void SwapOut()
    {
        isSwap = false;
    }
    void Interaction()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                if(!hasWeapons[weaponIndex])
                    hasWeapons[weaponIndex] = true;
                else
                {
                    // upGrade item;
                    if (weaponIndex == 0)
                    {
                        weapons[weaponIndex].GetComponent<Melee>().UpGrade();
                    }
                    else if (weaponIndex == 3)
                    {
                        weapons[weaponIndex].GetComponent<SpellSword>().UpGrade();
                    }
                    else
                    {
                         weapons[weaponIndex].GetComponent<Range>().UpGrade(); 
                    }
                    weaponsLv[weaponIndex]++;
                }
                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall")
        {
            if(isJump == true)
            {
                anim.SetBool("isJump",false);
                isJump = false;
                jumpPower = 10;
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isDead) return;
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo) 
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) 
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.MaxHeart:
                    maxHealth += item.value;
                    health += item.value;
                    break;
                case Item.Type.Grenade:
                    hasGrendes++;
                    if (hasGrendes > maxHasGrendes)
                        hasGrendes = maxHasGrendes;
                    grenades[hasGrendes-1].SetActive(true);
                    break;
                case Item.Type.JumpPack:
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if(!isDmg){
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.dmg;

                bool isBossAtk = (other.name == "Boss Melee Area");
                StartCoroutine(OnDmg(isBossAtk));
            }
            if(other.GetComponent<Rigidbody>() != null) Destroy(other.gameObject);
        }
    }

    IEnumerator OnDmg(bool isBossAtk)
    {
        isDmg = true;
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.yellow+Color.red;
        
        if(isBossAtk)
            rbody.AddForce(transform.forward * -20, ForceMode.Impulse);
            
        if (health <= 0 && !isDead)
            OnDie();
        
        yield return new WaitForSeconds(1f);
        isDmg = false;

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.white;
        
        if(isBossAtk)
            rbody.velocity = Vector3.zero;
    }
    public void HitByGrenade(Vector3 explosionPos)
    {
        health -= 10;
        OnDmg(false);
        Vector3 reactVec = transform.position - explosionPos;
        reactVec += Vector3.up * 3;
        rbody.freezeRotation = false;
        rbody.AddTorque(reactVec * 15, ForceMode.Impulse);
        rbody.AddForce(transform.forward * -1, ForceMode.Impulse);
    }
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        StopAllCoroutines();
        manager.GameOver();
    }
    
    void FreezeRotation()
    {
        rbody.angularVelocity = Vector3.zero;
    }
    
    void StopToWall()
    {
        Vector3 chkWall = new Vector3(0,1,0) + transform.position;
        Debug.DrawRay(transform.position, transform.forward * 5, Color.magenta);
        isBorder = Physics.Raycast(chkWall, moveVec,0.5f, LayerMask.GetMask("Wall"));
    }
    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop" || other.tag == "Ship")
            nearObject = other.gameObject;
       //Debug.Log(nearObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        if (other.tag == "Shop" && nearObject != null)
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
    
    // return : -180 ~ 180 degree (for unity)
    public static float GetAngle(Vector3 vStart, Vector3 vEnd) {
        Vector3 v = vEnd - vStart;
        return Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
    }

    public void GameOver()
    {
        manager.Restart();
    }
}
