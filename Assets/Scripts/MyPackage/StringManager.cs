using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.XR;

public class StringManager : MonoSingleton<StringManager> {
    // Start is called before the first frame update

    class StringData {
        public string name;
        public string value;

        public void Read(XmlNode node)
        {
            name = node.Attributes["name"].Value;
            value = node.Attributes["value"].Value;
        }
    }

    Dictionary<string, StringData> _dic;

    string _nowLang = "Kor";
    string _fileName = "String.xml";

    protected override void OnCreate()
    {
        ReadStringFromXml(_nowLang);
    }

    public void ReadStringFromXml(string lang)
    {
        _dic = new Dictionary<string, StringData>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets/XML/" + lang + "/" + _fileName);

        XmlNodeList nodes = xmlDoc.SelectNodes("StringData/String");

        for (int i = 0; i < nodes.Count; i++)
        {
            StringData stringData = new StringData();
            stringData.Read(nodes[i]);

            _dic.Add(stringData.name, stringData);
        }

    }

    public string GetStringByKey(string key) {

        if (_dic.TryGetValue(key, out StringData temp)) {
            return temp.value;
        }
        return string.Empty;

    }
}
