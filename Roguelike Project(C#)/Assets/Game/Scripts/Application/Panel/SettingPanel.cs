using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour {

    private string filePath;

    private GameObject settingPanel;

    private Player p;
    private Button settingButton;
    private Button closeButton;
    private Button resetButton;
    private Button backButton;
    private Button bgAudioButton;
    private Button effectAudioButton;

    private Slider bgAudioSlider;
    private Slider effectAudioSlider;

    private float lastBgVolume;
    private float lastEffectVolume;

    private bool isPanelShow = false;
    //是否静音
    private bool isBgMute = false;
    private bool isEffectMute = false;

    void Start () {
		if (SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level1 || SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level2 || SceneManager.GetActiveScene().name == Game.Instance.StaticData.Level3)
        {
            filePath = Application.persistentDataPath + "/Player.xml";
            p = GameObject.Find("Player").GetComponent<Player>();
            settingButton = GameObject.Find("SettingButton").GetComponent<Button>();
            settingPanel = transform.Find("SettingPanel").gameObject;
            resetButton = settingPanel.transform.Find("ResetButton").GetComponent<Button>();
            backButton = settingPanel.transform.Find("BackButton").GetComponent<Button>();

            resetButton.onClick.AddListener(delegate () { OnResetClick(); });
            backButton.onClick.AddListener(delegate () { OnBackClick(); });
        }
        else if (SceneManager.GetActiveScene().name == "1.Start" || SceneManager.GetActiveScene().name == Game.Instance.StaticData.Main)
        {
            settingButton = transform.Find("SettingPanel/SettingButton").GetComponent<Button>();
            settingPanel = transform.Find("SettingPanel/Setting").gameObject;
        }
        closeButton = settingPanel.transform.Find("CloseButton").GetComponent<Button>();
        bgAudioButton = settingPanel.transform.Find("BgAudioButton").GetComponent<Button>();
        effectAudioButton = settingPanel.transform.Find("EffectAudioButton").GetComponent<Button>();
        bgAudioSlider = bgAudioButton.transform.Find("BgAudioSlider").GetComponent<Slider>();
        effectAudioSlider = effectAudioButton.transform.Find("EffectAudioSlider").GetComponent<Slider>();

        bgAudioSlider.value = Game.Instance.StaticData.BgVolume;
        effectAudioSlider.value = Game.Instance.StaticData.EffectVolume;

        settingButton.onClick.AddListener(delegate () { OnSettingClick(); });
        closeButton.onClick.AddListener(delegate () { OnCloseClick(); });
        bgAudioButton.onClick.AddListener(delegate () { OnBgAudioClick(); });
        effectAudioButton.onClick.AddListener(delegate () { OnEffectAudioClick(); });
    }

    void Update()
    {
        Game.Instance.Sound.BgVolume = bgAudioSlider.value;
        Game.Instance.Sound.EffectVolume = effectAudioSlider.value;
    }

    public void OnSettingClick()
    {
        if (!isPanelShow)
        {
            settingPanel.SetActive(true);
            isPanelShow = true;
        }
    }

    public void OnCloseClick()
    {
        if (SceneManager.GetActiveScene().name != "1.Start")
        {
            Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
            Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
            Game.Instance.StaticData.UpdateGameInfo();
        }
        settingPanel.SetActive(false);
        isPanelShow = false;
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
        Game.Instance.StaticData.UpdateGameInfo();
        Game.Instance.StaticData.ResetPlayInfo();
        p.isJump = true;
        SceneManager.LoadScene(Game.Instance.StaticData.Main);
    }

    public void OnBgAudioClick()
    {
        if (!isBgMute)
        {
            if (bgAudioSlider.value != 0)
            {
                lastBgVolume = bgAudioSlider.value;
                bgAudioSlider.value = 0;
                isBgMute = true;
            }
        }
        else
        {
            bgAudioSlider.value = lastBgVolume;
            isBgMute = false;
        }
    }

    public void OnEffectAudioClick()
    {
        if (!isEffectMute)
        {
            if (effectAudioSlider.value != 0)
            {
                lastEffectVolume = effectAudioSlider.value;
                effectAudioSlider.value = 0;
                isEffectMute = true;
            }
        }
        else
        {
            effectAudioSlider.value = lastEffectVolume;
            isEffectMute = false;
        }
    }
}
