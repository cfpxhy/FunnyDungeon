using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour {

    public event Action<int, int> HpChanged; //血量变化
    public event Action<EnemyBase> Dead; //死亡

    [SerializeField]
    int m_Hp;
    [SerializeField]
    int m_MaxHp;
    [SerializeField]
    int m_Speed;
    [SerializeField]
    GameObject m_DeadEffect;

    float m_HurtTimer = 0.1f;

    public int Hp
    {
        get { return m_Hp; }
        set
        {
            //范围约定
            value = Mathf.Clamp(value, 0, m_MaxHp);

            //减少重复
            if (value == m_Hp)
                return;

            //赋值
            m_Hp = value;

            //血量变化
            if (HpChanged != null)
                HpChanged(m_Hp, m_MaxHp);

            //死亡事件
            if (m_Hp == 0)
            {
                if (Dead != null)
                    Dead(this);
            }
        }
    }

    public int MaxHp
    {
        get { return m_MaxHp; }
        set
        {
            if (value < 0)
                value = 0;

            m_MaxHp = value;
        }
    }

    public bool IsDead
    {
        get { return m_Hp == 0; }
    }

    public int Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }

    public float HurtTimer
    {
        get { return m_HurtTimer; }
    }

    public GameObject DeadEffect
    {
        get { return m_DeadEffect; }
        set { m_DeadEffect = value; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public virtual void Damage(int hit)
    {
        Hp -= hit;
        if (IsDead)
        {
            Despawn();
        }
    }

    public virtual void Move()
    {
        
    }

    public virtual void OnSpawn()
    {
        
    }

    public virtual void Despawn()
    {
        //HpChanged = null;
        //Dead = null;

        //m_Hp = 0;
        //m_MaxHp = 0;
    }
}
