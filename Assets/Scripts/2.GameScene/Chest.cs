using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Chest : MonoBehaviour
{
   private Animator anim;
   private int rewardValue;
   
   public GameObject[] reward;

   private void Start()
   {
      anim = GetComponentInChildren<Animator>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Player")
      {
         anim.SetTrigger("Open");

        
         StartCoroutine(ChestOpen());
         return;
      }
   }

   IEnumerator ChestOpen()
   {
      yield return new WaitForSeconds(2f);
      
      rewardValue = UnityEngine.Random.Range(0, reward.Length);
      GameObject ireward = Instantiate(reward[rewardValue], transform.position, Quaternion.identity);
      Rigidbody rewardRigid = ireward.GetComponent<Rigidbody>();
      rewardRigid.AddForce(Vector3.up+Vector3.forward*-1,ForceMode.Impulse);
      Destroy(gameObject);
   }
}
