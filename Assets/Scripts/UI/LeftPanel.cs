using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
        icon.GetComponent<Image>().color = Color.white;
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
        if (org.destroyed)
            icon.GetComponent<Image>().color = Color.gray;
        info.GetComponent<TextMeshProUGUI>().text = "Gang: " + org.name + "\nLeader: " + org.active[0].name + "\nMoney: " + org.money;
        buttons.gameObject.SetActive(false);
    }
    public void Char(Char c)
    {
        Org org = ActiveEntities.Instance.GetOrg(c.org);
        this.c = c;
        icon.GetComponent<Image>().sprite = c.image;
        icon.GetComponent<Image>().color = Color.white;
        info.GetComponent<TextMeshProUGUI>().text = c.description + "\n\nOperates in " + c.district + " district";
        if (c.order != "")
            info.GetComponent<TextMeshProUGUI>().text += "\nOn mission to " + c.order + " " + c.orderTarget;
        buttons.gameObject.SetActive(true);
        if (org.player)
        {
            buttons.transform.GetChild(1).gameObject.SetActive(true);
            if (org.active.Contains(c) && (c.subordinates.Count > 0 || c.solo))
            {
                buttons.transform.GetChild(0).gameObject.SetActive(true);
                switch (c.strategy)
                {
                    case "": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 0; break;
                    case "Recruit": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 1; break;
                    case "Extort": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 2; break;
                    case "Punish": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 3; break;
                    case "Rob": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 4; break;
                    case "Patrol": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 5; break;
                    case "Hunt": buttons.transform.Find("Strategy Dropdown").GetComponent<TMP_Dropdown>().value = 6; break;
                }
                if (c.solo || c.squadLeader)
                    buttons.transform.GetChild(2).gameObject.SetActive(true);
                else
                    buttons.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                buttons.transform.GetChild(0).gameObject.SetActive(false);
                buttons.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        else
        {
            buttons.transform.GetChild(0).gameObject.SetActive(false);
            buttons.transform.GetChild(1).gameObject.SetActive(false);
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
        else if (val == 2)
            c.strategy = "Extort";
        else if (val == 3)
            c.strategy = "Punish";
        else if (val == 4)
            c.strategy = "Rob";
        else if (val == 5)
            c.strategy = "Patrol";
        else if (val == 6)
            c.strategy = "Hunt";
    }
    public void ChooseAction(int val)
    {
        if (val == 1)
        {
            c.Fire();
            Org();
        }
        else if (val == 2)
            c.Move("Northslum");
        else if (val == 3)
            c.Move("Eastmouth");
        else if (val == 4)
            c.Move("Westboro");
        
    }
    public void OpenOrderPanel()
    {
        transform.parent.Find("Main Panel (Stuff)").Find("City Panel").gameObject.SetActive(false);
        transform.parent.Find("Main Panel (Stuff)").Find("Gang Panel").gameObject.SetActive(false);
        transform.parent.Find("Main Panel (Stuff)").Find("Order Panel").gameObject.SetActive(true);
        if (ActiveEntities.Instance.GetOrg(c.org).player)
            transform.parent.Find("Main Panel (Stuff)").Find("Order Panel").GetComponent<OrderPanel>().SetExecutor(c);
        else
            transform.parent.Find("Main Panel (Stuff)").Find("Order Panel").GetComponent<OrderPanel>().SetTarget(c);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Org();
    }
}
