using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respond : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Player")
         other.transform.position = new Vector3(0, 0, -16);
   }
}
