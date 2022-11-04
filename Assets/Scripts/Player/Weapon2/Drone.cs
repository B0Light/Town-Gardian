using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] GameObject _bullet;
    [SerializeField] Transform _bulletPos;
    public List<Transform> atkPos;
    public float shortDis;
    private float fireDelay;
    private bool isFireReady;
    private float rate;
    public Orbit orbit;

    void Start()
    {
        rate = 0.5f;
    }

   

    // Update is called once per frame
    void Update()
    {
        fireDelay += Time.deltaTime;
        isFireReady = rate < fireDelay;
        atkPos.Clear();
        if (isFireReady)
            DroneFind();
    }

    void DroneFind()
    {
        
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position,
            20, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hitObj in raycastHits)
        {
            atkPos.Add(hitObj.transform);
        }

        if(atkPos.Count > 0)
        {
            foreach (Transform firePos in atkPos)
            {
                orbit.orbitSpeed = 0;
                transform.LookAt(firePos);
                Fire();
            }
        }

        
    }
    public void Fire()
    {
        StopCoroutine("Shot");
        StartCoroutine("Shot");
        fireDelay = 0;
    }

    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(_bullet, _bulletPos.position, _bulletPos.rotation);
        Bullet newbullet = instantBullet.GetComponent<Bullet>();
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = _bulletPos.forward * 100;
        orbit.orbitSpeed = -50;
        yield return null;
        
    }

}
