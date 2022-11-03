using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;
    
    public GameObject[] itemObj;
    protected int[] itemPrice = new int[3];
    public Transform[] itemPos;
    public Text[] itemPriceTexts;
    public Text talkText;
    public string[] talkData;
    private Player enterPlayer;
    // Start is called before the first frame update

    public void Start()
    {
        itemPrice[0] = 1000;
        itemPrice[1] = 2000;
        itemPrice[2] = 5000;
        for (int i = 0; i < itemPrice.Length; i++)
        {
            itemPriceTexts[i].text = itemPrice[i] + "G";
        }
    }

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.gameObject.SetActive(true);
        uiGroup.anchoredPosition = Vector3.zero;
        for (int i = 0; i < itemPrice.Length; i++)
        {
            itemPrice[i] += enterPlayer.weaponsLv[i] * 1000;
            itemPriceTexts[i].text = itemPrice[i] + "G";
        }
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
    public void Exit()
    { 
        anim.SetTrigger("doHello");

        uiGroup.gameObject.SetActive(false);
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
            UpdateShop(index);
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

    public void UpdateShop(int index)
    {
       
     itemPriceTexts[index].text = itemPrice[index] + "G";
       
    }
}
