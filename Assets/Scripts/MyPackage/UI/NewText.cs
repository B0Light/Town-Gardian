using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
[AddComponentMenu("UI/Legacy/NewText", 100)]
public class NewText : Text
{
    // Start is called before the first frame update
    [SerializeField] protected string m_Key = String.Empty;


    override protected void OnEnable()
    {
        //SetText(m_Key);
        base.OnEnable();
    }

    public void SetText(string key)
    {
        text = StringManager.Instance.GetStringByKey(key);

        if (text == string.Empty)
            text = key;
        
    }
}
