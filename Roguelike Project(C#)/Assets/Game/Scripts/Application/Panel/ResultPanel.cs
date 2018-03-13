using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour {

    private string filePath;

    private GameObject resultPanel;
    private Player p;

    private Button resetButton;
    private Button backButton;

    private Slider bgAudioSlider;
    private Slider effectAudioSlider;

    private Text resultMoney;

    private float timer = 0;

    void Start()
    {
        filePath = Application.persistentDataPath + "/Player.xml";

        p = GameObject.Find("Player").GetComponent<Player>();

        resultPanel = transform.Find("ResultPanel").gameObject;

        resetButton = resultPanel.transform.Find("ResetButton").GetComponent<Button>();
        backButton = resultPanel.transform.Find("BackButton").GetComponent<Button>();

        bgAudioSlider = transform.Find("SettingPanel/BgAudioButton/BgAudioSlider").GetComponent<Slider>();
        effectAudioSlider = transform.Find("SettingPanel/EffectAudioButton/EffectAudioSlider").GetComponent<Slider>();

        resultMoney = resultPanel.transform.Find("GetMoney/ResultMoney").GetComponent<Text>();

        resetButton.onClick.AddListener(delegate () { OnResetClick(); });
        backButton.onClick.AddListener(delegate () { OnBackClick(); });
    }

    void Update()
    {
        if (resultPanel.activeInHierarchy)
        {
            timer += Time.deltaTime;
            if (timer < 2f)
            {
                float res = Mathf.Lerp(0, Game.Instance.StaticData.Money, timer / 2);
                resultMoney.text = ((int)res).ToString();
                if (((int)res) >= Game.Instance.StaticData.Money * 0.9f)
                {
                    resultMoney.text = Game.Instance.StaticData.Money.ToString();
                }
            }
        }
    }

    public void OnResetClick()
    {
        Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
        Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
        p.SavePlayerInfo();
        if (!File.Exists(filePath))
        {
            Game.Instance.StaticData.SavePlayerInfo();
        }
        else
        {
            Game.Instance.StaticData.UpdatePlayerInfo();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnBackClick()
    {
        Game.Instance.StaticData.DeletePlayerInfo();
        Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
        Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
        Game.Instance.StaticData.AddMoney(Game.Instance.StaticData.Money);
        Game.Instance.StaticData.UpdateGameInfo();
        Game.Instance.StaticData.ResetPlayInfo();
        p.isJump = true;
        SceneManager.LoadScene(Game.Instance.StaticData.Main);
    }
}
