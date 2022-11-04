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
    protected Player enterPlayer;
    // Start is called before the first frame update

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.gameObject.SetActive(true);
        uiGroup.anchoredPosition = Vector3.zero;
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

    public virtual void Buy(int index) { }
    
    
}
