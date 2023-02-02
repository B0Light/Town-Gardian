using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIFullScreen : GUIWindow
{
    protected override void Open()
    {
        base.Open();
        UIManager.Instance.EnrollmentGUI(this);
    }

    public override void Close()
    {
        UIManager.Instance.Pop();
        base.Close();
    }
}
