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
        info.GetComponent<TextMeshProUGUI>().text = "Gang: " + org.name + "\nLeader: " + org.leader.name + "\nMoney: " + org.money;
        buttons.gameObject.SetActive(false);
    }
    public void Char(Char c)
    {
        icon.GetComponent<Image>().sprite = c.image;
        info.GetComponent<TextMeshProUGUI>().text = "Name: " + c.name + "\nGang: " + c.org + "\nUpkeep: " + c.pay + "\nMental: " + c.mental + "\nSocial: " + c.social + "\nPhysical: " + c.physical;
        buttons.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Org();
    }
}
