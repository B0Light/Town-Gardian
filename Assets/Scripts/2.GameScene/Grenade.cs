using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rbody; 
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2f);
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position,
            20, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hitObj in raycastHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject, 5);
    }
}
