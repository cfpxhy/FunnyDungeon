using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : MonoBehaviour {

    private Button quitButton;

    void Start()
    {
        quitButton = transform.Find("QuitPanel/QuitButton").GetComponent<Button>();

        quitButton.onClick.AddListener(delegate () { OnQuitClick(); });
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}
