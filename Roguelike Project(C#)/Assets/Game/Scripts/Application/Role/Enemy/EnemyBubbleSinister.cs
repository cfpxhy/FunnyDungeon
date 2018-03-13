using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class EnemyBubbleSinister : EnemyBase
{
    private SpriteRenderer sr;
    private SkillCD skillCD;
    public Sprite[] sprite;
    public GameObject attackEffect;
    private float v = -1;
    private float h;

    private GameObject[] rooms;
    private RoomInstance room;
    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;

    private int damage;
    //是否在被攻击的CD
    private bool isAttacted = false;
    private float damageCD = 0.2f;
    private float timer = 0;
    private float maxX, minX, maxY, minY;
    private float offestX = 148;
    private float offestY = 69;
    private float size = 20.8f;

    private float attackTimer;
    private float changeDirectionTimer;
    private List<Vector2> firePosition = new List<Vector2>();
    private List<ItemInfo> items = new List<ItemInfo>();

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];
        rooms = GameObject.FindGameObjectsWithTag("Room");

        items = Game.Instance.StaticData.itemList;
        //得到该怪物所在的房间
        foreach (GameObject r in rooms)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(r.transform.position.x, r.transform.position.y)) < 170)
            {
                room = r.GetComponent<RoomInstance>();
            }
        }
        maxX = room.transform.position.x + offestX;
        minX = room.transform.position.x - offestX;
        maxY = room.transform.position.y + offestY;
        minY = room.transform.position.y - offestY;
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
        if (changeDirectionTimer >= 3)
        {
            int num = Random.Range(0, 5);
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
            else if (num >= 4 && num <= 5)
            {
                h = 0;
                v = 0;
                //原地发呆时有几率攻击
                int ran = Random.Range(0, 1);
                {
                    if (ran == 0)
                    {
                        StartCoroutine(Attack());                      
                    }
                }
            }
            changeDirectionTimer = 0;
        }
        else
        {
            //attackTimer += Time.fixedDeltaTime;
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

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(2.0f);
        CalculationFirePosition();
        foreach (Vector2 pos in firePosition)
        {
            //当没有超出当前房间的边界
            if (pos.x <= maxX && pos.x >= minX && pos.y <= maxY && pos.y >= minY)
            {
                effectSpawnPool.Spawn(attackEffect, pos, Quaternion.identity);
            }
        }
        //移除已经用来生成过特效的坐标集合元素
        firePosition.Clear();
        //attackTimer = 0;
    }

    //计算着火点位置
    private void CalculationFirePosition()
    {
        //检测垂直上方是否有障碍物
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.up, 300, LayerMask.GetMask("Obstacle"));
        {
            //如果有
            if (hit1.collider != null)
            {
                Vector2 colPos = hit1.collider.transform.position;
                for (int i = 1; i < 7; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x, transform.position.y + (size * i));
                    //障碍物position不能生成attackEffect，所以要-size，生成到障碍物前一个size的距离
                    if(pos.y < colPos.y - size)
                    {
                        firePosition.Add(pos);
                    }
                }
            }
            else
            {
                for (int i = 1; i < 7; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x, transform.position.y + (size * i));
                    firePosition.Add(pos);
                }
            }
        }
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Vector2.left, 300, LayerMask.GetMask("Obstacle"));
        {
            if (hit2.collider != null)
            {
                Vector2 colPos = hit2.collider.transform.position;
                for (int i = 1; i < 14; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x - (size * i), transform.position.y);
                    if (pos.x > colPos.x + size)
                    {
                        firePosition.Add(pos);
                    }
                }
            }
            else
            {
                for (int i = 1; i < 14; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x - (size * i), transform.position.y);
                    firePosition.Add(pos);
                }
            }
        }
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position, Vector2.down, 300,LayerMask.GetMask("Obstacle"));
        {
            if (hit3.collider != null)
            {
                Vector2 colPos = hit3.collider.transform.position;
                for (int i = 1; i < 7; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x, transform.position.y - (size * i));
                    if (pos.y > colPos.y + size)
                    {
                        firePosition.Add(pos);
                    }
                }
            }
            else
            {
                for (int i = 1; i < 7; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x, transform.position.y - (size * i));
                    firePosition.Add(pos);
                }
            }
        }
        RaycastHit2D hit4 = Physics2D.Raycast(transform.position, Vector2.right, 300, LayerMask.GetMask("Obstacle"));
        {
            if (hit4.collider != null)
            {
                Vector2 colPos = hit4.collider.transform.position;
                for (int i = 1; i < 14; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x + (size * i), transform.position.y);
                    if (pos.x < colPos.x - size)
                    {
                        firePosition.Add(pos);
                    }
                }
            }
            else
            {
                for (int i = 1; i < 14; i++)
                {
                    Vector2 pos = new Vector2(transform.position.x + (size * i), transform.position.y);
                    firePosition.Add(pos);
                }
            }
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
            GameObject.Find("Player").GetComponent<Player>().money += 20;
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
