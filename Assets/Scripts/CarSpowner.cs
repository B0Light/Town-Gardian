using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CarSpowner : MonoBehaviour
{
    public GameObject car;
    public Transform[] carPos;
    public GameManager manager;
    void Start()
    {
        
    }
    
    void Update()
    {
        StartCoroutine(lanch());
    }

    IEnumerator lanch()
    {
        yield return new WaitForSeconds(5f);
        int carPosition = UnityEngine.Random.Range(0, carPos.Length);
        GameObject instantCar = Instantiate(car, carPos[carPosition].position, carPos[carPosition].rotation);
        Rigidbody rbodyBullet = instantCar.GetComponent<Rigidbody>();
        rbodyBullet.velocity = transform.forward * UnityEngine.Random.Range(10,20+manager.stage*3);
                
        yield return new WaitForSeconds(25f);
    }
}
