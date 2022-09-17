using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;
    
    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;
    private Player enterPlayer;
    // Start is called before the first frame update
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
    public void Exit()
    { 
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        enterPlayer.coin -= price;
        Instantiate(itemObj[index], itemPos[index].position, itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
