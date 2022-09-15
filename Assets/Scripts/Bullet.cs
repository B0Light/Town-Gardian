using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   public enum Type
   {
      HandGun,
      MachineGun,
      Missile,
      Rock,
      Melee
   };
   public Type type;
   public int dmg;
   public bool isMelee;
   public bool isRock;
   public float bulletlife;
   public int lv;
   private void Start()
   {
      bulletlife = 0;   
   }

   public void SetUp(int newlv)
   {
      dmg *= newlv;
   }
   private void Update()
   {
       bulletlife += Time.deltaTime;
       if(bulletlife > 10f && type != Type.Melee) Destroy(gameObject);
       else if (type == Type.Rock && bulletlife > 5f) Destroy(gameObject);
       else if (type == Type.Missile && bulletlife > 7f) Destroy(gameObject);
       
   }
   private void OnCollisionEnter(Collision collision)
   {
      if(collision.gameObject.tag == "Floor") 
         if(type != Type.Rock && type != Type.Melee)
            Destroy(gameObject);
   }

   private void OnTriggerEnter(Collider other)
   {
      if(!isMelee && other.gameObject.tag == "Floor") Destroy(gameObject);
   }
}
