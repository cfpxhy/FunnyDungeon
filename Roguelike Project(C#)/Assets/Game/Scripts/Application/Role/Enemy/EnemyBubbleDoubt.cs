using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBubbleDoubt : EnemyBase
{
    public Sprite[] sprite;
    private SkillCD skillCD;
    private SpriteRenderer sr;
    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;

    private Transform leftEdge;
    private Transform rightEdge;

    private float v = -1;
    private float h;

    private float damageCD = 0.2f;
    private float timer = 0;

    private int damage;
    private bool isAttacted = false;

    private bool choseItemLight = false;

    private GameObject[] rooms;
    private RoomInstance room;
    private float changeDirectionTimer;

    private List<ItemInfo> items = new List<ItemInfo>();
    private GameObject[] itemLight = new GameObject[20];

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];

        leftEdge = transform.FindChild("LeftEdge");
        rightEdge = transform.FindChild("RightEdge");

        rooms = GameObject.FindGameObjectsWithTag("Room");

        foreach (GameObject r in rooms)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(r.transform.position.x, r.transform.position.y)) < 170)
            {
                room = r.GetComponent<RoomInstance>();
            }
        }

        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            for (int i = 0; i < 20; i++)
            {
                itemLight[i] = GameObject.Find("Lights").transform.GetChild(i).transform.GetChild(0).gameObject;
            }
            items = Game.Instance.StaticData.shineItemList;
        }
        else
        {
            items = Game.Instance.StaticData.itemList;
        }
    }

    void Update()
    {
        if (isAttacted)
        {
            timer += Time.deltaTime;
            if (timer >= damageCD)
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
                int num = Random.Range(0, 7);
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
                else if (num == 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            if (transform.position.x <= room.transform.position.x && transform.position.y <= room.transform.position.y)
            {
                int num = Random.Range(0, 7);
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
                else if (num == 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            if (transform.position.x < room.transform.position.x && transform.position.y > room.transform.position.y)
            {
                int num = Random.Range(0, 7);
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
                else if (num == 6)
                {
                    h = 0;
                    v = 0;
                }
            }
            if (transform.position.x > room.transform.position.x && transform.position.y < room.transform.position.y)
            {
                int num = Random.Range(0, 7);
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
                else if (num == 6)
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

        if (v < 0)
        {
            sr.sprite = sprite[0];
        }

        else if (v > 0)
        {
            sr.sprite = sprite[1];
        }

        if (v != 0)
        {
            return;
        }

        transform.Translate(Vector3.right * h * Speed * Time.fixedDeltaTime, Space.World);

        if (h < 0)
        {
            sr.sprite = sprite[0];
        }

        else if (h > 0)
        {
            sr.sprite = sprite[1];
        }
    }

    public override void Damage(int hit)
    {
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
            if (v != 0 || h != 0)
            {
                if (!isAttacted)
                {
                    damage = col.gameObject.GetComponent<PlayerBullet>().BulletDamage;
                    Damage(damage);
                    isAttacted = true;
                }
            }
        }
    }

    public override void Despawn()
    {
        if (gameObject.activeInHierarchy)
        {
            if (SceneManager.GetActiveScene().name != "5.Level3")
            {
                effectSpawnPool.Spawn(DeadEffect, new Vector2(transform.position.x, transform.position.y - 10.4f), Quaternion.identity);
            }

            int ran = Random.Range(0, items.Count * 5);
            if (ran >= 0 && ran < items.Count)
            {
                GameObject item = Instantiate(items[ran].Prefab, transform.position, Quaternion.identity);
                if (SceneManager.GetActiveScene().name == "5.Level3")
                {
                    //itemLight位置处理
                    foreach (GameObject go in itemLight)
                    {
                        if (go.activeInHierarchy == false)
                        {
                            if (!choseItemLight)
                            {
                                go.SetActive(true);
                                go.transform.position = transform.position;
                                go.transform.parent.SetParent(item.transform.GetChild(0));

                                choseItemLight = true;
                            }
                        }
                    }
                }
            }
            GameObject.Find("Player").GetComponent<Player>().money += 10;
            //入对象池之前恢复之前的血量
            Hp = MaxHp;
            choseItemLight = false;
            enemySpawnPool.Despawn(transform);
        }
    }

    public void DespawnWithoutItem()
    {
        if (gameObject.activeInHierarchy)
        {
            if (SceneManager.GetActiveScene().name != "5.Level3")
            {
                effectSpawnPool.Spawn(DeadEffect, new Vector2(transform.position.x, transform.position.y - 10.4f), Quaternion.identity);
            }
            //入对象池之前恢复之前的血量
            Hp = MaxHp;
            enemySpawnPool.Despawn(transform);
        }
    }
}
