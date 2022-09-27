using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : Weapon {

    public int _maxAmo;
    public int _curAmmo;

    [SerializeField] GameObject _bullet;
    [SerializeField] Transform _bulletPos;

    [SerializeField] GameObject _bulletCase;
    [SerializeField] Transform _bulletCasePos;

    public override void Use() {
        if (_curAmmo > 0) {
            _curAmmo--;
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    public override void UpGrade()
    {
        level++;
    }

    IEnumerator Shot() {
        GameObject instantBullet = Instantiate(_bullet, _bulletPos.position, _bulletPos.rotation);
        Bullet newbullet = instantBullet.GetComponent<Bullet>();
        newbullet.UpGradeBullet(level);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = _bulletPos.forward * 50;
        yield return null;
        GameObject instantCase = Instantiate(_bulletCase, _bulletCasePos.position, _bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = _bulletCasePos.forward * Random.Range(-2, -1) + Vector3.up;
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up, ForceMode.Impulse);
    }
}
