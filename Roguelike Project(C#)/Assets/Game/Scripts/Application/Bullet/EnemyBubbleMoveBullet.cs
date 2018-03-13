using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBubbleMoveBullet : BulletBase {

    private Animator ani;
    private Collider2D collider;
    private SpawnPool bulletSpawnPool;

    private SkillCD skillCD;

    private bool isCollision = false;

    private GameObject[] enemy;
    void Start()
    {
        ani = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();
        bulletSpawnPool = PoolManager.Pools["Bullet"];
        enemy = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject go in enemy)
        {
            Physics2D.IgnoreCollision(collider, go.GetComponent<Collider2D>());
        }
    }

    void FixedUpdate()
    {
        if (skillCD.skill != SkillCD.Skill.TimeSkill)
        {
            if (!isCollision)
            {
                transform.Translate(transform.up * 80 * Time.fixedDeltaTime, Space.World);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isCollision = true;
        ani.SetBool("Hit", true);
        Despawn();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Door")
        {
            isCollision = true;
            ani.SetBool("Hit", true);
            Despawn();
        }
    }

    public override void Despawn()
    {
        Destroy(this.gameObject, 0.2f);
        //bulletSpawnPool.Despawn(transform, 0.2f);
    }
}
