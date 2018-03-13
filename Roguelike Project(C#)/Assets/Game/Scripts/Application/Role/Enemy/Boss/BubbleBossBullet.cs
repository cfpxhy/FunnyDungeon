using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class BubbleBossBullet : MonoBehaviour {

    Transform bullet;

    SpawnPool bulletSpawnPool;

    private int rotateSpeed = 180;
    private int moveSpeed = 40;
    void Start () {
        bullet = transform.FindChild("bullet");
        bulletSpawnPool = PoolManager.Pools["Bullet"];
	}
	
	// Update is called once per frame
	void Update () {
        //逆时针自转
        bullet.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime, Space.World);

        transform.Translate(transform.up * moveSpeed * Time.fixedDeltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Wall" || col.tag == "Player" || col.tag == "Door")
        {
            Despawn();
        }
    }

    public void Despawn()
    {
        bulletSpawnPool.Despawn(transform);
    }
}
