using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class SkillCD : MonoBehaviour {
    GameObject camera;
    Camera cam;
    GameObject timeSkillCamera;
    Sprite sprite;
    [HideInInspector]
    public GameObject maskGameObject;
    Image mask;
    GameObject CDGameObject;
    Text CD;
    ETCButton etc;
    LightUI lightUI;
    GameObject pointLight;
    GameObject spotLight;
    Player p;

    private float timer = 0;
    private float invincibleTime = 10.0f;
    private float timeSkillTime = 8.0f;
    private float eyeSkillTime = 8.0f;
    private float invincibleTimer = 40.0f;
    private float timeSkillTimer = 30.0f;
    private float eyeSkillTimer = 30.0f;
    [HideInInspector]
    public bool invincibleCD = false;
    [HideInInspector]
    public bool timeSkillCD = false;
    [HideInInspector]
    public bool eyeSkillCD = false;
    //能否捡起其他的主动道具
    [HideInInspector]
    public bool isChange = true;
    //当前主动技能UI图标
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
    //当前正在放的技能
    [HideInInspector]
    public Skill skill;
    [HideInInspector]
    public enum Skill
    {
        None,
        Invincible,
        TimeSkill,
        EyeSkill
    }

    void Start () {
        camera = GameObject.Find("MainCamera");
        cam = camera.GetComponent<Camera>();
        if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level3)
        {
            timeSkillCamera = transform.parent.FindChild("TimeSkillCamera").gameObject;
        }
        else
        {
            p = GameObject.Find("Player").GetComponent<Player>();
            lightUI = GameObject.Find("Canvas").GetComponent<LightUI>();
            pointLight = GameObject.Find("PlayerLight").transform.Find("PointLight").gameObject;
            spotLight = GameObject.Find("PlayerLight").transform.Find("SpotLight").gameObject;
        }
        sprite = GetComponent<Image>().sprite;
        maskGameObject = transform.FindChild("Mask").gameObject;
        mask = maskGameObject.GetComponent<Image>();
        CDGameObject = transform.FindChild("CD").gameObject;
        CD = CDGameObject.GetComponent<Text>();
        CD.text = "";
        etc = GetComponent<ETCButton>();
        if (Game.Instance.StaticData.LayerCount == 1)
        {
            skillIcon = SkillIcon.None;
        }
    }
	
	void Update () {
        if (skillIcon == SkillIcon.Invincible)
        {
            if (invincibleCD)
            {
                timer += Time.deltaTime;
                //skill = Skill.Invincible;
                isChange = false;
                //按键不可点击
                etc.activated = false;
                //启用CD遮罩
                maskGameObject.SetActive(true);
                //CD遮罩运行
                mask.fillAmount = (invincibleTimer - timer) / invincibleTimer;
                //实时显示CD还剩多久
                CD.text = ((int)(invincibleTimer - timer)).ToString();
                if (timer < invincibleTime)
                {
                    skill = Skill.Invincible;
                    //Player不受到任何伤害
                }
                else
                {
                    skill = Skill.None;
                }
                if (timer >= invincibleTimer)
                {
                    mask.fillAmount = 1;
                    etc.activated = true;
                    maskGameObject.SetActive(false);
                    invincibleCD = false;
                    isChange = true;
                    //skillIcon = SkillIcon.None;
                    CD.text = "";
                    timer = 0;
                }
            }
        }
        if (skillIcon == SkillIcon.TimeSkill)
        {
            if (timeSkillCD)
            {
                timer += Time.deltaTime;

                etc.activated = false;
                isChange = false;
                maskGameObject.SetActive(true);
                mask.fillAmount = (timeSkillTimer - timer) / timeSkillTimer;
                CD.text = ((int)(timeSkillTimer - timer)).ToString();
                if (timer < timeSkillTime)
                {
                    //主摄像机剔除Player PlayerBullet Boss层
                    cam.cullingMask &= ~(1 << 9);
                    cam.cullingMask &= ~(1 << 14);
                    cam.cullingMask &= ~(1 << 16);
                    cam.cullingMask &= ~(1 << 17);
                    if (!timeSkillCamera.activeInHierarchy)
                    {
                        timeSkillCamera.SetActive(true);
                    }
                    skill = Skill.TimeSkill;
                }
                else
                {
                    //主摄像恢复被剔除的层
                    cam.cullingMask |= (1 << 9);
                    cam.cullingMask |= (1 << 14);
                    cam.cullingMask |= (1 << 16);
                    cam.cullingMask |= (1 << 17);
                    timeSkillCamera.SetActive(false);

                    camera.GetComponent<PostProcessingBehaviour>().enabled = false;
                    skill = Skill.None;
                }
                if (timer >= timeSkillTimer)
                {
                    mask.fillAmount = 1;
                    etc.activated = true;
                    maskGameObject.SetActive(false);
                    timeSkillCD = false;
                    isChange = true;
                    //skillIcon = SkillIcon.None;
                    CD.text = "";
                    timer = 0;
                }
            }
        }
        if (skillIcon == SkillIcon.EyeSkill)
        {
            if (eyeSkillCD)
            {
                timer += Time.deltaTime;

                etc.activated = false;
                isChange = false;
                maskGameObject.SetActive(true);
                mask.fillAmount = (eyeSkillTimer - timer) / eyeSkillTimer;
                CD.text = ((int)(eyeSkillTimer - timer)).ToString();
                if (timer < eyeSkillTime)
                {
                    skill = Skill.EyeSkill;
                }
                else
                {
                    camera.GetComponent<PostProcessingBehaviour>().profile = Resources.Load("LightEffect") as PostProcessingProfile;
                    if (lightUI.isPointLight)
                    {
                        pointLight.SetActive(true);
                    }
                    else
                    {
                        spotLight.SetActive(true);
                    }
                    foreach (GameObject go in p.rooms)
                    {
                        go.GetComponent<SpriteRenderer>().material = p.smooth;
                    }
                    for (int i = 0; i < GameObject.Find("EnemyPool").transform.childCount; i++)
                    {
                        GameObject.Find("EnemyPool").transform.GetChild(i).GetComponent<SpriteRenderer>().material = p.shell;
                    }
                    skill = Skill.None;
                }
                if (timer >= eyeSkillTimer)
                {
                    mask.fillAmount = 1;
                    etc.activated = true;
                    maskGameObject.SetActive(false);
                    eyeSkillCD = false;
                    isChange = true;
                    //skillIcon = SkillIcon.None;
                    CD.text = "";
                    timer = 0;
                }
            }
        }
    }
}
