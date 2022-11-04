using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    protected Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        transform.LookAt(player.transform);
    }
}
