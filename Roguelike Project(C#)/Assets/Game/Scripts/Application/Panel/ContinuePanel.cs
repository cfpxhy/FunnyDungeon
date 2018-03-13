using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ContinuePanel : MonoBehaviour {

    private Slider bgAudioSlider;
    private Slider effectAudioSlider;

    private Button continueButton;
    private GameObject promptText;

    private bool isTextShow = false;

    void Start()
    {
        continueButton = transform.Find("ContinuePanel/Continue").GetComponent<Button>();
        promptText = transform.Find("ContinuePanel/PromptText").gameObject;

        bgAudioSlider = transform.Find("SettingPanel/Setting/BgAudioButton/BgAudioSlider").GetComponent<Slider>();
        effectAudioSlider = transform.Find("SettingPanel/Setting/EffectAudioButton/EffectAudioSlider").GetComponent<Slider>();

        continueButton.onClick.AddListener(delegate () { OnContinueClick(); });
    }

    public void OnContinueClick()
    {
        if (!File.Exists(Application.persistentDataPath + "/Game.xml"))
        {
            if (!isTextShow)
            {
                StartCoroutine(PromptText());
            }
        }
        else
        {
            Game.Instance.StaticData.BgVolume = bgAudioSlider.value;
            Game.Instance.StaticData.EffectVolume = effectAudioSlider.value;
            Game.Instance.StaticData.UpdateGameInfo();
            SceneManager.LoadScene(Game.Instance.StaticData.Main);
        }
    }

    IEnumerator PromptText()
    {
        isTextShow = true;
        promptText.SetActive(true);
        promptText.GetComponent<Text>().color = Color.red;
        yield return new WaitForSeconds(3.0f);
        promptText.GetComponent<Text>().DOFade(0, 2.0f);
        yield return new WaitForSeconds(2.0f);
        promptText.SetActive(false);
        isTextShow = false;
    }
}
