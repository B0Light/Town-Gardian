using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIInforBox : MonoBehaviour {

    [SerializeField] Text _timeHourTenText;
    [SerializeField] Text _timeHourOneText;
    [SerializeField] Text _timeMinuteText;

    void Start() {
        UIManager.Instance.InforBox = this;
    }

    public void SetTimeText(float gameTime) {

        int planetTime = (int)(gameTime / 15);

        _timeHourTenText.text = (planetTime / 10).ToString();
        _timeHourOneText.text = (planetTime % 10).ToString();

        _timeMinuteText.text = (((int)((gameTime % 15) * 0.4f))).ToString();
    }

}
