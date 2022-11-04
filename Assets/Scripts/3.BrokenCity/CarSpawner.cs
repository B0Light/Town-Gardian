using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CarSpawner : MonoBehaviour
{
    public GameObject car;
    public Transform[] carPos;
    public GameManager manager;
    bool isSpawn;
    
    void Update()
    {
        carSpawn();
    }

    void carSpawn()
    {
        if (isSpawn == false)
        {
            isSpawn = true;
            StartCoroutine(lanch());
        }
    }

    IEnumerator lanch()
    {
        yield return new WaitForSeconds(5f);
        int carPosition = UnityEngine.Random.Range(0, carPos.Length);
        GameObject instantCar = Instantiate(car, carPos[carPosition].position, carPos[carPosition].rotation);
        Rigidbody rbodyBullet = instantCar.GetComponent<Rigidbody>();
        rbodyBullet.AddForce(instantCar.transform.forward* UnityEngine.Random.Range(10, 20), ForceMode.Impulse);
        yield return new WaitForSeconds(5f);
        isSpawn = false;
    }
}
