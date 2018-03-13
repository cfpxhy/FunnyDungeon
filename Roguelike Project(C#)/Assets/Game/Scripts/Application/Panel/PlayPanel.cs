using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayPanel : MonoBehaviour
{
    private Button playButton;
    private Button closeButton;
    private Button firstButton;
    private Button secondButton;
    private Button thirdButton;

    private Button secondLockButton;
    private Button thirdLockButton;

    private GameObject play;
    private GameObject playPanelBg;
    private GameObject promptText;

    private GameObject secondNormalFill;
    private GameObject secondLock;
    private GameObject secondText;
    private GameObject thirdNormalFill;
    private GameObject thirdLock;
    private GameObject thirdText;

    private bool isPromptTextShow = false;

    private int money;
    private int lockedCost = 1000;

    private int aniOffest = 150;

    private bool panelIsShow = false;
    void Start()
    {
        //查找
        play = GameObject.Find("Play");
        playButton = play.transform.Find("PlayButton").GetComponent<Button>();
        playPanelBg = GameObject.Find("PlayPanel").transform.Find("Bg").gameObject;
        closeButton = playPanelBg.transform.Find("CloseButton").GetComponent<Button>();
        firstButton = playPanelBg.transform.Find("FirstCard").GetComponent<Button>();
        secondButton = playPanelBg.transform.Find("SecondCard").GetComponent<Button>();
        thirdButton = playPanelBg.transform.Find("ThirdCard").GetComponent<Button>();
        secondLockButton = playPanelBg.transform.Find("SecondCard/Button").GetComponent<Button>();
        thirdLockButton = playPanelBg.transform.Find("ThirdCard/Button").GetComponent<Button>();

        promptText = playPanelBg.transform.Find("PromptText").gameObject;

        secondNormalFill = playPanelBg.transform.Find("SecondCard/NormalFill").gameObject;
        secondLock = playPanelBg.transform.Find("SecondCard/Lock").gameObject;
        secondText = playPanelBg.transform.Find("SecondCard/Text").gameObject;
        thirdNormalFill = playPanelBg.transform.Find("ThirdCard/NormalFill").gameObject;
        thirdLock = playPanelBg.transform.Find("ThirdCard/Lock").gameObject;
        thirdText = playPanelBg.transform.Find("ThirdCard/Text").gameObject;
        //加载存档
        money = Game.Instance.StaticData.TotalMoney;
        if (Game.Instance.StaticData.Level2Lock == 1)
        {
            secondLockButton.gameObject.SetActive(false);
            secondNormalFill.SetActive(true);
            secondLock.SetActive(false);
            secondText.SetActive(true);
            secondButton.interactable = true;
        }
        if (Game.Instance.StaticData.Level3Lock == 1)
        {
            thirdLockButton.gameObject.SetActive(false);
            thirdNormalFill.SetActive(true);
            thirdLock.SetActive(false);
            thirdText.SetActive(true);
            thirdButton.interactable = true;
        }
        //注册事件监听
        playButton.onClick.AddListener(delegate () { OnPlayClick(); });
        closeButton.onClick.AddListener(delegate () { OnCloseClick(); });
        firstButton.onClick.AddListener(delegate () { OnFirstCardClick(); });
        secondButton.onClick.AddListener(delegate () { OnSecondCardClick(); });
        thirdButton.onClick.AddListener(delegate () { OnThirdCardClick(); });
        secondLockButton.onClick.AddListener(delegate () { OnSecondLockClick(); });
        thirdLockButton.onClick.AddListener(delegate () { OnThirdLockClick(); });
    }

    public void OnPlayClick()
    {
        if (!panelIsShow)
        {
            playPanelBg.SetActive(true);
            play.transform.DOMoveX(play.transform.position.x + aniOffest, 0.5f);
            panelIsShow = true;
        }
    }

    public void OnCloseClick()
    {
        playPanelBg.SetActive(false);
        play.transform.DOMoveX(play.transform.position.x - aniOffest, 0.5f);
        panelIsShow = false;
    }

    public void OnFirstCardClick()
    {
        SceneManager.LoadScene(Game.Instance.StaticData.Level1);
    }

    public void OnSecondCardClick()
    {
        SceneManager.LoadScene(Game.Instance.StaticData.Level2);
    }

    public void OnThirdCardClick()
    {
        SceneManager.LoadScene(Game.Instance.StaticData.Level3);
    }

    public void OnSecondLockClick()
    {
        if (money >= lockedCost)
        {
            //money -= lockedCost;
            //Game.Instance.StaticData.TotalMoney = money;
            Game.Instance.StaticData.AddMoney(-lockedCost);
            Game.Instance.StaticData.Level2Lock = 1;
            Game.Instance.StaticData.UpdateGameInfo();
            secondLockButton.gameObject.SetActive(false);
            secondNormalFill.SetActive(true);
            secondLock.SetActive(false);
            secondText.SetActive(true);
            secondButton.interactable = true;
        }
        else
        {
            if (!isPromptTextShow)
            {
                StartCoroutine(PromptText("金币不足1000，无法解锁"));
            }
        }
    }

    public void OnThirdLockClick()
    {
        if (money >= lockedCost)
        {
            if (Game.Instance.StaticData.Level2Lock == 1)
            {
                //money -= lockedCost;
                //Game.Instance.StaticData.TotalMoney = money;
                Game.Instance.StaticData.AddMoney(-lockedCost);
                Game.Instance.StaticData.Level3Lock = 1;
                thirdLockButton.gameObject.SetActive(false);
                Game.Instance.StaticData.UpdateGameInfo();
                thirdNormalFill.SetActive(true);
                thirdLock.SetActive(false);
                thirdText.SetActive(true);
                thirdButton.interactable = true;
            }
            else
            {
                if (!isPromptTextShow)
                {
                    StartCoroutine(PromptText("请先解锁第二关"));
                }
            }
        }
        else
        {
            if (!isPromptTextShow)
            {
                StartCoroutine(PromptText("金币不足1000，无法解锁"));
            }
        }
    }

    IEnumerator PromptText(string s)
    {
        isPromptTextShow = true;
        promptText.SetActive(true);
        promptText.GetComponent<Text>().text = s;
        promptText.GetComponent<Text>().color = Color.red;
        yield return new WaitForSeconds(3.0f);
        promptText.GetComponent<Text>().DOFade(0, 2.0f);
        yield return new WaitForSeconds(2.0f);
        promptText.SetActive(false);
        isPromptTextShow = false;
    }
}