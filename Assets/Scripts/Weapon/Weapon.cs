using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };

    public Type type;

    public float rate;

    abstract public void Use();

}
