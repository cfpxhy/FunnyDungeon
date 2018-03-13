using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;

public class StaticData : Singleton<StaticData> {

    private string path = "Game/Res/";

    private string main = "2.Main";
    public String Main
    {
        get { return main; }
        set { main = value; }
    }

    private string level1 = "3.Level1";
    public String Level1
    {
        get { return level1; }
        set { level1 = value; }
    }

    private string level2 = "4.Level2";
    public String Level2
    {
        get { return level2; }
        set { level2 = value; }
    }

    private string level3 = "5.Level3";
    public String Level3
    {
        get { return level3; }
        set { level3 = value; }
    }

    private float bgVolume = 1.0f;
    public float BgVolume
    {
        get { return bgVolume; }
        set { bgVolume = value; }
    }

    private float effectVolume = 1.0f;
    public float EffectVolume
    {
        get { return effectVolume; }
        set { effectVolume = value; }
    }
    //0锁着 1解锁了
    private int level2Lock = 0;
    public int Level2Lock
    {
        get { return level2Lock; }
        set { level2Lock = value; }
    }

    private int level3Lock = 0;
    public int Level3Lock
    {
        get { return level3Lock; }
        set { level3Lock = value; }
    }


    [HideInInspector]
    public List<EnemyInfo> enemyList = new List<EnemyInfo>();
    [HideInInspector]
    public List<EnemyInfo> shellEnemyList = new List<EnemyInfo>();
    [HideInInspector]
    public List<EnemyInfo> bossEnemyList = new List<EnemyInfo>();

    [HideInInspector]
    public List<EffectInfo> enemyEffectList = new List<EffectInfo>();
    [HideInInspector]
    public List<EffectInfo> bossEffectList = new List<EffectInfo>();
    [HideInInspector]
    public EffectInfo bonfireEffect;


    [HideInInspector]
    public List<BulletInfo> playerBulletList = new List<BulletInfo>();
    [HideInInspector]
    public List<BulletInfo> enemyBulletList = new List<BulletInfo>();
    [HideInInspector]
    public List<BulletInfo> bossBulletList = new List<BulletInfo>();

    [HideInInspector]
    public List<ItemInfo> itemList = new List<ItemInfo>();
    [HideInInspector]
    public List<ItemInfo> shineItemList = new List<ItemInfo>();

    private int money = 0;
    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    private int totalMoney = 0;
    public int TotalMoney
    {
        get { return totalMoney; }
        set { totalMoney = value; }
    }

    private int maxLayer = 3;
    public int MaxLayer
    {
        get { return maxLayer; }
        set { maxLayer = value; }
    }

    private int layerCount = 1;
    public int LayerCount
    {
        get { return layerCount; }
        set { layerCount = value; }
    }

    private int hp = 0;
    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    private int shootLevel = 0;
    public int ShootLevel
    {
        get { return shootLevel; }
        set { shootLevel = value; }
    }
    
    private int speedLevel = 0;
    public int SpeedLevel
    {
        get { return speedLevel; }
        set { speedLevel = value; }
    }
    
    private int bulletLevel = 0;
    public int BulletLevel
    {
        get { return bulletLevel; }
        set { bulletLevel = value; }
    }
    
    private int lightLevel = 0;
    public int LightLevel
    {
        get { return lightLevel; }
        set { lightLevel = value; }
    }
    
    private int speed = 50;
    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    
    private float shootTimer = 0.7f;
    public float ShootTimer
    {
        get { return shootTimer; }
        set { shootTimer = value; }
    }
    [HideInInspector]
    public SkillIcon skillIcon;
    [HideInInspector]
    public enum SkillIcon
    {
        None,
        Invincible,
        TimeSkill,
        EyeSkill
    }

    void Start()
    {
        SerializeEnemy();
        SerializeEffect();
        SerializeBullet();
        SerializeItem();
    }

    #region 解析
    public void SerializeGameInfo()
    {
        //下一次开启游戏时候需要加载的东西
        string filePath = Application.persistentDataPath + "/Game.xml";
        if (File.Exists(filePath))
        {
            XmlDocument game = new XmlDocument();
            game.LoadXml(File.ReadAllText(filePath, Encoding.UTF8));
            XmlNode rootNode = game.SelectSingleNode("Game");
            XmlNodeList gameNodeList = rootNode.ChildNodes;
            foreach (XmlNode gameNode in gameNodeList)
            {
                if (gameNode.Name == "TotalMoney")
                {
                    TotalMoney = int.Parse(gameNode.InnerText);
                }
                if (gameNode.Name == "BgVolume")
                {
                    BgVolume = float.Parse(gameNode.InnerText);
                }
                if (gameNode.Name == "EffectVolume")
                {
                    EffectVolume = float.Parse(gameNode.InnerText);
                }
                if (gameNode.Name == "Level2Lock")
                {
                    Level2Lock = int.Parse(gameNode.InnerText);
                }
                if (gameNode.Name == "Level3Lock")
                {
                    Level3Lock = int.Parse(gameNode.InnerText);
                }
            }
        }
    }

    private void SerializeEnemy()
    {
        XmlDocument level1 = new XmlDocument();
        //level1.Load(path + "Enemy.xml");
        level1.LoadXml(Resources.Load<TextAsset>(path + "Enemy").text);
        //根节点
        XmlNode rootNode = level1.SelectSingleNode("Level");
        //Enemy结点集合
        XmlNodeList enemyNodeList = rootNode.ChildNodes;
        foreach (XmlNode enemyNode in enemyNodeList)
        {
            if (enemyNode.Name == "Enemy")
            {
                EnemyInfo enemyInfo = new EnemyInfo();
                //获取Enemy结点下面的所有结点
                XmlNodeList fieldNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    //找到名为ID的结点
                    if (fieldNode.Name == "ID")
                    {
                        //获取结点内的文本
                        int id = int.Parse(fieldNode.InnerText);
                        enemyInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        enemyInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                enemyList.Add(enemyInfo);
            }
            if (enemyNode.Name == "ShellEnemy")
            {
                EnemyInfo enemyInfo = new EnemyInfo();
                //获取Enemy结点下面的所有结点
                XmlNodeList fieldNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    //找到名为ID的结点
                    if (fieldNode.Name == "ID")
                    {
                        //获取结点内的文本
                        int id = int.Parse(fieldNode.InnerText);
                        enemyInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        enemyInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                shellEnemyList.Add(enemyInfo);
            }
            if (enemyNode.Name == "Boss")
            {
                EnemyInfo enemyInfo = new EnemyInfo();
                //获取Enemy结点下面的所有结点
                XmlNodeList fieldNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    //找到名为ID的结点
                    if (fieldNode.Name == "ID")
                    {
                        //获取结点内的文本
                        int id = int.Parse(fieldNode.InnerText);
                        enemyInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        enemyInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                bossEnemyList.Add(enemyInfo);
            }
        }
        //foreach (EnemyInfo e in shellEnemyList)
        //{
        //    Debug.Log(e);
        //}
    }

    private void SerializeEffect()
    {
        XmlDocument level1 = new XmlDocument();
        //level1.Load(path + "Effect.xml");
        level1.LoadXml(Resources.Load<TextAsset>(path + "Effect").text);
        //根节点
        XmlNode rootNode = level1.SelectSingleNode("Effect");
        //Enemy结点集合
        XmlNodeList effectNodeList = rootNode.ChildNodes;
        foreach (XmlNode enemyNode in effectNodeList)
        {
            if (enemyNode.Name == "Enemy")
            {
                EffectInfo effectInfo = new EffectInfo();
                XmlNodeList enemyEffectNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in enemyEffectNodeList)
                {
                    if (fieldNode.Name == "ID")
                    {
                        int id = int.Parse(fieldNode.InnerText);
                        effectInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        effectInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                enemyEffectList.Add(effectInfo);
            }
            else if (enemyNode.Name == "Boss")
            {
                EffectInfo effectInfo = new EffectInfo();
                XmlNodeList enemyEffectNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in enemyEffectNodeList)
                {
                    if (fieldNode.Name == "ID")
                    {
                        int id = int.Parse(fieldNode.InnerText);
                        effectInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        effectInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                bossEffectList.Add(effectInfo);
            }
            else
            {
                EffectInfo effectInfo = new EffectInfo();
                XmlNodeList enemyEffectNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in enemyEffectNodeList)
                {
                    if (fieldNode.Name == "ID")
                    {
                        int id = int.Parse(fieldNode.InnerText);
                        effectInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        effectInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                bonfireEffect = effectInfo;
            }
        }
        //foreach (EffectInfo e in bonfireEffect)
        //{
        //    Debug.Log(bonfireEffect.ID + "---------------" + bonfireEffect.Prefab);
        //}
    }

    private void SerializeBullet()
    {
        XmlDocument level1 = new XmlDocument();
        //level1.Load(path + "Bullet.xml");
        level1.LoadXml(Resources.Load<TextAsset>(path + "Bullet").text);
        //根节点
        XmlNode rootNode = level1.SelectSingleNode("Bullet");
        //Enemy结点集合
        XmlNodeList bulletNodeList = rootNode.ChildNodes;
        foreach (XmlNode bulletNode in bulletNodeList)
        {
            if (bulletNode.Name == "Boss")
            {
                BulletInfo bulletInfo = new BulletInfo();
                XmlNodeList enemyEffectNodeList = bulletNode.ChildNodes;
                foreach (XmlNode fieldNode in enemyEffectNodeList)
                {
                    if (fieldNode.Name == "ID")
                    {
                        int id = int.Parse(fieldNode.InnerText);
                        bulletInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        bulletInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                bossBulletList.Add(bulletInfo);
            }
            if (bulletNode.Name == "Player")
            {
                BulletInfo bulletInfo = new BulletInfo();
                XmlNodeList enemyEffectNodeList = bulletNode.ChildNodes;
                foreach (XmlNode fieldNode in enemyEffectNodeList)
                {
                    if (fieldNode.Name == "ID")
                    {
                        int id = int.Parse(fieldNode.InnerText);
                        bulletInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        bulletInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                playerBulletList.Add(bulletInfo);
            }
            if (bulletNode.Name == "Enemy")
            {
                BulletInfo bulletInfo = new BulletInfo();
                XmlNodeList enemyEffectNodeList = bulletNode.ChildNodes;
                foreach (XmlNode fieldNode in enemyEffectNodeList)
                {
                    if (fieldNode.Name == "ID")
                    {
                        int id = int.Parse(fieldNode.InnerText);
                        bulletInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        bulletInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                enemyBulletList.Add(bulletInfo);
            }
        }
        //foreach (BulletInfo e in bulletList)
        //{
        //    Debug.Log(e);
        //}
    }

    private void SerializeItem()
    {
        XmlDocument level1 = new XmlDocument();
        //level1.Load(path + "Item.xml");
        level1.LoadXml(Resources.Load<TextAsset>(path + "Item").text);
        //根节点
        XmlNode rootNode = level1.SelectSingleNode("Item");
        //Enemy结点集合
        XmlNodeList enemyNodeList = rootNode.ChildNodes;
        foreach (XmlNode enemyNode in enemyNodeList)
        {
            if (enemyNode.Name == "Prop")
            {
                ItemInfo itemInfo = new ItemInfo();
                //获取Enemy结点下面的所有结点
                XmlNodeList fieldNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    //找到名为ID的结点
                    if (fieldNode.Name == "ID")
                    {
                        //获取结点内的文本
                        int id = int.Parse(fieldNode.InnerText);
                        itemInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        itemInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                itemList.Add(itemInfo);
            }
            if (enemyNode.Name == "ShineProp")
            {
                ItemInfo itemInfo = new ItemInfo();
                //获取Enemy结点下面的所有结点
                XmlNodeList fieldNodeList = enemyNode.ChildNodes;
                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    //找到名为ID的结点
                    if (fieldNode.Name == "ID")
                    {
                        //获取结点内的文本
                        int id = int.Parse(fieldNode.InnerText);
                        itemInfo.ID = id;
                    }
                    if (fieldNode.Name == "url")
                    {
                        string url = fieldNode.InnerText;
                        itemInfo.Prefab = Resources.Load(url) as GameObject;
                    }
                }
                shineItemList.Add(itemInfo);
            }
        }
        //foreach (ItemInfo e in shineItemList)
        //{
        //    Debug.Log(e);
        //}
    }
    #endregion

    #region 存储
    public void SavePlayerInfo()
    {
        string filePath = Application.persistentDataPath + "/Player.xml";

        if (!File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            //头结点
            XmlElement root = xmlDoc.CreateElement("Player");

            XmlElement locallayer = xmlDoc.CreateElement("LevelLayer");
            locallayer.InnerText = LayerCount.ToString();
            XmlElement localHp = xmlDoc.CreateElement("Hp");
            localHp.InnerText = Hp.ToString();
            XmlElement localShootLevel = xmlDoc.CreateElement("ShootLevel");
            localShootLevel.InnerText = ShootLevel.ToString();
            XmlElement localSpeedLevel = xmlDoc.CreateElement("SpeedLevel");
            localSpeedLevel.InnerText = SpeedLevel.ToString();
            XmlElement localBulletLevel = xmlDoc.CreateElement("BulletLevel");
            localBulletLevel.InnerText = BulletLevel.ToString();
            XmlElement localLightlevel = xmlDoc.CreateElement("LightLevel");
            localLightlevel.InnerText = LightLevel.ToString();
            XmlElement localSpeed = xmlDoc.CreateElement("Speed");
            localSpeed.InnerText = Speed.ToString();
            XmlElement localShootTimer = xmlDoc.CreateElement("ShootTimer");
            localShootTimer.InnerText = ShootTimer.ToString();
            XmlElement localMoney = xmlDoc.CreateElement("Money");
            localMoney.InnerText = Money.ToString();
            XmlElement localSkillIcon = xmlDoc.CreateElement("SkillICon");
            localSkillIcon.InnerText = skillIcon.ToString();

            root.AppendChild(locallayer);
            root.AppendChild(localHp);
            root.AppendChild(localShootLevel);
            root.AppendChild(localSpeedLevel);
            root.AppendChild(localBulletLevel);
            root.AppendChild(localLightlevel);
            root.AppendChild(localSpeed);
            root.AppendChild(localShootTimer);
            root.AppendChild(localMoney);
            root.AppendChild(localSkillIcon);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filePath);
        }
    }

    public void UpdatePlayerInfo()
    {
        string filePath = Application.persistentDataPath + "/Player.xml";

        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(filePath);

            XmlNodeList playerNodeList = xmlDoc.SelectSingleNode("Player").ChildNodes;
            //遍历Player结点下所有子节点
            foreach (XmlElement xe in playerNodeList)
            {
                switch (xe.Name)
                {
                    case "LevelLayer":
                        xe.InnerText = LayerCount.ToString();
                        break;
                    case "Hp":
                        xe.InnerText = Hp.ToString();
                        break;
                    case "ShootLevel":
                        xe.InnerText = ShootLevel.ToString();
                        break;
                    case "SpeedLevel":
                        xe.InnerText = SpeedLevel.ToString();
                        break;
                    case "BulletLevel":
                        xe.InnerText = BulletLevel.ToString();
                        break;
                    case "LightLevel":
                        xe.InnerText = BulletLevel.ToString();
                        break;
                    case "Speed":
                        xe.InnerText = Speed.ToString();
                        break;
                    case "ShootTimer":
                        xe.InnerText = ShootTimer.ToString();
                        break;
                    case "Money":
                        xe.InnerText = Money.ToString();
                        break;
                    case "SkillIcon":
                        xe.InnerText = skillIcon.ToString();
                        break;
                }
            }
            xmlDoc.Save(filePath);
        }
    }

    public void DeletePlayerInfo()
    {
        string filePath = Application.persistentDataPath + "/Player.xml";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void ResetPlayInfo()
    {
        Money = 0;

        LayerCount = 1;
        Hp = 0;
        ShootLevel = 0;
        SpeedLevel = 0;
        BulletLevel = 0;
        Speed = 50;
        ShootTimer = 0.7f;
        skillIcon = SkillIcon.None;
    }

    public void ResetGameInfo()
    {
        TotalMoney = 0;
        Level2Lock = 0;
        Level3Lock = 0;
    }

    public void AddMoney(int m)
    {
        TotalMoney += m;
    }

    public void SaveGameInfo()
    {
        string filePath = Application.persistentDataPath + "/Game.xml";

        if (!File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            //头结点
            XmlElement root = xmlDoc.CreateElement("Game");

            XmlElement localMoney = xmlDoc.CreateElement("TotalMoney");
            localMoney.InnerText = TotalMoney.ToString();
            XmlElement localBgVolume = xmlDoc.CreateElement("BgVolume");
            localBgVolume.InnerText = BgVolume.ToString();
            XmlElement localEffectVolume = xmlDoc.CreateElement("EffectVolume");
            localEffectVolume.InnerText = EffectVolume.ToString();
            XmlElement localLevel2Lock = xmlDoc.CreateElement("Level2Lock");
            localLevel2Lock.InnerText = Level2Lock.ToString();
            XmlElement localLevel3Lock = xmlDoc.CreateElement("Level3Lock");
            localLevel3Lock.InnerText = Level3Lock.ToString();

            root.AppendChild(localMoney);
            root.AppendChild(localBgVolume);
            root.AppendChild(localEffectVolume);
            root.AppendChild(localLevel2Lock);
            root.AppendChild(localLevel3Lock);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filePath);
        }
    }

    public void UpdateGameInfo()
    {
        string filePath = Application.persistentDataPath + "/Game.xml";

        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(filePath);

            XmlNodeList gameNodeList = xmlDoc.SelectSingleNode("Game").ChildNodes;
            //遍历Player结点下所有子节点
            foreach (XmlElement xe in gameNodeList)
            {
                switch (xe.Name)
                {
                    case "TotalMoney":
                        xe.InnerText = TotalMoney.ToString();
                        break;
                    case "BgVolume":
                        xe.InnerText = BgVolume.ToString();
                        break;
                    case "EffectVolume":
                        xe.InnerText = EffectVolume.ToString();
                        break;
                    case "Level2Lock":
                        xe.InnerText = Level2Lock.ToString();
                        break;
                    case "Level3Lock":
                        xe.InnerText = Level3Lock.ToString();
                        break;
                }
            }
            xmlDoc.Save(filePath);
        }
    }
    #endregion
}
