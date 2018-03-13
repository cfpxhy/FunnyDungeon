using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class PlayerBullet : BulletBase {

    //起始sprite
    //public Sprite firstSprite;

    private Animator ani;
    private SpawnPool bulletSpawnPool;
    private SpriteRenderer sr;
    private Collider2D col2D;

    private bool isCollision = false;
    private bool isReverse = false;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        //firstSprite = sr.sprite;
        ani = GetComponent<Animator>();
        col2D = GetComponent<Collider2D>();
        bulletSpawnPool = PoolManager.Pools["Bullet"];
    }

    void FixedUpdate()
    {
        if (!isCollision)
        {
            transform.Translate(transform.up * BulletSpeed * Time.fixedDeltaTime, Space.World);
        }
        else
        {
            if (isReverse)
            {
                transform.Translate(-transform.up * 100 * Time.fixedDeltaTime, Space.World);
            }
        }
    }

	void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy" && col.transform.childCount == 1)
        {
            if (!isReverse)
            {
                isCollision = true;
                tag = "Bullet";
                sr.color = new Color(255, 237, 0);
                isReverse = true;
            }
            else
            {
                isCollision = true;
                col2D.isTrigger = true;
                ani.SetBool("Hit", true);
                Destroy(this.gameObject, 0.2f);
            }
        }
        else
        {
            //Despawn();
            isCollision = true;
            col2D.isTrigger = true;
            ani.SetBool("Hit", true);
            Destroy(this.gameObject, 0.2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Door")
        {
            isCollision = true;
            col2D.isTrigger = true;
            ani.SetBool("Hit", true);
            Destroy(this.gameObject, 0.2f);
        }
    }

    //public override void Despawn()
    //{
    //    if (gameObject.activeInHierarchy)
    //    {
    //        StartCoroutine(BulletAnimation());
    //        bulletSpawnPool.Despawn(transform, 1f);
    //    }
    //}

    //IEnumerator BulletAnimation()
    //{
    //    //ani.SetBool("Hit", true);
    //    ani.Play("BubbleExplosion");
    //    yield return new WaitForSeconds(0.4f);
    //    //ani.Stop("")
    //    //ani.Play("Hit");
    //    ani.Update(0);
    //    //ani.SetBool("Hit", false);
    //    //sr.sprite = firstSprite;
    //    //Debug.Log(firstSprite);
    //}
}
