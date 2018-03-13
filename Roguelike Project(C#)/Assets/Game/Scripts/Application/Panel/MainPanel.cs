using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainPanel : MonoBehaviour {

    private int money;
    private Text moneyText;
    private Button backButton;

    private Slider bgAudioSlider;
    private Slider effectAudioSlider;

    void Start () {
        money = Game.Instance.StaticData.TotalMoney;
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        moneyText.text = money.ToString();

        backButton = transform.Find("MainPanel/BackButton").GetComponent<Button>();

        backButton.onClick.AddListener(delegate () { OnBackClick(); });

        bgAudioSlider = transform.Find("SettingPanel/Setting/BgAudioButton/BgAudioSlider").GetComponent<Slider>();
        effectAudioSlider = transform.Find("SettingPanel/Setting/EffectAudioButton/EffectAudioSlider").GetComponent<Slider>();

        Game.Instance.Sound.PlayBg("GameBg");
    }

    void Update()
    {
        money = Game.Instance.StaticData.TotalMoney;
        moneyText.text = money.ToString();
    }

    public void OnBackClick()
    {
        SceneManager.LoadScene("1.Start");
    }
}
