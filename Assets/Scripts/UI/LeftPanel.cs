using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour
{
    public static LeftPanel Instance { get; private set; }
    private Transform icon;
    private Transform info;
    private Transform buttons;
    private Char c;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
        private void Start()
    {
        transform.parent.Find("Main Panel (Stuff)").Find("City Panel").Find("City Gangs Panel").Find("Gang0").GetComponent<Image>().sprite = ActiveEntities.Instance.orgs[0].emblem;
        icon = transform.Find("Icon");
        info = transform.Find("Info");
        buttons = transform.Find("Gangster Buttons Panel");
        Org();
    }
    public void CityDistrict(string name)
    {
        District district = ActiveEntities.Instance.GetDistrict(name);
        icon.GetComponent<Image>().sprite = district.view;
        info.GetComponent<TextMeshProUGUI>().text = "District: " + district.name + "\nWealth: " + district.wealth + "\nNumber of businesses: " + district.number_of_businesses + "\nDistrict control: " + district.GetControlData();
        buttons.gameObject.SetActive(false);
    }
    public void Org(string name="")
    {
        Org org;
        if (name == "")
            org = ActiveEntities.Instance.orgs[0];
        else
            org = ActiveEntities.Instance.GetOrg(name);
        icon.GetComponent<Image>().sprite = org.emblem;
        info.GetComponent<TextMeshProUGUI>().text = "Gang: " + org.name + "\nLeader: " + org.active[0].name + "\nMoney: " + org.money;
        buttons.gameObject.SetActive(false);
    }
    public void Char(Char c)
    {
        this.c = c;
        icon.GetComponent<Image>().sprite = c.image;
        info.GetComponent<TextMeshProUGUI>().text = c.description;
        buttons.gameObject.SetActive(true);
        if (c.org == ActiveEntities.Instance.orgs[0].name)
        {
            switch (c.strategy)
            {
                case "Recruit": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 1; break;
            }
            switch (c.order)
            {
                case "Kill": buttons.transform.Find("Order Dropdown").GetComponent<TMP_Dropdown>().value = 1; break;
            }
        }
        else
        {
            buttons.transform.GetChild(0).gameObject.SetActive(false);
            switch (c.order)
            {
                case "Kill": buttons.transform.Find("Order Dropdown").GetComponent<TMP_Dropdown>().value = 1; break;
            }
        }

    }
    public void ChooseStrategy(int val)
    {
        if (val == 0)
            c.strategy = "";
        else if (val == 1)
            c.strategy = "Recruit";
    }
    public void ChooseOrder(int val)
    {
        if (val == 0)
            c.order = "";
        else if (val == 1)
        {
            c.order = "Kill";
            OpenOrderPanel();
        }
    }
    public void OpenOrderPanel()
    {
        transform.parent.Find("Main Panel (Stuff)").Find("Order Panel").gameObject.SetActive(true);
        transform.parent.Find("Main Panel (Stuff)").Find("City Panel").gameObject.SetActive(false);
        transform.parent.Find("Main Panel (Stuff)").Find("Char Panel").gameObject.SetActive(false);
        transform.parent.Find("Main Panel (Stuff)").Find("Gang Panel").gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Org();
    }
}
