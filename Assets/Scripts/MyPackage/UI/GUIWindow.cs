using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIWindow : MonoBehaviour {
    // Start is called before the first frame update

    public int priority = 0;

    private void Awake()
    {
        Open();
    }

    protected virtual void Open()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();

        rect.SetParent(GameObject.Find("Canvas").transform);
        rect.offsetMax = Vector3.zero;
        rect.offsetMin = Vector3.zero;
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }

    public void OpenWindow(string key) {
        UIManager.OpenGUI<GUIWindow>(key);
    }

}
