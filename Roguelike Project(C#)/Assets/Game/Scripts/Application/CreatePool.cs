using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using UnityEngine.SceneManagement;

public class CreatePool : MonoBehaviour {

    private List<EnemyInfo> enemyInfo = new List<EnemyInfo>();
    private List<EnemyInfo> shellEnemyInfo = new List<EnemyInfo>();
    private List<EnemyInfo> bossEnemyInfo = new List<EnemyInfo>();
    private string enemyPool = "Enemy";
    private SpawnPool enemySpawnPool;

    private List<EffectInfo> enemyEffectInfo = new List<EffectInfo>();
    private List<EffectInfo> bossEffectInfo = new List<EffectInfo>();
    private EffectInfo bonfireEffectInfo;
    private string effectPool = "Effect";
    private SpawnPool effectSpawnPool;

    private List<BulletInfo> bossBulletInfo = new List<BulletInfo>();
    private List<BulletInfo> playerBulletInfo = new List<BulletInfo>();
    private List<BulletInfo> enemyBulletInfo = new List<BulletInfo>();
    private string bulletPool = "Bullet";
    private SpawnPool bulletSpawnPool;
    //private GameObject bulletSpawnPool;

    void Awake()
    {
        GenerateEnemyPool();
        GenerateEffectPool();
        GenerateBulletPool();
    }
    //创建enemyPool
    void GenerateEnemyPool()
    {
        enemySpawnPool = PoolManager.Pools.Create(enemyPool);
        enemySpawnPool.group.parent = transform;
        enemySpawnPool.dontDestroyOnLoad = true;

        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            shellEnemyInfo = Game.Instance.StaticData.shellEnemyList;
            foreach (EnemyInfo info in shellEnemyInfo)
            {
                PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
                PoolSetting(prefabPool, 4);
                enemySpawnPool.CreatePrefabPool(prefabPool);
            }
        }
        else
        {
            enemyInfo = Game.Instance.StaticData.enemyList;
            foreach (EnemyInfo info in enemyInfo)
            {
                PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
                PoolSetting(prefabPool, 4);
                enemySpawnPool.CreatePrefabPool(prefabPool);
            }

            bossEnemyInfo = Game.Instance.StaticData.bossEnemyList;
            foreach (EnemyInfo info in bossEnemyInfo)
            {
                PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
                PoolSetting(prefabPool, 12);
                enemySpawnPool.CreatePrefabPool(prefabPool);
            }
        }
    }
    //创建effectPool
    void GenerateEffectPool()
    {
        effectSpawnPool = PoolManager.Pools.Create(effectPool);
        effectSpawnPool.group.parent = transform;
        enemyEffectInfo = Game.Instance.StaticData.enemyEffectList;
        foreach (EffectInfo info in enemyEffectInfo)
        {
            PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
            switch (info.Prefab.name)
            {
                case "GenerationEffect":
                    PoolSetting(prefabPool, 8);
                    break;
                case "DeadEffect":
                    PoolSetting(prefabPool, 6);
                    break;
                case "BubbleSinisterAttackEffect":
                    PoolSetting(prefabPool, 50);
                    break;
                case "BubbleAngryAttackEffect":
                    PoolSetting(prefabPool, 4);
                    break;
            }
            effectSpawnPool.CreatePrefabPool(prefabPool);
        }

        bossEffectInfo = Game.Instance.StaticData.bossEffectList;
        foreach (EffectInfo info in bossEffectInfo)
        {
            PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
            switch (info.Prefab.name)
            {
                case "MarkEffect":
                    PoolSetting(prefabPool, 20);
                    break;
                case "ThunderEffect":
                    PoolSetting(prefabPool, 20);
                    break;
            }
            effectSpawnPool.CreatePrefabPool(prefabPool);
        }

        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            bonfireEffectInfo = Game.Instance.StaticData.bonfireEffect;
            PrefabPool prefabPool = new PrefabPool(bonfireEffectInfo.Prefab.transform);
            if (bonfireEffectInfo.Prefab.name == "BonfireEffect")
            {
                PoolSetting(prefabPool, 2);
            }
            effectSpawnPool.CreatePrefabPool(prefabPool);
        }
    }
    //创建bulletPool
    void GenerateBulletPool()
    {
        bulletSpawnPool = PoolManager.Pools.Create(bulletPool);
        bulletSpawnPool.group.parent = transform;
        bossBulletInfo = Game.Instance.StaticData.bossBulletList;
        foreach (BulletInfo info in bossBulletInfo)
        {
            PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
            if (info.Prefab.name == "BubbleBossBullet")
            {
                PoolSetting(prefabPool, 50);
            }
            bulletSpawnPool.CreatePrefabPool(prefabPool);
        }

        playerBulletInfo = Game.Instance.StaticData.playerBulletList;
        foreach (BulletInfo info in playerBulletInfo)
        {
            PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
            if (info.Prefab.name == "PlayerBullet")
            {
                PoolSetting(prefabPool, 30);
            }
            bulletSpawnPool.CreatePrefabPool(prefabPool);
        }

        enemyBulletInfo = Game.Instance.StaticData.enemyBulletList;
        foreach (BulletInfo info in enemyBulletInfo)
        {
            PrefabPool prefabPool = new PrefabPool(info.Prefab.transform);
            if (info.Prefab.name == "EnemyBubbleMoveBullet")
            {
                PoolSetting(prefabPool, 50);
            }
            bulletSpawnPool.CreatePrefabPool(prefabPool);
        }
    }
    //设置EnemyPool各项参数
    void PoolSetting(PrefabPool pool, int count)
    {
        pool.preloadAmount = count;
        pool.cullDespawned = false;
        pool.limitInstances = false;
        pool.limitFIFO = false;
    }
}
