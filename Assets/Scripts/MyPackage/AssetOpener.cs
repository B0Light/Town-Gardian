using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetOpener : MonoBehaviour
{
    public static T Import<T>(string path) where T: Object
    {
        T source = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
        return Instantiate(source);
    }
    public static T Import<T>(string path, string parentName) where T : Object
    {
        T source = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
        return Instantiate(source, GameObject.Find(parentName).transform);
    }
}
