using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Sound))]
[RequireComponent(typeof(StaticData))]
public class Game : Singleton<Game>
{
    //全局访问功能
    [HideInInspector]
    public Sound Sound = null;//声音控制
    [HideInInspector]
    public StaticData StaticData = null;

    //游戏入口
    void Start()
    {
        //确保Game对象一直存在
        Object.DontDestroyOnLoad(this.gameObject);

        //全局单例赋值
        Sound = Sound.Instance;
        StaticData = StaticData.Instance;
        //载入上一次游戏的信息
        StaticData.SerializeGameInfo();
        //启动游戏
        SceneManager.LoadScene("1.Start");
    }
}
