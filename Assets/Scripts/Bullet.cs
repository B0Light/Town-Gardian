using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   public int dmg;
   public bool isMelee;
   private void OnCollisionEnter(Collision collision)
   {
      if(collision.gameObject.tag == "Floor") Destroy(gameObject, 3f);
      
   }

   private void OnTriggerEnter(Collider other)
   {
      if(!isMelee && other.gameObject.tag == "Wall") Destroy(gameObject);
   }
}
