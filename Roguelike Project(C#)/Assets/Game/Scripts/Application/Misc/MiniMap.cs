using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {

    private Button miniMapButton;
    private GameObject mapRoot;
    private GameObject cam;

    private bool isShow = false;

    void Start () {
        miniMapButton = GameObject.Find("MiniMap").GetComponent<Button>();
        mapRoot = GameObject.Find("MapRoot").gameObject;
        cam = GameObject.Find("MainCamera");
        mapRoot.transform.SetParent(cam.transform);
        mapRoot.SetActive(false);
        miniMapButton.onClick.AddListener(delegate () { OnClick(); });
    }
	
    public void OnClick()
    {
        if (!isShow)
        {
            mapRoot.SetActive(true);
            isShow = true;
        }
        else
        {
            mapRoot.SetActive(false);
            isShow = false;
        }
    }
}
