using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageZone : MonoBehaviour
{
    public ZoneManager zoneManager;
    public GameObject[] carSpawner;
    public int zoneNum;
    
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            zoneManager.EnterZone(zoneNum);
    }
}
