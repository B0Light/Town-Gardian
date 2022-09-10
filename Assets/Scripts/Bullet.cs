using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   public int dmg;

   private void OnCollisionEnter(Collision collision)
   {
      if(collision.gameObject.tag == "Floor") Destroy(gameObject, 3f);
      else if(collision.gameObject.tag == "Wall") Destroy(gameObject);
   }
}
