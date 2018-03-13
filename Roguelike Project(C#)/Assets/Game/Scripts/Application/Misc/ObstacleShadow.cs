using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleShadow : MonoBehaviour {

    private SpriteRenderer sr;
    private Transform player;
    private Player p;
    private LightUI lightUI;
    private SkillCD skillCD;
    //可视距离
    private int pointMaxSight;
    private int pointMinSight;
    private int spotMaxSight;
    private int spotMinSight;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        p = player.GetComponent<Player>();
        lightUI = GameObject.Find("Canvas").GetComponent<LightUI>();

        skillCD = GameObject.Find("SkillButton").GetComponent<SkillCD>();

        pointMaxSight = 80;
        pointMinSight = 20;
        spotMaxSight = 120;
        spotMinSight = 60;
    }

    void Update()
    {
        if (skillCD.skill != SkillCD.Skill.EyeSkill)
        {
            //根据亮度等级调整阴影可视距离
            switch (p.lightLevel)
            {
                case 0:
                    pointMaxSight = 80;
                    pointMinSight = 20;
                    spotMaxSight = 120;
                    spotMinSight = 70;
                    break;
                case 1:
                    pointMaxSight = 100;
                    pointMinSight = 30;
                    spotMaxSight = 150;
                    spotMinSight = 90;
                    break;
                case 2:
                    pointMaxSight = 120;
                    pointMinSight = 40;
                    spotMaxSight = 180;
                    spotMinSight = 110;
                    break;
            }
            //火把模式
            if (lightUI.isPointLight)
            {
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < pointMinSight)
                {
                    sr.color = new Color(1, 1, 1, 1);
                }
                //如果障碍物的距离和角色的距离在minSight到maxSight之间
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= pointMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= pointMinSight)
                {
                    //直线距离减maxSight的绝对值除以minSight
                    float a = (Mathf.Abs((Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) - pointMaxSight)) / (pointMaxSight - pointMinSight));
                    sr.color = new Color(1, 1, 1, a);
                }
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) > pointMaxSight)
                {
                    sr.color = new Color(1, 1, 1, 0);
                }
            }
            //手电筒模式
            else
            {
                switch (p.direction)
                {
                    case Player.Direction.Up:
                        Vector2 dirUp = transform.position - player.position;
                        if (Vector2.Angle(transform.up, dirUp) > 0 && Vector2.Angle(transform.up, dirUp) <= 45)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < spotMinSight)
                            {
                                sr.color = new Color(1, 1, 1, 1);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                //Debug.Log(Vector2.Angle(transform.up, dir));
                                //直线距离减maxSight的绝对值除以maxSight与minSight的差
                                float a = Mathf.Abs((Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) - spotMaxSight)) / (spotMaxSight - spotMinSight);
                                sr.color = new Color(1, 1, 1, a);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) > spotMaxSight)
                            {
                                sr.color = new Color(1, 1, 1, 0);
                            }
                        }
                        else if (Vector2.Angle(transform.up, dirUp) > 45 && Vector2.Angle(transform.up, dirUp) <= 60)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                float a = Mathf.Abs((Vector2.Angle(transform.up, dirUp) - 60) / (60 - 45));
                                sr.color = new Color(1, 1, 1, a);
                            }
                        }
                        else
                        {
                            sr.color = new Color(1, 1, 1, 0);
                        }
                        break;
                    case Player.Direction.Left:
                        Vector2 dirLeft = transform.position - player.position;
                        if (Vector2.Angle(-transform.right, dirLeft) > 0 && Vector2.Angle(-transform.right, dirLeft) <= 45)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < spotMinSight)
                            {
                                sr.color = new Color(1, 1, 1, 1);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                //Debug.Log(Vector2.Angle(transform.up, dir));
                                //直线距离减maxSight的绝对值除以maxSight与minSight的差
                                float a = Mathf.Abs((Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) - spotMaxSight)) / (spotMaxSight - spotMinSight);
                                sr.color = new Color(1, 1, 1, a);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) > spotMaxSight)
                            {
                                sr.color = new Color(1, 1, 1, 0);
                            }
                        }
                        else if (Vector2.Angle(-transform.right, dirLeft) > 45 && Vector2.Angle(-transform.right, dirLeft) <= 60)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                float a = Mathf.Abs((Vector2.Angle(-transform.right, dirLeft) - 60) / (60 - 45));
                                sr.color = new Color(1, 1, 1, a);
                            }
                        }
                        else
                        {
                            sr.color = new Color(1, 1, 1, 0);
                        }
                        break;
                    case Player.Direction.Down:
                        Vector2 dirDown = transform.position - player.position;
                        if (Vector2.Angle(-transform.up, dirDown) > 0 && Vector2.Angle(-transform.up, dirDown) <= 45)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < spotMinSight)
                            {
                                sr.color = new Color(1, 1, 1, 1);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                //Debug.Log(Vector2.Angle(transform.up, dir));
                                //直线距离减maxSight的绝对值除以maxSight与minSight的差
                                float a = Mathf.Abs((Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) - spotMaxSight)) / (spotMaxSight - spotMinSight);
                                sr.color = new Color(1, 1, 1, a);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) > spotMaxSight)
                            {
                                sr.color = new Color(1, 1, 1, 0);
                            }
                        }
                        else if (Vector2.Angle(-transform.up, dirDown) > 45 && Vector2.Angle(-transform.up, dirDown) <= 60)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                float a = Mathf.Abs((Vector2.Angle(-transform.up, dirDown) - 60) / (60 - 45));
                                sr.color = new Color(1, 1, 1, a);
                            }
                        }
                        else
                        {
                            sr.color = new Color(1, 1, 1, 0);
                        }
                        break;
                    case Player.Direction.Right:
                        Vector2 dirRight = transform.position - player.position;
                        if (Vector2.Angle(transform.right, dirRight) > 0 && Vector2.Angle(transform.right, dirRight) <= 45)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < spotMinSight)
                            {
                                sr.color = new Color(1, 1, 1, 1);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                //Debug.Log(Vector2.Angle(transform.up, dir));
                                //直线距离减maxSight的绝对值除以maxSight与minSight的差
                                float a = Mathf.Abs((Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) - spotMaxSight)) / (spotMaxSight - spotMinSight);
                                sr.color = new Color(1, 1, 1, a);
                            }
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) > spotMaxSight)
                            {
                                sr.color = new Color(1, 1, 1, 0);
                            }
                        }
                        else if (Vector2.Angle(transform.right, dirRight) > 45 && Vector2.Angle(transform.right, dirRight) <= 60)
                        {
                            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= spotMaxSight && Vector2.Distance(new Vector2(sr.transform.position.x, sr.transform.position.y), new Vector2(player.position.x, player.position.y)) >= spotMinSight)
                            {
                                float a = Mathf.Abs((Vector2.Angle(transform.right, dirRight) - 60) / (60 - 45));
                                sr.color = new Color(1, 1, 1, a);
                            }
                        }
                        else
                        {
                            sr.color = new Color(1, 1, 1, 0);
                        }
                        break;
                }
            }
        }
        else
        {
            sr.color = new Color(1, 1, 1, 1);
        }
    }
}
