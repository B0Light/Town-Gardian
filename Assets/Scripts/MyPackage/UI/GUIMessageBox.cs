using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GUIMessageBox : GUIPopUp {

    [SerializeField] NewText _textField;

    Action _buttonMethod;

    public void SetMessage(string key)
    {
        SetMessage(key, CloseMessageBox);
    }

    public void SetMessage(string key, Action buttonMethod)
    {
        _textField.SetText(key);
        gameObject.SetActive(true);

        _buttonMethod = buttonMethod;
    }

    public void MessageBoxButton()
    {
        _buttonMethod.Invoke();

        if (_buttonMethod != CloseMessageBox) return;

        CloseMessageBox();
    }

    public void CloseMessageBox()
    {
        Destroy(gameObject);
    }

}
