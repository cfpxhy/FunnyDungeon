using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MapPanel : MonoBehaviour {

    private Text text;
    private GameObject mapPanel;
    private Button backButton;

    private Slider bgAudioSlider;
    private Slider effectAudioSlider;

    void Start()
    {
        mapPanel = transform.Find("MapPanel").gameObject;
        text = mapPanel.transform.Find("Text").GetComponent<Text>();

        bgAudioSlider = transform.Find("SettingPanel/BgAudioButton/BgAudioSlider").GetComponent<Slider>();
        effectAudioSlider = transform.Find("SettingPanel/EffectAudioButton/EffectAudioSlider").GetComponent<Slider>();

        backButton = mapPanel.transform.Find("BackButton").GetComponent<Button>();

        backButton.onClick.AddListener(delegate () { OnBackClick(); });
    }

    void Update()
    {
        if (mapPanel.activeInHierarchy)
        {
            StartCoroutine(Show());
        }
    }

    public void OnBackClick()
    {
        Game.Instance.StaticData.DeletePlayerInfo();
        Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
        Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
        Game.Instance.StaticData.AddMoney(Game.Instance.StaticData.Money);
        Game.Instance.StaticData.UpdateGameInfo();
        Game.Instance.StaticData.ResetPlayInfo();
        SceneManager.LoadScene(Game.Instance.StaticData.Main);
    }

    IEnumerator Show()
    {
        mapPanel.transform.DOScale(1, 1.0f);
        yield return new WaitForSeconds(1.0f);
        Tween tween = text.DOFade(1, 1.0f);
        tween.SetEase(Ease.Linear);
    }
}
