using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBubbleGrievance : EnemyBase
{
    private SkillCD skillCD;
    private SpriteRenderer sr;
    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;
    public Sprite[] sprite;
    private float v = -1;
    private float h;
    private int damage;
    //是否在被攻击的CD
    private bool isAttacted = false;
    private float damageCD = 0.2f;
    private float timer = 0;

    private bool choseItemLight = false;

    private float changeDirectionTimer;

    private GameObject[] itemLight = new GameObject[20];
    private List<ItemInfo> items = new List<ItemInfo>();

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];
        
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
        if (changeDirectionTimer >= 1)
        {
            int num = Random.Range(0, 4);
            if (num == 0)
            {
                v = -1;
                h = 0;
            }
            else if (num == 1)
            {
                v = 1;
                h = 0;
            }
            else if (num == 2)
            {
                h = -1;
                v = 0;
            }
            else if (num == 3)
            {
                h = 1;
                v = 0;
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
        if(col.gameObject.tag != "Player")
        {
            changeDirectionTimer = 1;
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
    //爆道具的移除
    public override void Despawn()
    {
        if(gameObject.activeInHierarchy)
        {
            if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level3)
            {
                effectSpawnPool.Spawn(DeadEffect, new Vector2(transform.position.x, transform.position.y - 10.4f), Quaternion.identity);
            }

            //爆道具
            int ran = Random.Range(0, items.Count * 5);
            //int ran = Random.Range(0, items.Count);
            if (ran >= 0 && ran < items.Count)
            {
                GameObject item = Instantiate(items[ran].Prefab, transform.position, Quaternion.identity);
                //GameObject item = Instantiate(items[5].Prefab, transform.position, Quaternion.identity);
                if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
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
            //int ran = Random.Range(0, items.Count);
            //int ran = Random.Range(items.Count - 2, items.Count);
            //GameObject item = Instantiate(items[0].Prefab, transform.position, Quaternion.identity);
            GameObject.Find("Player").GetComponent<Player>().money += 10;
            //入对象池之前恢复之前的血量
            Hp = MaxHp;
            choseItemLight = false;
            enemySpawnPool.Despawn(transform);
            //Instantiate(items[4].Prefab, transform.position, Quaternion.identity);
            //items[ran].Prefab.GetComponent<Animator>().SetBool("Move", true);
        }
    }
    //不爆道具的移除
    public void DespawnWithoutItem()
    {
        if (gameObject.activeInHierarchy)
        {
            if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level3)
            {
                effectSpawnPool.Spawn(DeadEffect, new Vector2(transform.position.x, transform.position.y - 10.4f), Quaternion.identity);
            }
            //入对象池之前恢复之前的血量
            Hp = MaxHp;
            enemySpawnPool.Despawn(transform);
        }
    }
}
