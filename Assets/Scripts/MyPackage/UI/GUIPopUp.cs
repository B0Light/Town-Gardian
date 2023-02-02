using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIPopUp : GUIWindow
{
    protected override void Open()
    {
        if (UIManager.Instance.NowPopUp == null)
        {
            base.Open();
            return;
        }

        RectTransform rect = gameObject.GetComponent<RectTransform>();

        rect.SetParent(UIManager.Instance.NowPopUp.transform);
        rect.offsetMax = Vector3.zero;
        rect.offsetMin = Vector3.zero;
    }
}
