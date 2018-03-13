using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class BubbleBossEnemy : MonoBehaviour {

    public Sprite[] sprite;
    public GameObject deadEffect;

    private int speed = 50;
    private SpriteRenderer sr;
    private bool turnDirection = false;

    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;
    //0从左边生成1从右边生成
    [HideInInspector]
    public int startPos = 0;

    private float offestX = 142;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];
    }

    void Update()
    {
        if (startPos == 0)
        {
            if (transform.position.x >= offestX)
            {
                turnDirection = true;
            }
            if (!turnDirection)
            {
                RightMove();
            }
            else
            {
                LeftMove();
            }
            if (turnDirection && transform.position.x <= -offestX)
            {
                turnDirection = false;
                Despawn();
            }
        }
        else
        {
            if (transform.position.x <= -offestX)
            {
                turnDirection = true;
            }
            if (!turnDirection)
            {
                LeftMove();
            }
            else
            {
                RightMove();
            }
            if (turnDirection && transform.position.x >= offestX)
            {
                turnDirection = false;
                Despawn();
            }
        }
    }

    void RightMove()
    {
        sr.sprite = sprite[1];
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    void LeftMove()
    {
        sr.sprite = sprite[0];
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
    }

    void Despawn()
    {
        effectSpawnPool.Spawn(deadEffect, transform.position, Quaternion.identity);
        enemySpawnPool.Despawn(transform);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            effectSpawnPool.Spawn(deadEffect, transform.position, Quaternion.identity);
            enemySpawnPool.Despawn(transform);
        }
    }
}
