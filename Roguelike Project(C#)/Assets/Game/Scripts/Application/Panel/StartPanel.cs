using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class StartPanel : MonoBehaviour {

    private Transform huaJi;
    private GameObject startPanel;

    private Button startButton;
    private Button yesButton;
    private Button noButton;

    private Slider bgAudioSlider;
    private Slider effectAudioSlider;

    private bool isPanelShow = false;

    void Start () {
        huaJi = transform.Find("HuaJi");
        startPanel = transform.Find("StartPanel/Start/Panel").gameObject;
        startButton = GameObject.Find("Start").GetComponent<Button>();
        yesButton = startPanel.transform.Find("YesButton").GetComponent<Button>();
        noButton = startPanel.transform.Find("NoButton").GetComponent<Button>();

        startButton.onClick.AddListener(delegate () { OnStartClick(); });
        yesButton.onClick.AddListener(delegate () { OnYesCilck(); });
        noButton.onClick.AddListener(delegate () { OnNoCilck(); });

        bgAudioSlider = transform.Find("SettingPanel/Setting/BgAudioButton/BgAudioSlider").GetComponent<Slider>();
        effectAudioSlider = transform.Find("SettingPanel/Setting/EffectAudioButton/EffectAudioSlider").GetComponent<Slider>();

        bgAudioSlider.value = Game.Instance.StaticData.BgVolume;
        effectAudioSlider.value = Game.Instance.StaticData.EffectVolume;

        Game.Instance.Sound.PlayBg("GameBg");
    }

    void Update()
    {
        huaJi.Rotate(Vector3.forward * 80 * Time.deltaTime, Space.World);
    }

    public void OnStartClick()
    {
        string filePath = Application.persistentDataPath + "/Game.xml";
        if (!File.Exists(filePath))
        {
            Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
            Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
            Game.Instance.StaticData.SaveGameInfo();
            SceneManager.LoadScene(Game.Instance.StaticData.Main);
        }
        else
        {
            if (!isPanelShow)
            {
                startPanel.SetActive(true);
                isPanelShow = true;
            }
        }
    }

    public void OnYesCilck()
    {
        string filePath = Application.persistentDataPath + "/Game.xml";
        Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
        Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
        if (!File.Exists(filePath))
        {
            Game.Instance.StaticData.SaveGameInfo();
        }
        else
        {
            Game.Instance.StaticData.ResetGameInfo();
            Game.Instance.StaticData.UpdateGameInfo();
        }
        SceneManager.LoadScene(Game.Instance.StaticData.Main);
    }

    public void OnNoCilck()
    {
        startPanel.SetActive(false);
        isPanelShow = false;
    }
}
