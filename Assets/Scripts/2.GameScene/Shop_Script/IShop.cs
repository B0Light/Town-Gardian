using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IShop : Shop
{
    // Start is called before the first frame update
    void Start()
    {
        itemPrice[0] = 2000;
        itemPrice[1] = 1000;
        itemPrice[2] = 500;
        for (int i = 0; i < itemPrice.Length; i++)
        {
            itemPriceTexts[i].text = itemPrice[i] + "G";
        }
    }

    public override void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer._coin.Value)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        enterPlayer._coin.Value -= price;
        Instantiate(itemObj[index], itemPos[index].position, itemPos[index].rotation);

    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }

}
