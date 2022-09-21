using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable]
public class Stat {
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrendes;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrendes = 0;

}

public class Player : MonoBehaviour
{
    public GameManager manager;
    public Camera followCamera;

    public int score;

    private Vector3 inputVec;

    public Stat stat;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public float maxStamina;
    public int maxHasGrendes;

    public int ammo;
    public int coin;
    public int health;
    public float stamina;
    public int hasGrendes = 0;

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

    //
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rbody = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        //PlayerPrefs.SetInt("MaxScore", 199999);
        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
    }

    private void Start()
    {
        isFireReady = true;
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
    }

    void Move()
    {
        if (isDodge) {
            moveVec = dodgeVec;
        }
        else if (isSwap || !isFireReady || isRelaod || isDead) {
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
            
            Invoke("ReloadOut", 3f);
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
        ammo -= reAmmo;             // 30발 장전할 수 있는데 2발 남기고 재장전 하면 28개가 소모되는게 아닌 30개가 소모되면 문제가 생기지 않을까 합니다.
        ammo += leftBullet;         // Range 클래스에 Reload 함수를 만들고 그 반환값을 소모한 개수(28개)로 삼고 그 개수만큼 빼주면 문제가 해결될 듯 합니다.
                                    // 다른 방법으로 해결하였습니다!! 잔여 총알 개수를 먼저 가저온 후 재장전 후 보유 총알에 잔여총알을 더하였습니다.

        isRelaod = false;
    }
    void Dodge()
    {
        if (dDown && !isJump && !isDodge && !isSwap &&!isShop && !isDead && stamina >= 5)
        {
            stamina -= 30;
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
            if(equipWeapon.type == Weapon.Type.Range) manager.Aiming();
            else manager.EndAiming();
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
                    else
                    {
                         weapons[weaponIndex].GetComponent<Range>().UpGrade();
                    }
                       
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
    // return : -180 ~ 180 degree (for unity)
    public static float GetAngle(Vector3 vStart, Vector3 vEnd) {
        Vector3 v = vEnd - vStart;
        

        return Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
    }
}
