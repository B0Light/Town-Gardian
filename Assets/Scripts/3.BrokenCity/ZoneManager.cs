using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : GameManager
{
    public GameObject[] jumpZone;
    public GameObject[] StageZone;
    private bool[] isClear;

    protected override void Start()
    {
        base.Start();
        isClear = new bool[StageZone.Length];
        for(int i = 0; i < isClear.Length; i++)
        {
            isClear[i] = false;
        }
        bossStage = 4;
    }

    public void EnterZone(int index)
    {
        if (isClear[index] == false)
        {
            isClear[index] = true;
            StageStart();
        }
    }

    public override void StageEnd()
    {
        base.StageEnd();
        if(stage < jumpZone.Length)
        {
            jumpZone[stage].SetActive(true);
        }
    }

}
