using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;
using DynamicLight2D;
using System.IO;

public class Player : MonoBehaviour
{
    #region 变量
    public Transform timeSkillCamera;
    public GameObject bullet;
    public GameObject skillButton;
    public GameObject bonfireEffect;
    public GameObject deadEffect;
    public GameObject mapRoot;

    private string filePath;

    public Sprite invincible;
    public Sprite time;
    public Sprite eye;

    public Material spriteDefault;
    public Material smooth;
    public Material shell;

    private Transform camera;
    private Transform pointLight;
    private Transform spotLight;
    private Transform lights;

    private SpawnPool effectSpawnPool;
    private SpawnPool bulletSpawnPool;
    private SpriteRenderer sr;
    private GameObject hpUI;
    private GameObject[] bonFireLight = new GameObject[2];
    private Transform[] heart = new Transform[7];
    [HideInInspector]
    public Text moneyText;
    private Text itemPrompt;

    private Animator animator;
    private ETCJoystick joystick;
    private GameObject playerMapPosition;
    private GameObject resultPanel;
    private GameObject resultResetButton;
    private GameObject mapPanel;
    private GameObject failText;
    private GameObject successText;

    [HideInInspector]
    public GameObject[] rooms;
    private RoomInstance room;
    private LevelGeneration levelGeneration;

    private Transform firePositionUp;
    private Transform firePositionUL;
    private Transform firePositionLeft;
    private Transform firePositionDL;
    private Transform firePositionDown;
    private Transform firePositionDR;
    private Transform firePositionRight;
    private Transform firePositionUR;

    [HideInInspector]
    public int money;
    private int roomCount;
    //private int roomIsClean = 0;
    private int hp = 7;
    private int speed = 50;
    private float shootTime;
    public float shootTimer = 0.7f;
    private float switchRoomTime = 1f;
    private float damageTime = 0;
    private float damageTimer = 2f;

    private bool damageCD = false;
    private bool isSuccess = false;
    //是否跳转到下个页面
    [HideInInspector]
    public bool isJump = false;
    private bool isCollider = false;
    private bool successThisLayer = false;
    private bool itemPromptShow = false;

    private int shootLevel = 0;
    private int speedLevel = 0;
    private int bulletLevel = 0;
    [HideInInspector]
    public int lightLevel = 0;
    //当前面朝的方向
    [HideInInspector]
    public Direction direction;
    [HideInInspector]
    public enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }
    #endregion

    #region 常量
    private int roomIntervalX = 960;
    private int roomIntervalY = 496;
    private int playerOffestX = 670;
    private int playerOffestY = 372;
    private int miniMapX = 16;
    private int miniMapY = 8;
    private int bonFireOffest = 80;
    private float rayDistance1 = 10.4f;
    private float rayDistance2 = 11.4f;
    private float rayDistance3 = 12.4f;
    private float rayDistance4 = 13.4f;
    #endregion

    #region Unity回调
    void Start()
    {
        filePath = Application.persistentDataPath + "/Player.xml";

        camera = GameObject.Find("MainCamera").GetComponent<Transform>();
        hpUI = GameObject.Find("HpUI");
        for (int i = 0; i < hpUI.transform.childCount; i++)
        {
            heart[i] = hpUI.transform.FindChild("Heart" + i);
        }
        //读取存档---------------
        shootTime = shootTimer;
        money = 0;
        shootLevel = Game.Instance.StaticData.ShootLevel;
        speedLevel = Game.Instance.StaticData.SpeedLevel;
        bulletLevel = Game.Instance.StaticData.BulletLevel;
        speed = Game.Instance.StaticData.Speed;
        shootTimer = Game.Instance.StaticData.ShootTimer;
        money = Game.Instance.StaticData.Money;
        switch (Game.Instance.StaticData.skillIcon)
        {
            case StaticData.SkillIcon.None:
                skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.None;
                break;
            case StaticData.SkillIcon.Invincible:
                skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.Invincible;
                skillButton.GetComponent<Image>().sprite = invincible;
                skillButton.GetComponent<ETCButton>().normalSprite = invincible;
                skillButton.GetComponent<ETCButton>().pressedSprite = invincible;

                skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                skillButton.GetComponent<ETCButton>().normalColor = new Color(1, 1, 1, 1);
                skillButton.GetComponent<ETCButton>().pressedColor = new Color(1, 1, 1, 1);
                break;
            case StaticData.SkillIcon.TimeSkill:
                skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.TimeSkill;

                skillButton.GetComponent<Image>().sprite = time;
                skillButton.GetComponent<ETCButton>().normalSprite = time;
                skillButton.GetComponent<ETCButton>().pressedSprite = time;

                skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                skillButton.GetComponent<ETCButton>().normalColor = new Color(1, 1, 1, 1);
                skillButton.GetComponent<ETCButton>().pressedColor = new Color(1, 1, 1, 1);
                break;
            case StaticData.SkillIcon.EyeSkill:
                skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.EyeSkill;

                skillButton.GetComponent<Image>().sprite = eye;
                skillButton.GetComponent<ETCButton>().normalSprite = eye;
                skillButton.GetComponent<ETCButton>().pressedSprite = eye;

                skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                skillButton.GetComponent<ETCButton>().normalColor = new Color(1, 1, 1, 1);
                skillButton.GetComponent<ETCButton>().pressedColor = new Color(1, 1, 1, 1);
                break;
        }
        //---------
        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            mapPanel = GameObject.Find("Canvas").transform.Find("MapPanel").gameObject;
            //promptText = GameObject.Find("Canvas").transform.Find("PromptText").gameObject;

            pointLight = GameObject.Find("PlayerLight").transform.Find("PointLight");
            spotLight = GameObject.Find("PlayerLight").transform.Find("SpotLight");

            lights = GameObject.Find("Lights").transform;

            for (int i = 0; i < 2; i++)
            {
                bonFireLight[i] = GameObject.Find("BonFireLights").transform.GetChild(i).gameObject;
            }
            //读取存档---------------
            lightLevel = Game.Instance.StaticData.LightLevel;

            pointLight.GetComponent<DynamicLight>().LightRadius += 20 * lightLevel;
            pointLight.GetComponent<Source>().scale += 0.5f * lightLevel;
            spotLight.GetComponent<DynamicLight>().LightRadius += 30 * lightLevel;
        }

        joystick = GameObject.Find("PlayerMoveJoystick").GetComponent<ETCJoystick>();
        resultPanel = GameObject.Find("Canvas").transform.Find("ResultPanel").gameObject;
        resultResetButton = resultPanel.transform.Find("ResetButton").gameObject;
        failText = resultPanel.transform.Find("FailText").gameObject;
        successText = resultPanel.transform.Find("SuccessText").gameObject;
        itemPrompt = GameObject.Find("Canvas").transform.Find("ItemPrompt").GetComponent<Text>();
        sr = GetComponent<SpriteRenderer>();
        effectSpawnPool = PoolManager.Pools["Effect"];
        bulletSpawnPool = PoolManager.Pools["Bullet"];
        playerMapPosition = mapRoot.transform.Find("PlayerMapPosition(Clone)").gameObject;
        rooms = GameObject.FindGameObjectsWithTag("Room");
        levelGeneration = GameObject.Find("LevelGenerator").GetComponent<LevelGeneration>();
        //总房间数
        roomCount = levelGeneration.roomCount;
        foreach (GameObject go in rooms)
        {
            room = go.GetComponent<RoomInstance>();
        }

        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        moneyText.text = money.ToString();

        direction = Direction.Down;

        firePositionUp = transform.FindChild("FirePositionUp").GetComponent<Transform>();
        firePositionUL = transform.FindChild("FirePositionUL").GetComponent<Transform>();
        firePositionLeft = transform.FindChild("FirePositionLeft").GetComponent<Transform>();
        firePositionDL = transform.FindChild("FirePositionDL").GetComponent<Transform>();
        firePositionDown = transform.FindChild("FirePositionDown").GetComponent<Transform>();
        firePositionDR = transform.FindChild("FirePositionDR").GetComponent<Transform>();
        firePositionRight = transform.FindChild("FirePositionRight").GetComponent<Transform>();
        firePositionUR = transform.FindChild("FirePositionUR").GetComponent<Transform>();

        animator = GetComponent<Animator>();

        //--------------Editor--------------//
        //GameObject BubbleBoss = Resources.Load("Game/Prefabs/Enemy/Boss/BubbleBoss") as GameObject;
        //Instantiate(BubbleBoss, Vector3.zero, Quaternion.identity);

        //Game.Instance.StaticData.UpdatePlayerInfo();
    }

    void Update()
    {
        if (!isJump)
        {
            Game.Instance.StaticData.Money = money;
            moneyText.text = money.ToString();
        }
        //被攻击时的CD
        if (damageCD)
        {
            damageTime += Time.deltaTime;
            if (damageTime >= damageTimer)
            {
                damageCD = false;
            }
        }
    }

    void FixedUpdate()
    {
        switch (direction)
        {
            case Direction.Up:
                if (speedLevel == 0)
                {
                    RaycastHit2D hitUp1 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance1, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitUp2 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance1, LayerMask.GetMask("Obstacle"));
                    if (hitUp1.collider != null || hitUp2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 1)
                {
                    RaycastHit2D hitUp1 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance2, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitUp2 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance2, LayerMask.GetMask("Obstacle"));
                    if (hitUp1.collider != null || hitUp2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 2)
                {
                    RaycastHit2D hitUp1 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance3, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitUp2 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance3, LayerMask.GetMask("Obstacle"));
                    if (hitUp1.collider != null || hitUp2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 3)
                {
                    RaycastHit2D hitUp1 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance4, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitUp2 = Physics2D.Raycast(transform.position, Vector2.up, rayDistance4, LayerMask.GetMask("Obstacle"));
                    if (hitUp1.collider != null || hitUp2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                break;
            case Direction.Left:
                if (speedLevel == 0)
                {
                    RaycastHit2D hitLeft1 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance1, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitLeft2 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance1, LayerMask.GetMask("Obstacle"));
                    if (hitLeft1.collider != null || hitLeft2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 1)
                {
                    RaycastHit2D hitLeft1 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance2, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitLeft2 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance2, LayerMask.GetMask("Obstacle"));
                    if (hitLeft1.collider != null || hitLeft2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 2)
                {
                    RaycastHit2D hitLeft1 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance3, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitLeft2 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance3, LayerMask.GetMask("Obstacle"));
                    if (hitLeft1.collider != null || hitLeft2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 3)
                {
                    RaycastHit2D hitLeft1 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance4, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitLeft2 = Physics2D.Raycast(transform.position, Vector2.left, rayDistance4, LayerMask.GetMask("Obstacle"));
                    if (hitLeft1.collider != null || hitLeft2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                break;
            case Direction.Down:
                if (speedLevel == 0)
                {
                    RaycastHit2D hitDown1 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance1, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitDown2 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance1, LayerMask.GetMask("Obstacle"));
                    if (hitDown1.collider != null || hitDown2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 1)
                {
                    RaycastHit2D hitDown1 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance2, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitDown2 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance2, LayerMask.GetMask("Obstacle"));
                    if (hitDown1.collider != null || hitDown2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 2)
                {
                    RaycastHit2D hitDown1 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance3, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitDown2 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance3, LayerMask.GetMask("Obstacle"));
                    if (hitDown1.collider != null || hitDown2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 3)
                {
                    RaycastHit2D hitDown1 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance4, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitDown2 = Physics2D.Raycast(transform.position, Vector2.down, rayDistance4, LayerMask.GetMask("Obstacle"));
                    if (hitDown1.collider != null || hitDown2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                break;
            case Direction.Right:
                if (speedLevel == 0)
                {
                    RaycastHit2D hitRight1 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance1, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitRight2 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance1, LayerMask.GetMask("Obstacle"));
                    if (hitRight1.collider != null || hitRight2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 1)
                {
                    RaycastHit2D hitRight1 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance2, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitRight2 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance2, LayerMask.GetMask("Obstacle"));
                    if (hitRight1.collider != null || hitRight2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 2)
                {
                    RaycastHit2D hitRight1 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance3, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitRight2 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance3, LayerMask.GetMask("Obstacle"));
                    if (hitRight1.collider != null || hitRight2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                if (speedLevel == 3)
                {
                    RaycastHit2D hitRight1 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance4, LayerMask.GetMask("Wall"));
                    RaycastHit2D hitRight2 = Physics2D.Raycast(transform.position, Vector2.right, rayDistance4, LayerMask.GetMask("Obstacle"));
                    if (hitRight1.collider != null || hitRight2.collider != null)
                    {
                        isCollider = true;
                    }
                    else
                    {
                        isCollider = false;
                    }
                }
                break;
        }
    }

    void LateUpdate()
    {
        //灯光跟随
        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            pointLight.position = transform.position;
            spotLight.position = transform.position;
            switch (direction)
            {
                case Direction.Up:
                    spotLight.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case Direction.Left:
                    spotLight.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case Direction.Down:
                    spotLight.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    break;
                case Direction.Right:
                    spotLight.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    break;
            }
        }
    }
    #endregion

    #region 摇杆事件
    //Idle Direction
    public void OnJoystickTouchUp()
    {
        switch (direction)
        {
            case Direction.Up:
                AnimationContorller(-1);
                break;
            case Direction.Left:
                AnimationContorller(-2);
                break;
            case Direction.Down:
                AnimationContorller(-3);
                break;
            case Direction.Right:
                AnimationContorller(-4);
                break;
        }
    }
    // 1--walkup  2--walkleft  3--walkdown  4--walkright
    public void OnJoystickPressUp()
    {
        AnimationContorller(1);
        direction = Direction.Up;
        if (!isCollider)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
    public void OnJoystickPressLeft()
    {
        AnimationContorller(2);
        direction = Direction.Left;
        if (!isCollider)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }
    public void OnJoystickPressDown()
    {
        AnimationContorller(3);
        direction = Direction.Down;
        if (!isCollider)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }
    public void OnJoystickPressRight()
    {
        AnimationContorller(4);
        direction = Direction.Right;
        if (!isCollider)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    void AnimationContorller(int index)
    {
        animator.SetInteger("WalkDirection", index);
    }
    #endregion

    #region 碰撞事件
    private void OnTriggerStay2D(Collider2D col)
    {
        switch (col.name)
        {
            case "DoorU(Clone)":
                if (direction == Direction.Up)
                {
                    //此时还在上一个房间
                    ChangeRoom();
                    //主角位移到下一个房间
                    transform.position = new Vector2(transform.position.x, transform.position.y + playerOffestY);
                    //变换小地图当前所在位置
                    playerMapPosition.transform.position = new Vector2(playerMapPosition.transform.position.x, playerMapPosition.transform.position.y + miniMapY);
                    //切换房间的过程中禁用摇杆
                    StartCoroutine(JoystickHide());
                    //需要面向门的时候才能进切换房间
                    AnimationContorller(-1);
                    //摄像机位移
                    camera.DOMoveY(camera.position.y + roomIntervalY, switchRoomTime);
                    timeSkillCamera.DOMoveY(camera.position.y + roomIntervalY, switchRoomTime);
                }
                break;
            case "DoorL(Clone)":
                if (direction == Direction.Left)
                {
                    ChangeRoom();
                    transform.position = new Vector2(transform.position.x - playerOffestX, transform.position.y);
                    playerMapPosition.transform.position = new Vector2(playerMapPosition.transform.position.x - miniMapX, playerMapPosition.transform.position.y);
                    StartCoroutine(JoystickHide());
                    AnimationContorller(-2);
                    camera.DOMoveX(camera.position.x - roomIntervalX, switchRoomTime);
                    timeSkillCamera.DOMoveX(camera.position.x - roomIntervalX, switchRoomTime);
                }
                break;
            case "DoorD(Clone)":
                if (direction == Direction.Down)
                {
                    ChangeRoom();
                    transform.position = new Vector2(transform.position.x, transform.position.y - playerOffestY);
                    playerMapPosition.transform.position = new Vector2(playerMapPosition.transform.position.x, playerMapPosition.transform.position.y - miniMapY);
                    StartCoroutine(JoystickHide());
                    AnimationContorller(-3);
                    camera.DOMoveY(camera.position.y - roomIntervalY, switchRoomTime);
                    timeSkillCamera.DOMoveY(camera.position.y - roomIntervalY, switchRoomTime);
                }
                break;
            case "DoorR(Clone)":
                if (direction == Direction.Right)
                {
                    ChangeRoom();
                    transform.position = new Vector2(transform.position.x + playerOffestX, transform.position.y);
                    playerMapPosition.transform.position = new Vector2(playerMapPosition.transform.position.x + miniMapX, playerMapPosition.transform.position.y);
                    StartCoroutine(JoystickHide());
                    AnimationContorller(-4);
                    camera.DOMoveX(camera.position.x + roomIntervalX, switchRoomTime);
                    timeSkillCamera.DOMoveX(camera.position.x + roomIntervalX, switchRoomTime);
                }
                break;
            case "BluePotion(Clone)":
                if (shootLevel < 4)
                {
                    if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
                    {
                        //itemLight重置处理
                        col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        col.transform.GetChild(0).GetChild(0).SetParent(lights);
                    }
                    Game.Instance.Sound.PlayEffect("PickItem");
                    Destroy(col.gameObject);
                    shootTimer -= 0.1f;
                    shootLevel++;
                }
                else
                {
                    if (!itemPromptShow)
                    {
                        StartCoroutine(ItemPrompt("当前道具数量已达上限"));
                    }
                }
                break;
            case "Shoes(Clone)":
                if (speedLevel < 3)
                {
                    if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
                    {
                        col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        col.transform.GetChild(0).GetChild(0).SetParent(lights);
                    }
                    Game.Instance.Sound.PlayEffect("PickItem");
                    Destroy(col.gameObject);
                    speed += 10;
                    speedLevel++;
                }
                else
                {
                    if (!itemPromptShow)
                    {
                        StartCoroutine(ItemPrompt("当前道具数量已达上限"));
                    }
                }
                break;
            case "PinkPotion(Clone)":
                if (bulletLevel < 2)
                {
                    if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
                    {
                        col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        col.transform.GetChild(0).GetChild(0).SetParent(lights);
                    }
                    Game.Instance.Sound.PlayEffect("PickItem");
                    Destroy(col.gameObject);
                    bulletLevel++;
                }
                else
                {
                    if (!itemPromptShow)
                    {
                        StartCoroutine(ItemPrompt("当前道具数量已达上限"));
                    }
                }
                break;
            case "ColourPotion(Clone)":
                if (skillButton.GetComponent<Image>().sprite != invincible)
                {
                    //当前主动道具(如果有)未处于CD才能捡其他的主动道具
                    if (skillButton.GetComponent<SkillCD>().isChange)
                    {
                        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
                        {
                            col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                            col.transform.GetChild(0).GetChild(0).SetParent(lights);
                        }
                        skillButton.GetComponent<Image>().sprite = invincible;
                        skillButton.GetComponent<ETCButton>().normalSprite = invincible;
                        skillButton.GetComponent<ETCButton>().pressedSprite = invincible;

                        skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        skillButton.GetComponent<ETCButton>().normalColor = new Color(1, 1, 1, 1);
                        skillButton.GetComponent<ETCButton>().pressedColor = new Color(1, 1, 1, 1);

                        skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.Invincible;
                        Game.Instance.Sound.PlayEffect("PickItem");
                        Destroy(col.gameObject);
                    }
                    else
                    {
                        if (!itemPromptShow)
                        {
                            StartCoroutine(ItemPrompt("当前主动道具未冷却完毕"));
                        }
                    }
                }
                break;
            case "Clock(Clone)":
                if (skillButton.GetComponent<Image>().sprite != time)
                {
                    if (skillButton.GetComponent<SkillCD>().isChange)
                    {
                        //if (SceneManager.GetActiveScene().name == "5.Level3")
                        //{
                        //    col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        //    col.transform.GetChild(0).GetChild(0).SetParent(lights);
                        //}
                        skillButton.GetComponent<Image>().sprite = time;
                        skillButton.GetComponent<ETCButton>().normalSprite = time;
                        skillButton.GetComponent<ETCButton>().pressedSprite = time;

                        skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        skillButton.GetComponent<ETCButton>().normalColor = new Color(1, 1, 1, 1);
                        skillButton.GetComponent<ETCButton>().pressedColor = new Color(1, 1, 1, 1);

                        skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.TimeSkill;
                        Game.Instance.Sound.PlayEffect("PickItem");
                        Destroy(col.gameObject);
                    }
                    else
                    {
                        if (!itemPromptShow)
                        {
                            StartCoroutine(ItemPrompt("当前主动道具未冷却完毕"));
                        }
                    }
                }
                break;
            case "Paper(Clone)":
                if (skillButton.GetComponent<Image>().sprite != eye)
                {
                    if (skillButton.GetComponent<SkillCD>().isChange)
                    {
                        col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        col.transform.GetChild(0).GetChild(0).SetParent(lights);

                        skillButton.GetComponent<Image>().sprite = eye;
                        skillButton.GetComponent<ETCButton>().normalSprite = eye;
                        skillButton.GetComponent<ETCButton>().pressedSprite = eye;

                        skillButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        skillButton.GetComponent<ETCButton>().normalColor = new Color(1, 1, 1, 1);
                        skillButton.GetComponent<ETCButton>().pressedColor = new Color(1, 1, 1, 1);

                        skillButton.GetComponent<SkillCD>().skillIcon = SkillCD.SkillIcon.EyeSkill;
                        Game.Instance.Sound.PlayEffect("PickItem");
                        Destroy(col.gameObject);
                    }
                    else
                    {
                        if (!itemPromptShow)
                        {
                            StartCoroutine(ItemPrompt("当前主动道具未冷却完毕"));
                        }
                    }
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "AttackEffect" || col.tag == "Thorn" || col.tag == "BossEnemy")
        {
            if (skillButton.GetComponent<SkillCD>().skill != SkillCD.Skill.Invincible)
            {
                if (!damageCD)
                {
                    Damage();
                }
            }
        }
        switch (col.name)
        {
            case "Hole(Clone)":
                //进入下一层
                if (Game.Instance.StaticData.LayerCount == 1)
                {
                    Game.Instance.StaticData.LayerCount++;
                    SavePlayerInfo();
                    if (!File.Exists(filePath))
                    {
                        Game.Instance.StaticData.SavePlayerInfo();
                    }
                    else
                    {
                        Game.Instance.StaticData.UpdatePlayerInfo();
                    }
                }
                else
                {
                    Game.Instance.StaticData.LayerCount++;
                    SavePlayerInfo();
                    Game.Instance.StaticData.UpdatePlayerInfo();
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "Bulb(Clone)":
                if (lightLevel < 2)
                {
                    //灯光亮度增大
                    pointLight.GetComponent<DynamicLight>().LightRadius += 20;
                    pointLight.GetComponent<Source>().scale += 0.5f;
                    spotLight.GetComponent<DynamicLight>().LightRadius += 30;
                    col.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    col.transform.GetChild(0).GetChild(0).SetParent(lights);

                    Game.Instance.Sound.PlayEffect("PickItem");
                    Destroy(col.gameObject);
                    lightLevel++;
                }
                else
                {
                    if (!itemPromptShow)
                    {
                        StartCoroutine(ItemPrompt("当前道具数量已达上限"));
                    }
                }
                break;
            case "Map(Clone)":
                mapPanel.SetActive(true);
                Game.Instance.Sound.PlayEffect("PickItem");
                Destroy(col.gameObject);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            if (skillButton.GetComponent<SkillCD>().skill != SkillCD.Skill.Invincible)
            {
                if (!damageCD)
                {
                    Damage();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "EnemySad" || col.gameObject.tag == "Boss")
        {
            if (skillButton.GetComponent<SkillCD>().skill != SkillCD.Skill.Invincible)
            {
                if (!damageCD)
                {
                    if (!isSuccess)
                    {
                        Damage();
                    }
                }
            }
        }
    }
    #endregion

    #region 方法
    //换到其他房间
    void ChangeRoom()
    {
        foreach (GameObject r in rooms)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(r.transform.position.x, r.transform.position.y)) < 170)
            {
                if (r.GetComponent<RoomInstance>().type == 1)
                {

                }
                else
                {
                    EnemySpawn enemySpawn = r.GetComponent<EnemySpawn>();
                    //这个房间视为没生成过怪物(下次进入再生成怪物)
                    enemySpawn.genreateEnemy = false;
                    //换房间的时候清掉这个房间的所有怪物和子弹
                    enemySpawn.RemoveEnemy();
                    enemySpawn.RemoveBullet();

                    rooms = GameObject.FindGameObjectsWithTag("Room");

                    int roomIsClean = 0;
                    foreach (GameObject go in rooms)
                    {
                        room = go.GetComponent<RoomInstance>();
                        if (room.isClean)
                        {
                            //出生点默认IsClean
                            roomIsClean++;
                            if (roomIsClean == roomCount)
                            {
                                //如果通过了本层
                                if (!successThisLayer)
                                {
                                    successThisLayer = true;
                                    //如果不在最后一层(第五层),生成地洞
                                    if (Game.Instance.StaticData.LayerCount != Game.Instance.StaticData.MaxLayer)
                                    {
                                        GameObject hole = Resources.Load("Game/Prefabs/Map/Hole") as GameObject;
                                        Instantiate(hole, Vector3.zero, Quaternion.identity);
                                        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
                                        {
                                            GameObject Wood = Resources.Load("Game/Prefabs/Map/Level3/Wood") as GameObject;
                                            //GameObject BonfireEffect = Resources.Load("Game/Prefabs/Effect/BonfireEffect") as GameObject;
                                            //木堆
                                            Instantiate(Wood, new Vector2(bonFireOffest, 0), Quaternion.identity);
                                            //火焰特效
                                            effectSpawnPool.Spawn(bonfireEffect, new Vector2(bonFireOffest, 0), Quaternion.identity);
                                            //Instantiate(bonfireEffect, new Vector2(bonFireOffest, 0), Quaternion.identity);
                                            //光线
                                            bonFireLight[0].SetActive(true);
                                            bonFireLight[0].transform.position = new Vector2(bonFireOffest, 0);

                                            Instantiate(Wood, new Vector2(-bonFireOffest, 0), Quaternion.identity);
                                            effectSpawnPool.Spawn(bonfireEffect, new Vector2(-bonFireOffest, 0), Quaternion.identity);
                                            //Instantiate(bonfireEffect, new Vector2(-bonFireOffest, 0), Quaternion.identity);
                                            bonFireLight[1].SetActive(true);
                                            bonFireLight[1].transform.position = new Vector2(-bonFireOffest, 0);
                                        }
                                    }
                                    else
                                    {
                                        if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level3)
                                        {
                                            GameObject BubbleBoss = Resources.Load("Game/Prefabs/Enemy/Boss/BubbleBoss") as GameObject;
                                            Instantiate(BubbleBoss, Vector3.zero, Quaternion.identity);
                                        }
                                        else
                                        {
                                            //Level3通关后的处理
                                            GameObject map = Resources.Load("Game/Prefabs/Item/Map") as GameObject;
                                            Instantiate(map, Vector2.zero, Quaternion.identity);

                                            GameObject Wood = Resources.Load("Game/Prefabs/Map/Level3/Wood") as GameObject;
                                            //GameObject BonfireEffect = Resources.Load("Game/Prefabs/Effect/BonfireEffect") as GameObject;
                                            //木堆
                                            Instantiate(Wood, new Vector2(bonFireOffest, 0), Quaternion.identity);
                                            //火焰特效
                                            effectSpawnPool.Spawn(bonfireEffect, new Vector2(bonFireOffest, 0), Quaternion.identity);
                                            //Instantiate(bonfireEffect, new Vector2(bonFireOffest, 0), Quaternion.identity);
                                            //光线
                                            bonFireLight[0].SetActive(true);
                                            bonFireLight[0].transform.position = new Vector2(bonFireOffest, 0);

                                            Instantiate(Wood, new Vector2(-bonFireOffest, 0), Quaternion.identity);
                                            effectSpawnPool.Spawn(bonfireEffect, new Vector2(-bonFireOffest, 0), Quaternion.identity);
                                            //Instantiate(bonfireEffect, new Vector2(-bonFireOffest, 0), Quaternion.identity);
                                            bonFireLight[1].SetActive(true);
                                            bonFireLight[1].transform.position = new Vector2(-bonFireOffest, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator JoystickHide()
    {
        joystick.activated = false;
        yield return new WaitForSeconds(switchRoomTime);
        joystick.activated = true;
    }

    public void Damage()
    {
        if (hp > 0)
        {
            hp--;

            heart[hp].gameObject.SetActive(false);
            StartCoroutine(Hurt());
            Game.Instance.Sound.PlayEffect("Hurt");
            //死亡
            if (hp == 0)
            {
                if (!isSuccess)
                {
                    Instantiate(deadEffect, transform.position, Quaternion.identity);
                    Game.Instance.Sound.PlayEffect("Lose");
                    resultPanel.SetActive(true);
                    failText.SetActive(true);
                    Tween tween = resultPanel.transform.DOScale(1, 2.0f);
                    tween.SetEase(Ease.OutBack);
                    gameObject.SetActive(false);
                }
            }
        }
        damageCD = true;
        damageTime = 0;
    }

    IEnumerator Hurt()
    {
        sr.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    IEnumerator ItemPrompt(string s)
    {
        itemPromptShow = true;
        itemPrompt.text = s;
        yield return new WaitForSeconds(3.0f);
        itemPrompt.text = "";
        itemPromptShow = false;
    }

    public void Success()
    {
        isSuccess = true;
        Game.Instance.Sound.PlayEffect("Success");
        resultPanel.SetActive(true);
        successText.SetActive(true);
        resultResetButton.SetActive(false);
        //加金币
        money += 500;
        //moneyText.text = money.ToString();
        Tween tween = resultPanel.transform.DOScale(1, 2.0f);
        tween.SetEase(Ease.OutBack);
    }

    #endregion

    #region 按键监听
    public void OnShootButtonPressed()
    {
        if (shootTime > shootTimer)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (bulletLevel != 2)
                    {
                        //bulletSpawnPool.Spawn(bullet, firePositionUp.position, Quaternion.Euler(transform.eulerAngles));
                        Instantiate(bullet, firePositionUp.position, Quaternion.Euler(transform.eulerAngles));
                    }
                    else
                    {
                        Instantiate(bullet, firePositionUL.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 45)));
                        Instantiate(bullet, firePositionUp.position, Quaternion.Euler(transform.eulerAngles));
                        Instantiate(bullet, firePositionUR.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -45)));
                    }
                    break;
                case Direction.Left:
                    if (bulletLevel != 2)
                    {
                        //bulletSpawnPool.Spawn(bullet, firePositionLeft.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 90)));
                        Instantiate(bullet, firePositionLeft.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 90)));
                    }
                    else
                    {
                        Instantiate(bullet, firePositionUL.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 45)));
                        Instantiate(bullet, firePositionLeft.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 90)));
                        Instantiate(bullet, firePositionDL.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 135)));
                    }
                    break;
                case Direction.Down:
                    if (bulletLevel != 2)
                    {
                        //bulletSpawnPool.Spawn(bullet, firePositionDown.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -180)));
                        Instantiate(bullet, firePositionDown.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -180)));
                    }
                    else
                    {
                        Instantiate(bullet, firePositionDL.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 135)));
                        Instantiate(bullet, firePositionDown.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -180)));
                        Instantiate(bullet, firePositionDR.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -135)));
                    }
                    break;
                case Direction.Right:
                    if (bulletLevel != 2)
                    {
                        //bulletSpawnPool.Spawn(bullet, firePositionRight.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -90)));
                        Instantiate(bullet, firePositionRight.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -90)));
                    }
                    else
                    {
                        Instantiate(bullet, firePositionDR.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -135)));
                        Instantiate(bullet, firePositionRight.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -90)));
                        Instantiate(bullet, firePositionUR.position, Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -45)));
                    }
                    break;
            }
            shootTime = 0;
        }
        else
        {
            shootTime += Time.deltaTime;
        }

    }

    public void OnSkillButtonDown()
    {
        if (skillButton.GetComponent<Image>().sprite == invincible)
        {
            GameObject invincibleEffect = Resources.Load("Game/Prefabs/Effect/Skill/InvincibleEffect") as GameObject;
            GameObject go = Instantiate(invincibleEffect, transform.position, Quaternion.identity);
            go.transform.SetParent(transform);

            skillButton.GetComponent<SkillCD>().invincibleCD = true;
        }
        if (skillButton.GetComponent<Image>().sprite == time)
        {
            camera.gameObject.GetComponent<PostProcessingBehaviour>().enabled = true;

            skillButton.GetComponent<SkillCD>().timeSkillCD = true;
        }
        if (skillButton.GetComponent<Image>().sprite == eye)
        {
            //所有room点亮(替换材质)
            foreach (GameObject go in rooms)
            {
                go.GetComponent<SpriteRenderer>().material = spriteDefault;
            }
            //所有enemy点亮(包括未生成的)
            for (int i = 0; i < GameObject.Find("EnemyPool").transform.childCount; i++)
            {
                GameObject.Find("EnemyPool").transform.GetChild(i).GetComponent<SpriteRenderer>().material = spriteDefault;
            }
            //摄像机特效
            camera.GetComponent<PostProcessingBehaviour>().profile = Resources.Load("EyeEffect") as PostProcessingProfile;
            //熄灭灯光
            if (pointLight.gameObject.activeInHierarchy)
            {
                pointLight.gameObject.SetActive(false);
            }
            else
            {
                spotLight.gameObject.SetActive(false);
            }
            skillButton.GetComponent<SkillCD>().eyeSkillCD = true;
        }
    }
    //}
    #endregion

    #region 数据存储
    //用于场景间跳转(进入新的一层迷宫)的时候保存数据用于传递
    public void SavePlayerInfo()
    {
        //Game.Instance.StaticData.Hp = hp;
        Game.Instance.StaticData.ShootLevel = shootLevel;
        Game.Instance.StaticData.SpeedLevel = speedLevel;
        Game.Instance.StaticData.BulletLevel = bulletLevel;
        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            Game.Instance.StaticData.LightLevel = lightLevel;
        }
        Game.Instance.StaticData.Speed = speed;
        Game.Instance.StaticData.ShootTimer = shootTimer;
        Game.Instance.StaticData.Money = money;
        switch (skillButton.GetComponent<SkillCD>().skillIcon)
        {
            case SkillCD.SkillIcon.None:
                Game.Instance.StaticData.skillIcon = StaticData.SkillIcon.None;
                break;
            case SkillCD.SkillIcon.Invincible:
                Game.Instance.StaticData.skillIcon = StaticData.SkillIcon.Invincible;
                break;
            case SkillCD.SkillIcon.TimeSkill:
                Game.Instance.StaticData.skillIcon = StaticData.SkillIcon.TimeSkill;
                break;
            case SkillCD.SkillIcon.EyeSkill:
                Game.Instance.StaticData.skillIcon = StaticData.SkillIcon.EyeSkill;
                break;
        }
        if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            Game.Instance.StaticData.LightLevel = lightLevel;
        }
    }
    #endregion
}
