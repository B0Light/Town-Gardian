using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };

    public Type type;

    public float rate;
    protected int level = 1;

    abstract public void Use();

    abstract public void UpGrade();

}
