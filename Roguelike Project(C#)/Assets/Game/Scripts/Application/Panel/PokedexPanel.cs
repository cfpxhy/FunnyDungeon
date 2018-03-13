using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PokedexPanel : MonoBehaviour {

    private string path = "Game/Res/";
    private Transform pokedexPanel;

    private GameObject pokedexPanelBg;
    private GameObject pokedex;

    private Button pokedexButton;
    private Button closeButton;

    private Toggle enemyToggle;
    private Toggle itemToggle;

    private GameObject enemyScroll;
    private GameObject itemScroll;
    private GameObject enemyInfo;
    private GameObject itemInfo;

    private Button[] enemyButton = new Button[8];
    private Button[] itemButton = new Button[7];

    private Text[] enemyText = new Text[5];
    private Text[] itemText = new Text[4];

    private List<string> enemyName = new List<string>();
    private List<string> enemyHp = new List<string>();
    private List<string> enemySpeed = new List<string>();
    private List<string> enemyMoney = new List<string>();
    private List<string> enemyRemark = new List<string>();

    private List<string> itemName = new List<string>();
    private List<string> itemEffect = new List<string>();
    private List<string> itemLimit = new List<string>();
    private List<string> itemType = new List<string>();

    private bool panelIsShow = false;

    private int aniOffest = 150;
    void Start()
    {
        SerializeEnemyDetail();
        SerializeItemDetail();

        pokedexPanel = transform.Find("PokedexPanel");
        pokedexPanelBg = pokedexPanel.Find("Bg").gameObject;
        pokedex = pokedexPanel.Find("Pokedex").gameObject;

        pokedexButton = pokedexPanel.Find("Pokedex/PokedexButton").GetComponent<Button>();
        closeButton = pokedexPanel.Find("Bg/CloseButton").GetComponent<Button>();

        enemyToggle = pokedexPanel.Find("Bg/Select/Enemy").GetComponent<Toggle>();
        itemToggle = pokedexPanel.Find("Bg/Select/Item").GetComponent<Toggle>();

        enemyScroll = pokedexPanel.Find("Bg/EnemyScroll").gameObject;
        itemScroll = pokedexPanel.Find("Bg/ItemScroll").gameObject;
        enemyInfo = pokedexPanel.Find("Bg/EnemyInfo").gameObject;
        itemInfo = pokedexPanel.Find("Bg/ItemInfo").gameObject;

        pokedexButton.onClick.AddListener(delegate () { OnPokedexClick(); });
        closeButton.onClick.AddListener(delegate () { OnCloseClick(); });

        for (int i = 0; i < enemyButton.Length; i++)
        {
            enemyButton[i] = enemyScroll.transform.Find("EnemyGrid").GetChild(i).GetChild(0).GetComponent<Button>();
            //enemyButton[i].onClick.AddListener(delegate () { OnEnemyClick(i); });
        }
        for (int i = 0; i < itemButton.Length; i++)
        {
            itemButton[i] = itemScroll.transform.Find("ItemGrid").GetChild(i).GetChild(0).GetComponent<Button>();
        }
        for (int i = 0; i < enemyText.Length; i++)
        {
            enemyText[i] = pokedexPanelBg.transform.Find("EnemyInfo").GetChild(i).GetChild(0).GetComponent<Text>();
        }
        for (int i = 0; i < itemText.Length; i++)
        {
            itemText[i] = pokedexPanelBg.transform.Find("ItemInfo").GetChild(i).GetChild(0).GetComponent<Text>();
        }

        enemyButton[0].onClick.AddListener(delegate () { OnEnemyClick(0); });
        enemyButton[1].onClick.AddListener(delegate () { OnEnemyClick(1); });
        enemyButton[2].onClick.AddListener(delegate () { OnEnemyClick(2); });
        enemyButton[3].onClick.AddListener(delegate () { OnEnemyClick(3); });
        enemyButton[4].onClick.AddListener(delegate () { OnEnemyClick(4); });
        enemyButton[5].onClick.AddListener(delegate () { OnEnemyClick(5); });
        enemyButton[6].onClick.AddListener(delegate () { OnEnemyClick(6); });
        enemyButton[7].onClick.AddListener(delegate () { OnEnemyClick(7); });

        itemButton[0].onClick.AddListener(delegate () { OnItemClick(0); });
        itemButton[1].onClick.AddListener(delegate () { OnItemClick(1); });
        itemButton[2].onClick.AddListener(delegate () { OnItemClick(2); });
        itemButton[3].onClick.AddListener(delegate () { OnItemClick(3); });
        itemButton[4].onClick.AddListener(delegate () { OnItemClick(4); });
        itemButton[5].onClick.AddListener(delegate () { OnItemClick(5); });
        itemButton[6].onClick.AddListener(delegate () { OnItemClick(6); });
    }

    void Update()
    {
        if (enemyToggle.isOn)
        {
            enemyScroll.SetActive(true);
            itemScroll.SetActive(false);
        }
        else
        {
            enemyScroll.SetActive(false);
            itemScroll.SetActive(true);
        }
    }

    public void OnPokedexClick()
    {
        if (!panelIsShow)
        {
            pokedexPanelBg.SetActive(true);
            pokedex.transform.DOMoveX(pokedex.transform.position.x + aniOffest, 0.5f);
            panelIsShow = true;
        }
    }

    public void OnCloseClick()
    {
        pokedexPanelBg.SetActive(false);
        pokedex.transform.DOMoveX(pokedex.transform.position.x - aniOffest, 0.5f);
        panelIsShow = false;
    }

    #region 图鉴事件监听
    public void OnEnemyClick(int i)
    {
        if (itemInfo.activeInHierarchy)
        {
            itemInfo.SetActive(false);
        }
        enemyInfo.SetActive(true);
        enemyText[0].text = enemyName[i];
        enemyText[1].text = enemyHp[i];
        enemyText[2].text = enemySpeed[i];
        enemyText[3].text = enemyMoney[i];
        enemyText[4].text = enemyRemark[i];
    }

    public void OnItemClick(int i)
    {
        if (enemyInfo.activeInHierarchy)
        {
            enemyInfo.SetActive(false);
        }
        itemInfo.SetActive(true);
        itemText[0].text = itemName[i];
        itemText[1].text = itemEffect[i];
        itemText[2].text = itemLimit[i];
        itemText[3].text = itemType[i];
    }
    #endregion

    #region 解析
    private void SerializeEnemyDetail()
    {
        XmlDocument enemyDetail = new XmlDocument();
        enemyDetail.LoadXml(Resources.Load<TextAsset>(path + "EnemyDetail").text);
        XmlNode rootNode = enemyDetail.SelectSingleNode("EnemyDetail");
        XmlNodeList enemyNodeList = rootNode.ChildNodes;
        foreach (XmlNode enemyNode in enemyNodeList)
        {
            switch (enemyNode.Name)
            {
                case "Grievance":
                    XmlNodeList detailNodeList0 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList0)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Move":
                    XmlNodeList detailNodeList1 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList1)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Doubt":
                    XmlNodeList detailNodeList2 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList2)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Cool":
                    XmlNodeList detailNodeList3 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList3)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Sad":
                    XmlNodeList detailNodeList4 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList4)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Angry":
                    XmlNodeList detailNodeList5 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList5)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Sinister":
                    XmlNodeList detailNodeList6 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList6)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Boss":
                    XmlNodeList detailNodeList7 = enemyNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList7)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                enemyName.Add(detailNode.InnerText);
                                break;
                            case "Hp":
                                enemyHp.Add(detailNode.InnerText);
                                break;
                            case "Speed":
                                enemySpeed.Add(detailNode.InnerText);
                                break;
                            case "Money":
                                enemyMoney.Add(detailNode.InnerText);
                                break;
                            case "Remark":
                                enemyRemark.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
            }
        }
    }

    private void SerializeItemDetail()
    {
        XmlDocument itemDetail = new XmlDocument();
        itemDetail.LoadXml(Resources.Load<TextAsset>(path + "ItemDetail").text);
        XmlNode rootNode = itemDetail.SelectSingleNode("ItemDetail");
        XmlNodeList itemNodeList = rootNode.ChildNodes;
        foreach (XmlNode itemNode in itemNodeList)
        {
            switch (itemNode.Name)
            {
                case "BluePotion":
                    XmlNodeList detailNodeList0 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList0)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Shoes":
                    XmlNodeList detailNodeList1 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList1)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "PinkPotion":
                    XmlNodeList detailNodeList2 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList2)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "ColourPotion":
                    XmlNodeList detailNodeList3 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList3)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Clock":
                    XmlNodeList detailNodeList4 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList4)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Bulb":
                    XmlNodeList detailNodeList5 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList5)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
                case "Paper":
                    XmlNodeList detailNodeList6 = itemNode.ChildNodes;
                    foreach (XmlNode detailNode in detailNodeList6)
                    {
                        switch (detailNode.Name)
                        {
                            case "Name":
                                itemName.Add(detailNode.InnerText);
                                break;
                            case "Effect":
                                itemEffect.Add(detailNode.InnerText);
                                break;
                            case "Limit":
                                itemLimit.Add(detailNode.InnerText);
                                break;
                            case "Type":
                                itemType.Add(detailNode.InnerText);
                                break;
                        }
                    }
                    break;
            }
        }
    }
    #endregion
}
