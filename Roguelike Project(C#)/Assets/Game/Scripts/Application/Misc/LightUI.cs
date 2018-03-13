using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUI : MonoBehaviour {

    private GameObject pointLight;
    private GameObject spotLight;
    [HideInInspector]
    public bool isPointLight = true;

    void Start()
    {
        pointLight = GameObject.Find("PlayerLight").transform.Find("PointLight").gameObject;
        spotLight = GameObject.Find("PlayerLight").transform.Find("SpotLight").gameObject;
    }

    public void OnClick()
    {
        if (isPointLight)
        {
            pointLight.SetActive(false);
            spotLight.SetActive(true);
            isPointLight = false;
        }
        else
        {
            pointLight.SetActive(true);
            spotLight.SetActive(false);
            isPointLight = true;
        }
    }
}
