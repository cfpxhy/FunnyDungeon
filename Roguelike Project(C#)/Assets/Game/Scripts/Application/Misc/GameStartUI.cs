using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameStartUI : MonoBehaviour {
    Text startText;
    int layerCount;
    void Start()
    {
        startText = GameObject.Find("StartUI").GetComponent<Text>();
        layerCount = Game.Instance.StaticData.LayerCount;
        switch (layerCount)
        {
            case 1:
                startText.text = "第一层";
                break;
            case 2:
                startText.text = "第二层";
                break;
            case 3:
                startText.text = "第三层";
                break;
            case 4:
                startText.text = "第四层";
                break;
            case 5:
                startText.text = "第五层";
                break;
        }
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        startText.DOFade(1, 0.5f);
        GameObject go = Resources.Load("Game/Prefabs/Effect/MagicCircle") as GameObject;
        Instantiate(go, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        startText.DOFade(0, 0.5f);
        yield return new WaitForSeconds(1.0f);
        startText.gameObject.SetActive(false);
    }
}
