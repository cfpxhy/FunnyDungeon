using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour {

    [SerializeField]
    int bulletSpeed;
    [SerializeField]
    int bulletDamage;

    public int BulletSpeed
    {
        get { return bulletSpeed; }
        set { bulletSpeed = value; }
    }
    public int BulletDamage
    {
        get { return bulletDamage; }
        set { bulletDamage = value; }
    }

    public virtual void OnSpawn()
    {
        
    }

    public virtual void Despawn()
    {
        
    }
}
