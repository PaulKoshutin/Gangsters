using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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


        if (!SaveLoader.Instance.loading)
        {
            orgs.Add(new("Slummers", "green", 500));
            orgs.Add(new("Hammers", "red", 1000));
            orgs.Add(new("Suits", "black", 2000));

            districts.Add(new("Northslum", new List<int>() { 20, 40, 5, 25, 5, 5 }, 20, 50, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0) }));
            districts.Add(new("Eastmouth", new List<int>() { 40, 20, 10, 20, 5, 5 }, 50, 80, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0) }));
            districts.Add(new("Westboro", new List<int>() { 50, 5, 20, 5, 10, 10 }, 80, 100, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0) }));
        }
    }
    public Org GetOrg(string name)
    {
        return orgs.Find(i => i.name == name);
    }
    public void Save()
    {
        string temp = JsonUtility.ToJson(this);
        Directory.CreateDirectory("Save");
        File.WriteAllText("Save/ActiveEntities.json", temp);
    }
    public void Load()
    {
        if (Directory.Exists("Save"))
        {
            string temp = File.ReadAllText("Save/ActiveEntities.json");
            JsonUtility.FromJsonOverwrite(temp, this);
        }
    }
}
