using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInfo
{
    public int ID { get; set; }
    public GameObject Prefab { get; set; }
    //Editor
    public override string ToString()
    {
        return string.Format("ID:{0}, PREFAB:{1}", ID, Prefab.name);
    }
}