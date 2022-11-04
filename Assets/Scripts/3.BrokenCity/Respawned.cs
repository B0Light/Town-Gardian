using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawned : MonoBehaviour
{
    public Transform startPos;
   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Player")
        {
            other.transform.position = startPos.position;
        }
         
         
   }
}
