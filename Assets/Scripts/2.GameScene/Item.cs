using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Ammo, Coin, Grenade, Heart, Weapon, MaxHeart
    }

    public Type type;

    public int value;
    private Rigidbody rbody;
    private SphereCollider _sphereCollider;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rbody.isKinematic = true;
            _sphereCollider.enabled = false;
        }
    }
}
