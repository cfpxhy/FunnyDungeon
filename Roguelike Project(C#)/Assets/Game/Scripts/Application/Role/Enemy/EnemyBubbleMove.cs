using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBubbleMove : EnemyBase
{
    public GameObject bullet;

    public float attackTimer = 2.0f;

    private SkillCD skillCD;
    private SpriteRenderer sr;
    //public GameObject attackEffect;
    //是否在被攻击的CD
    private bool isAttacted = false;
    private float damageCD = 0.2f;
    private float timerCD = 0;
    private int damage;
    private float v = -1;
    private float h;

    private GameObject[] rooms;
    private RoomInstance room;
    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;
    private SpawnPool bulletSpawnPool;
    private float changeDirectionTimer;
    private float timer;

    private List<ItemInfo> items = new List<ItemInfo>();

    private Transform firePositionU;
    private Transform firePositionUL;
    private Transform firePositionL;
    private Transform firePositionDL;
    private Transform firePositionD;
    private Transform firePositionDR;
    private Transform firePositionR;
    private Transform firePositionUR;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];
        bulletSpawnPool = PoolManager.Pools["Bullet"];
        rooms = GameObject.FindGameObjectsWithTag("Room");

        items = Game.Instance.StaticData.itemList;

        foreach (GameObject r in rooms)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(r.transform.position.x, r.transform.position.y)) < 170)
            {
                room = r.GetComponent<RoomInstance>();
            }
        }

        firePositionU = transform.FindChild("FirePositionU").GetComponent<Transform>();
        firePositionUL = transform.FindChild("FirePositionUL").GetComponent<Transform>();
        firePositionL = transform.FindChild("FirePositionL").GetComponent<Transform>();
        firePositionDL = transform.FindChild("FirePositionDL").GetComponent<Transform>();
        firePositionD = transform.FindChild("FirePositionD").GetComponent<Transform>();
        firePositionDR = transform.FindChild("FirePositionDR").GetComponent<Transform>();
        firePositionR = transform.FindChild("FirePositionR").GetComponent<Transform>();
        firePositionUR = transform.FindChild("FirePositionUR").GetComponent<Transform>();

        timer = attackTimer;
    }

    void Update()
    {
        if (skillCD.skill != SkillCD.Skill.TimeSkill)
        {
            if (h == 0 && v == 0)
            {
                if (attackTimer > timer)
                {
                    Attack();
                }
                else
                {
                    attackTimer += Time.deltaTime;
                }
            }
        }

        if (isAttacted)
        {
            timerCD += Time.deltaTime;
            if (timerCD >= damageCD)
            {
                isAttacted = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (skillCD.skill != SkillCD.Skill.TimeSkill)
        {
            Move();
        }
    }

    public override void Move()
    {
        if (changeDirectionTimer >= 2)
        {
            //让怪物尽量往房间中间移动的AI
            //当怪物处在该房间右上半区时，往左往下移动的概率要更高一点
            if (transform.position.x >= room.transform.position.x && transform.position.y >= room.transform.position.y)
            {
                int num = Random.Range(0, 10);
                if (num == 0 || num == 1)
                {
                    v = -1;
                    h = 0;
                }
                else if (num == 2)
                {
                    v = 1;
                    h = 0;
                }
                else if (num == 3 || num == 4)
                {
                    h = -1;
                    v = 0;
                }
                else if (num == 5)
                {
                    h = 1;
                    v = 0;
                }
                else if (num >= 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            if (transform.position.x <= room.transform.position.x && transform.position.y <= room.transform.position.y)
            {
                int num = Random.Range(0, 10);
                if (num == 0)
                {
                    v = -1;
                    h = 0;
                }
                else if (num == 1 || num == 2)
                {
                    v = 1;
                    h = 0;
                }
                else if (num == 3)
                {
                    h = -1;
                    v = 0;
                }
                else if (num == 4 || num == 5)
                {
                    h = 1;
                    v = 0;
                }
                else if (num >= 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            if (transform.position.x < room.transform.position.x && transform.position.y > room.transform.position.y)
            {
                int num = Random.Range(0, 10);
                if (num == 0 || num == 1)
                {
                    v = -1;
                    h = 0;
                }
                else if (num == 2)
                {
                    v = 1;
                    h = 0;
                }
                else if (num == 3)
                {
                    h = -1;
                    v = 0;
                }
                else if (num == 4 || num == 5)
                {
                    h = 1;
                    v = 0;
                }
                else if (num >= 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            if (transform.position.x > room.transform.position.x && transform.position.y < room.transform.position.y)
            {
                int num = Random.Range(0, 10);
                if (num == 0)
                {
                    v = -1;
                    h = 0;
                }
                else if (num == 1 || num == 2)
                {
                    v = 1;
                    h = 0;
                }
                else if (num == 3 || num == 4)
                {
                    h = -1;
                    v = 0;
                }
                else if (num == 5)
                {
                    h = 1;
                    v = 0;
                }
                else if (num >= 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            changeDirectionTimer = 0;
        }
        else
        {
            changeDirectionTimer += Time.fixedDeltaTime;
        }
        transform.Translate(Vector3.up * v * Speed * Time.fixedDeltaTime, Space.World);
        transform.Translate(Vector3.right * h * Speed * Time.fixedDeltaTime, Space.World);
    }

    void Attack()
    {
        GenerateBullet(firePositionU, new Vector3(0, 0, 0));
        GenerateBullet(firePositionUL, new Vector3(0, 0, 45));
        GenerateBullet(firePositionL, new Vector3(0, 0, 90));
        GenerateBullet(firePositionDL, new Vector3(0, 0, 135));
        GenerateBullet(firePositionD, new Vector3(0, 0, -180));
        GenerateBullet(firePositionDR, new Vector3(0, 0, -135));
        GenerateBullet(firePositionR, new Vector3(0, 0, -90));
        GenerateBullet(firePositionUR, new Vector3(0, 0, -45));

        attackTimer = 0;
    }

    void GenerateBullet(Transform t, Vector3 rorate)
    {
        Instantiate(bullet, t.position, Quaternion.Euler(transform.eulerAngles + rorate));
        //bulletSpawnPool.Spawn(bullet, t.position, Quaternion.Euler(transform.eulerAngles + rorate));
    }

    public override void Damage(int hit)
    {
        hit = damage;
        base.Damage(hit);
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(Hurt());
        }
    }

    IEnumerator Hurt()
    {
        sr.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(HurtTimer);
        sr.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Door")
        {
            v = -v;
            h = -h;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag != "Player")
        {
            changeDirectionTimer = 2;
        }
        if (col.gameObject.tag == "PlayerBullet")
        {
            if (!isAttacted)
            {
                damage = col.gameObject.GetComponent<PlayerBullet>().BulletDamage;
                Damage(damage);
                isAttacted = true;
            }
        }
    }

    public override void Despawn()
    {
        if (gameObject.activeInHierarchy)
        {
            effectSpawnPool.Spawn(DeadEffect, new Vector2(transform.position.x, transform.position.y - 10.4f), Quaternion.identity);


            int ran = Random.Range(0, items.Count * 5);
            if (ran >= 0 && ran < items.Count)
            {
                Instantiate(items[ran].Prefab, transform.position, Quaternion.identity);
            }
            GameObject.Find("Player").GetComponent<Player>().money += 15;
            //入对象池之前恢复之前的血量
            Hp = MaxHp;
            enemySpawnPool.Despawn(transform);
        }
    }

    public void DespawnWithoutItem()
    {
        if (gameObject.activeInHierarchy)
        {
            effectSpawnPool.Spawn(DeadEffect, new Vector2(transform.position.x, transform.position.y - 10.4f), Quaternion.identity);
            //入对象池之前恢复之前的血量
            Hp = MaxHp;
            enemySpawnPool.Despawn(transform);
        }
    }
}
