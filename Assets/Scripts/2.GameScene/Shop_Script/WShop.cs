using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WShop : Shop
{
    // Start is called before the first frame update
    void Start()
    {
        itemPrice[0] = 1000;
        itemPrice[1] = 2000;
        itemPrice[2] = 5000;
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
        if (enterPlayer.hasWeapons[index] == false)
        {
            enterPlayer.hasWeapons[index] = true;
        }
        else
        {
            if (index == 0)
            {
                enterPlayer.weapons[index].GetComponent<Melee>().UpGrade();
            }
            else if (index == 3)
            {
                enterPlayer.weapons[index].GetComponent<SpellSword>().UpGrade();
            }
            else
            {
                enterPlayer.weapons[index].GetComponent<Range>().UpGrade();
            }
            enterPlayer.weaponsLv[index]++;
        }
        UpdateShop(index);

    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }

    public void UpdateShop(int index)
    {
        itemPrice[index] += enterPlayer.weaponsLv[index] * 300 * (index+1);
        itemPriceTexts[index].text = itemPrice[index] + "G";
    }
}
