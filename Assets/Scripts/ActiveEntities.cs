using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveEntities : MonoBehaviour
{
    public static ActiveEntities Instance { get; private set; }
    public List<Org> orgs = new List<Org>();
    public List<District> districts = new List<District>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        orgs.Add(new("Slummers", "green", 500));
        orgs.Add(new("Hammers", "red", 1000));
        orgs.Add(new("Suits", "black", 2000));


        districts.Add(new("Northslum", new List<int>() { 20, 40, 5, 25, 5, 5}, 20, 50, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0) }));
        districts.Add(new("Eastmouth", new List<int>() { 40, 20, 10, 20, 5, 5}, 50, 80, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0) }));
        districts.Add(new("Westboro", new List<int>() { 50, 5, 20, 5, 10, 10}, 80, 100, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0) }));

    }
}
