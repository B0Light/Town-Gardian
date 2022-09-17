using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isMelee;
    public bool isRock;
    public float bulletlife;

    private void Start()
    {
        bulletlife = 0;
    }

    private void Update()
    {
        bulletlife += Time.deltaTime;
        if(bulletlife > 10f && !isMelee) Destroy(gameObject);
    }

    public void UpGradeBullet(int level)
    {
        dmg += (level*5);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor") Destroy(gameObject, 3f);
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isRock && other.gameObject.tag == "Wall") Destroy(gameObject);
        else if(!isMelee && !isRock && other.gameObject.tag == "Floor") Destroy(gameObject);
    }
}
