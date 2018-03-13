using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EnemyInfo {
    public int ID { get; set; }
    //public string url { get; set; }
    public GameObject Prefab;
    //Editor
    public override string ToString()
    {
        return string.Format("ID:{0}, PREFAB:{1}", ID, Prefab.name);
    }
}
