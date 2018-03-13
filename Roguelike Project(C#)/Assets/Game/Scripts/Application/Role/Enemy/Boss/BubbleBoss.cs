using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using UnityEngine.UI;
using DG.Tweening;

public class BubbleBoss : MonoBehaviour {

    #region 变量
    public GameObject generationEffect;
    public GameObject markEffect;
    public GameObject thunderEffect;
    public GameObject bullet;

    private GameObject[] rooms;
    //本房间下的子物体
    private List<Transform> startPos = new List<Transform>();

    private GameObject resultPanel;
    private GameObject successText;

    private Player p;

    private Transform sunglasses;
    private Transform mouse;
    private Transform leftHand;
    private Transform rightHand;

    private Text moneyText;

    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;
    private SpawnPool bulletSpawnPool;

    private int hp;
    private int maxHp;

    private SpriteRenderer sr;

    private Vector2 point;

    private bool sunglasseAnimation = false;

    private bool isSummon = false;
    private bool isThunder = false;
    private bool isFire = false;
    //上一次施放的技能,默认为Default
    private SkillCast skillCast;
    private enum SkillCast
    {
        Default,
        Summon,
        Thunder,
        Fire
    }

    private int damage;

    private int money;
    private int roundCount = 0;
    //BOSS每施放闪电技能会连续放几次
    private int thunderRoundCount = 1;
    private int summonRoundCount = 2;
    private float offestX = 142;
    private float offestY = 60;
    private float bossSize = 64;
    private float effectSize = 40;
    private float summonEnemyInterval = 24;
    private float firePositionRadius = 24;
    private int firePositions = 36;
    private List<Vector2> markPosition = new List<Vector2>();
    private List<Vector2> generatePositionLeft = new List<Vector2>();
    private List<Vector2> generatePositionRight = new List<Vector2>();
    private List<Vector2> firePosition = new List<Vector2>();
    private List<int> fireAngle = new List<int>();
    #endregion

    #region 字段
    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value;}
    }
    #endregion

    void Start()
    {
        Hp = 3000;
        MaxHp = Hp;
        p = GameObject.Find("Player").GetComponent<Player>();
        //money = GameObject.Find("Player").GetComponent<Player>().money;
        //moneyText = GameObject.Find("Player").GetComponent<Player>().moneyText;
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];
        bulletSpawnPool = PoolManager.Pools["Bullet"];

        resultPanel = GameObject.Find("Canvas").transform.Find("ResultPanel").gameObject;
        successText = resultPanel.transform.Find("SuccessText").gameObject;

        rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject go in rooms)
        {
            if (go.transform.position == Vector3.zero)
            {
                //获得当前房间下的子物体
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    startPos.Add(go.transform.GetChild(i));
                }
                foreach (Transform t in startPos)
                {
                    //本房间的所有门
                    if (t.gameObject.tag == "Door")
                    {
                        //锁上门
                        t.GetComponent<Collider2D>().isTrigger = false;
                    }
                }
            }
        }

        sr = GetComponent<SpriteRenderer>();

        sunglasses = transform.FindChild("sunglasses");
        mouse = transform.FindChild("mouse");
        leftHand = transform.FindChild("lefthand");
        rightHand = transform.FindChild("righthand");

        skillCast = SkillCast.Default;

        //Invoke("PlayAudio", 1.0f);
    }

    void Update()
    {
        if (Hp == MaxHp)
        {
            return;
        }
        else if (Hp > MaxHp * 0.4f && Hp < MaxHp)
        {
            if (!sunglasseAnimation)
            {
                sunglasses.DOLocalMoveY(0, 5.0f);
                sunglasseAnimation = true;
                Game.Instance.Sound.PlayBg("BossBg");
            }
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                if (!isSummon)
                {
                    //随机召唤技能的施放方向
                    int randomDirection = Random.Range(0, 2);
                    if (skillCast == SkillCast.Thunder)
                    {
                        if (randomDirection == 0)
                        {
                            StartCoroutine(SummonSkillLeft());
                            //如果第一波是从左边生成的，那么第二波就从右边，反之亦然
                            randomDirection = 1;
                        }
                        else
                        {
                            StartCoroutine(SummonSkillRight());
                            randomDirection = 0;
                        }
                        isSummon = true;
                        roundCount++;
                        if (roundCount >= summonRoundCount)
                        {
                            skillCast = SkillCast.Summon;
                            roundCount = 0;
                        }
                    }
                }
            }
            else
            {
                if (!isThunder)
                {
                    if (skillCast == SkillCast.Default || skillCast == SkillCast.Summon)
                    {
                        if (roundCount <= thunderRoundCount)
                        {
                            StartCoroutine(ThunderSkill());
                            isThunder = true;
                            roundCount++;
                        }
                        else
                        {
                            skillCast = SkillCast.Thunder;
                            roundCount = 0;
                        }
                    }
                }
            }
        }
        else if (Hp <= MaxHp * 0.4f && Hp > 0)
        {
            if (sunglasseAnimation)
            {
                sunglasses.GetComponent<SpriteRenderer>().DOFade(0, 3.0f);
                sunglasseAnimation = false;
                mouse.gameObject.SetActive(false);

                leftHand.GetComponent<SpriteRenderer>().DOFade(1, 3.0f);
                rightHand.GetComponent<SpriteRenderer>().DOFade(1, 3.0f);
            }
            if (!isFire)
            {
                if (skillCast == SkillCast.Summon || skillCast == SkillCast.Thunder)
                {
                    StartCoroutine(FireSkill());
                }
            }
            else
            {

            }
        }
    }

    #region 技能
    IEnumerator ThunderSkill()
    {
        CalculationMarkPosition();
        List<int> ranList = new List<int>();

        int ran;

        foreach (Vector2 pos in markPosition)
        {
            //有效格子生成特效的概率
            ran = Random.Range(0, 3);
            if (ran == 0)
            {
                effectSpawnPool.Spawn(markEffect, pos, Quaternion.identity);
            }
            ranList.Add(ran);
        }
        yield return new WaitForSeconds(1.0f);
        int i = 0;
        foreach (Vector2 pos in markPosition)
        {
            if (i < ranList.Count)
            {
                if (ranList[i] == 0)
                {
                    effectSpawnPool.Spawn(thunderEffect, pos, Quaternion.identity);
                }
            }
            i++;
        }
        ranList.Clear();
        //markPosition.Clear();
        yield return new WaitForSeconds(3.0f);

        isThunder = false;
    }

    void CalculationMarkPosition()
    {
        markPosition.Clear();

        for (int i = 0; i < 8; i++)
        {
            //第一列
            if (i == 0)
            {
                //添加左上角的点
                markPosition.Add(point);
                for (int j = 1; j < 4; j++)
                {
                    point = new Vector2(-offestX, offestY - effectSize * j);
                    markPosition.Add(point);
                }
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    point = new Vector2(-offestX + i * effectSize, offestY - j * effectSize);
                    float x = point.x;
                    float y = point.y;
                    //Debug.Log("X:" + x + "Y:" + y);
                    //判断排除掉BOSS所处的位置
                    if (x < -bossSize / 2 || x > bossSize / 2)
                    {
                        markPosition.Add(point);
                    }
                    else
                    {
                        if (y > bossSize / 2 || y < -bossSize / 2)
                        {
                            markPosition.Add(point);
                        }
                    }
                }
            }
        }
    }

    IEnumerator SummonSkillLeft()
    {
        CalculationGeneratePosition();
        foreach (Vector2 pos in generatePositionLeft)
        {
            effectSpawnPool.Spawn(generationEffect, pos, Quaternion.identity);
            yield return new WaitForSeconds(1.5f);
            GameObject summonEnemy = Resources.Load("Game/Prefabs/Enemy/Boss/BossEnemy") as GameObject;
            Transform enemy = enemySpawnPool.Spawn(summonEnemy, pos, Quaternion.identity);
            BubbleBossEnemy bubbleBossEnemy = enemy.GetComponent<BubbleBossEnemy>();
            bubbleBossEnemy.startPos = 0;
        }
        yield return new WaitForSeconds(7.0f);
        isSummon = false;
    }

    IEnumerator SummonSkillRight()
    {
        CalculationGeneratePosition();
        foreach (Vector2 pos in generatePositionRight)
        {
            effectSpawnPool.Spawn(generationEffect, pos, Quaternion.identity);
            yield return new WaitForSeconds(1.5f);
            GameObject summonEnemy = Resources.Load("Game/Prefabs/Enemy/Boss/BossEnemy") as GameObject;
            Transform enemy = enemySpawnPool.Spawn(summonEnemy, pos, Quaternion.identity);
            BubbleBossEnemy bubbleBossEnemy = enemy.GetComponent<BubbleBossEnemy>();
            bubbleBossEnemy.startPos = 1;
        }
        yield return new WaitForSeconds(7.0f);
        isSummon = false;
    }

    void CalculationGeneratePosition()
    {
        generatePositionLeft.Clear();
        generatePositionRight.Clear();
        //最靠左上的点
        point = new Vector2(-offestX, offestY);

        generatePositionLeft.Add(point);
        for (int i = 1;i < 6;i++)
        {
            point = new Vector2(-offestX, offestY - summonEnemyInterval * i);
            generatePositionLeft.Add(point);
        }
        //生成相对应的右边的生成点集合
        foreach (Vector2 pos in generatePositionLeft)
        {
            Vector2 newPos;
            newPos = new Vector2(-pos.x, pos.y);
            generatePositionRight.Add(newPos);
        }
    }

    IEnumerator FireSkill()
    {
        CalculationFirePosition();
        List<int> bulletPosIndex = new List<int>();
        for (int i = 0;i < 3000;i++)
        {
            int ran = Random.Range(0, firePosition.Count);
            bulletPosIndex.Add(ran);
        }

        foreach (int i in bulletPosIndex)
        {
            bulletSpawnPool.Spawn(bullet, firePosition[i], Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, fireAngle[i])));
            isFire = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void CalculationFirePosition()
    {
        for (int i = 0;i < firePositions; i++)
        {
            int angle = i * (360 / firePositions);
            if (angle > 0 && angle < 180)
            {
                if (angle <= 90)
                {
                    //第一象限(含正右)
                    float x = firePositionRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    float y = firePositionRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    fireAngle.Add(-angle);
                    firePosition.Add(new Vector2(x, y));
                }
                if (angle >= 90)
                {
                    //第三象限(含正左)
                    float x = -firePositionRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    float y = firePositionRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    fireAngle.Add(angle);
                    firePosition.Add(new Vector2(x, y));
                }
            }
            else if(angle == 180)
            {
                //正下
                float x = 0;
                float y = -firePositionRadius;
                fireAngle.Add(angle);
                firePosition.Add(new Vector2(x, y));
            }
            else if(angle == 0)
            {
                //正上
                float x = 0;
                float y = firePositionRadius;
                fireAngle.Add(angle);
                firePosition.Add(new Vector2(x, y));
            }
            else
            {
                if (angle > 270)
                {
                    //第二象限
                    float x = firePositionRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    float y = firePositionRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    fireAngle.Add(-angle);
                    firePosition.Add(new Vector2(x, y));
                }
                if (angle < 270)
                {
                    //第四象限
                    float x = -firePositionRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    float y = firePositionRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    fireAngle.Add(angle);
                    firePosition.Add(new Vector2(x, y));
                }
            }
        }
    }
    #endregion

    void PlayAudio()
    {
        Game.Instance.Sound.PlayEffect("Boss");
    }

    public void Damage(int hit)
    {
        hit = damage;
        if (Hp > 0)
        {
            Hp -= hit;
        }
        else
        {
            p.Success();
            //Game.Instance.Sound.PlayEffect("Success");
            //resultPanel.SetActive(true);
            //successText.SetActive(true);
            ////加金币
            //money += 500;
            //moneyText.text = money.ToString();
            //Tween tween = resultPanel.transform.DOScale(1, 2.0f);
            //tween.SetEase(Ease.OutBack);
            //BOSS死亡
            Destroy(gameObject);
        }
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(Hurt());
        }
    }

    IEnumerator Hurt()
    {
        sr.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "PlayerBullet")
        {
            damage = col.gameObject.GetComponent<PlayerBullet>().BulletDamage;
            Damage(damage);
        }
    }
}
