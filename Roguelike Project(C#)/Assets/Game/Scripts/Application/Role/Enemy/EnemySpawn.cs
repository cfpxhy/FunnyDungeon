using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemySpawn : MonoBehaviour
{

    public GameObject generateEffect;
    private Transform player;

    private SpawnPool enemySpawnPool;
    private SpawnPool effectSpawnPool;

    private Text promptText;
    private RoomInstance room;

    private Vector2[] spawn = new Vector2[4];

    private int offestX = 142;
    private int offestY = 60;
    private float switchRoomTime = 1.0f;
    private float generateEnemyTime = 0.3f;
    private int index;
    //所有怪物集合
    private List<EnemyInfo> enemyInfo = new List<EnemyInfo>();
    //用来存储生成的怪物类型数量数据(如果这房间没打完就走了，要用来还原同样的怪物)
    private List<EnemyInfo> enemySave = new List<EnemyInfo>();
    //记录该房间生成的怪物种类和其数量
    //private Dictionary<int, GameObject> enemyReset = new Dictionary<int, GameObject>();
    [HideInInspector]
    public List<int> enemyID = new List<int>();
    private List<GameObject> enemyPrefab = new List<GameObject>();

    [HideInInspector]
    public bool genreateEnemy = false;
    [HideInInspector]
    public GameObject[] roomEnemyArray;
    private GameObject[] thisRoomBullet;
    private List<GameObject> roomEnemyList = new List<GameObject>();

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        enemySpawnPool = PoolManager.Pools["Enemy"];
        effectSpawnPool = PoolManager.Pools["Effect"];

        room = GetComponent<RoomInstance>();

        spawn[0] = new Vector2(transform.position.x - offestX, transform.position.y + offestY);
        spawn[1] = new Vector2(transform.position.x - offestX, transform.position.y - offestY);
        spawn[2] = new Vector2(transform.position.x + offestX, transform.position.y + offestY);
        spawn[3] = new Vector2(transform.position.x + offestX, transform.position.y - offestY);

        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            enemyInfo = Game.Instance.StaticData.shellEnemyList;
            promptText = GameObject.Find("Canvas").transform.Find("PromptText").GetComponent<Text>();
        }
        else
        {
            enemyInfo = Game.Instance.StaticData.enemyList;
        }
        index = 0;
    }

    private void Update()
    {
        //排除出生房间
        if (room.type == 1)
        {
            return;
        }
        else
        {
            //当角色进入该房间
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < 170)
            {
                if (room.isClean)
                {
                    
                }
                //当该房间还未生成过怪物(或者生成过怪物但是没有清光)时
                if (!genreateEnemy)
                {
                    if (room.isClean == false)
                    {
                        StartCoroutine(GenerateEnemy());
                    }
                    else
                    {
                        //promptText.text = "本房间怪物已清空";
                    }
                    genreateEnemy = true;
                }
                else
                {

                }
            }
        }
    }

    //生成怪物
    public IEnumerator GenerateEnemy()
    {
        //如果该房间的怪物还没被打光
        if (room.isClean == false)
        {
            if (room.firstEnter == false)
            {
                yield return new WaitForSeconds(switchRoomTime);
                if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level3)
                {
                    foreach (Vector2 pos in spawn)
                    {
                        effectSpawnPool.Spawn(generateEffect, pos, Quaternion.identity);
                    }
                }
                yield return new WaitForSeconds(generateEnemyTime);
                index = 0;
                foreach (Vector2 pos in spawn)
                {
                    //在相同生成点还原上一次进这个房间刷的那批怪物
                    enemySpawnPool.Spawn(enemySave[index].Prefab, pos, Quaternion.identity);
                    index++;
                }
            }
            else
            {
                yield return new WaitForSeconds(switchRoomTime);
                if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level3)
                {
                    foreach (Vector2 pos in spawn)
                    {
                        effectSpawnPool.Spawn(generateEffect, pos, Quaternion.identity);
                    }
                }
                yield return new WaitForSeconds(generateEnemyTime);
                foreach (Vector2 pos in spawn)
                {
                    int ran;
                    if (SceneManager.GetActiveScene().name == "3.Level1")
                    {
                        ran = Random.Range(1, enemyInfo.Count - 1);
                        enemySpawnPool.Spawn(enemyInfo[ran - 1].Prefab, pos, Quaternion.identity);
                        enemySave.Add(enemyInfo[ran - 1]);
                    }
                    else if (SceneManager.GetActiveScene().name == "4.Level2")
                    {
                        ran = Random.Range(1, enemyInfo.Count + 1);
                        enemySpawnPool.Spawn(enemyInfo[ran - 1].Prefab, pos, Quaternion.identity);
                        enemySave.Add(enemyInfo[ran - 1]);
                    }
                    else
                    {
                        ran = Random.Range(1, enemyInfo.Count);
                        enemySpawnPool.Spawn(enemyInfo[ran - 1].Prefab, pos, Quaternion.identity);
                        enemySave.Add(enemyInfo[ran - 1]);
                        //enemySpawnPool.Spawn(enemyInfo[2].Prefab, pos, Quaternion.identity);
                        //enemySave.Add(enemyInfo[2]);
                    }
                    //Debug.Log(ran + "---" + enemyInfo[ran - 1].ID + "---" + enemyInfo[ran - 1].Prefab);
                    //enemySpawnPool.Spawn(enemyInfo[ran - 1].Prefab, pos, Quaternion.identity);
                    //enemySpawnPool.Spawn(enemyInfo[0].Prefab, pos, Quaternion.identity);
                    //Instantiate(enemyInfo[3].Prefab, pos, Quaternion.identity);
                    //用enemySave记录该房间生成的怪物种类位置等信息
                    //enemySave.Add(enemyInfo[ran - 1]);
                }
                room.firstEnter = false;
            }
        }
    }

    public void RemoveEnemy()
    {
        if (room.isClean == false)
        {
            roomEnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject go in roomEnemyArray)
            {
                roomEnemyList.Add(go);
            }
            //本房间怪没打完
            if (roomEnemyList.Count != 0)
            {
                room.isClean = false;
                foreach (GameObject go in roomEnemyList)
                {
                    go.SendMessage("DespawnWithoutItem");
                }
            }
            else
            {
                room.isClean = true;
                //清光一个房间后这个房间的缓存集合
                enemySave.Clear();
            }
            roomEnemyList.Clear();
        }
    }

    public void RemoveBullet()
    {
        thisRoomBullet = GameObject.FindGameObjectsWithTag("Bullet");
        if (thisRoomBullet.Length != 0)
        {
            foreach (GameObject go in thisRoomBullet)
            {
                go.SendMessage("Despawn");
            }
        }
        else
        {
            return;
        }
    }
}
