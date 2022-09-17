using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    private Rigidbody rbody;

    private float angularPower = 2;

    private float scaleValue = 0.1f;

    private bool isShoot;
    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 0.04f;
            scaleValue += 0.002f;
            transform.localScale = Vector3.one * scaleValue;
            rbody.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
    }
   
}
