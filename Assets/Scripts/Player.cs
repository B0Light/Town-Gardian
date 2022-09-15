using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameManager manager;
    public Camera followCamera;
    public int ammo;
    public int coin;
    public int health;
    public int hasGrendes = 0;
    
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrendes;

    public int score;
    
    private float hAxis;
    private float vAxis;
    
    public float speed;
    public GameObject[] weapons;
    public GameObject[] grenades ;
    public bool[] hasWeapons;
    public GameObject grenadeObj;
    public Vector3 moveVec;
    private Vector3 dodgeVec;
    private bool wDown; 
    private bool jDown;
    private bool dDown;
    private bool iDown;
    
    private bool sDown1;
    private bool sDown2;
    private bool sDown3;

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
    private bool isDead;
    
    private Animator anim;
    private Rigidbody rbody;
    private MeshRenderer[] meshs;
    private Transform camPos;
    
    private GameObject nearObject;
    // GM approach
    public Weapon equipWeapon;
    private int equipWeaponIndex = -1;
    
    //
    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    public Image crossHair;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    //
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rbody = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        camPos = followCamera.GetComponent<Transform>();

        //PlayerPrefs.SetInt("MaxScore", 199999);
        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
    }

    private void Start()
    {
        isFireReady = true;
        crossHair.color = new Color(0, 0, 0,0);
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
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
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
    }

    void Move()
    {
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        //rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        //rotationY = Mathf.Clamp (rotationY, -45, 45);
		
        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        moveVec = this.transform.TransformDirection(moveVec);
        moveVec.y = 0;
        if (isDodge) moveVec = dodgeVec;
        if (isSwap || !isFireReady || isRelaod || isDead) moveVec = Vector3.zero;  //|| !isFireReady
        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) *Time.deltaTime;
        anim.SetBool("isRun", moveVec != Vector3.zero); 
        anim.SetBool("isWalk", wDown); 
    }

    void Turn()
    { 
        //키보드 회전
        //if (!isDead) 
            //transform.LookAt(transform.position + moveVec); //나아가는 방향으로 바라 봄 
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
            rbody.AddForce(Vector3.up * 15, ForceMode.Impulse);
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
            
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmo ? ammo : equipWeapon.maxAmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isRelaod = false;
    }
    void Dodge()
    {
        if (dDown && !isJump && !isDodge && !isSwap &&!isShop && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            
            Invoke("DodgeOut", 0.5f);
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
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        if ((sDown1 || sDown2 || sDown3) && !isDodge && !isJump && !isDead)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); 
            
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
            
            if(equipWeapon == null) crossHair.color = new Color(0,0,0,0);
            else if (equipWeapon.type == Weapon.Type.Range) 
                crossHair.color = new Color(1, 1, 1,1);
            else if (equipWeapon.type == Weapon.Type.Melee) 
                crossHair.color = new Color(0, 0, 0,0);

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
                if (hasWeapons[weaponIndex])
                {
                    Weapon UpWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                    UpWeapon.UpGrade();
                }
                else
                {
                    hasWeapons[weaponIndex] = true;
                }
                    
                Destroy(nearObject);
                
            }else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            } 
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump",false);
            isJump = false;
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
                case Item.Type.Grenade:
                    hasGrendes++;
                    if (hasGrendes > maxHasGrendes)
                        hasGrendes = maxHasGrendes;
                    grenades[hasGrendes-1].SetActive(true);
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if(!isDmg){
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.dmg;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDmg(isBossAtk));
            }
            if(other.GetComponent<Rigidbody>() != null) Destroy(other.gameObject);
        }
    }

    IEnumerator OnDmg(bool isBossAtk)
    {
        isDmg = true;
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.yellow;
        
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

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
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
        if (other.tag == "Weapon" || other.tag == "Shop")
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
}
