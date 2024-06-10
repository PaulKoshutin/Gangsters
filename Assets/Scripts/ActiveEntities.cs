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
            orgs.Add(Generator.Instance.o);
            orgs.Add(new("Slummers", "green_baseball_cap", 500));
            orgs.Add(new("Hammers", "red_hair", 1000));
            orgs.Add(new("Suits", "black_bowtie", 2000));

            orgs[0].AddToActive(orgs[0].active[0]);
            orgs[1].AddToActive(new ("gangster","Northslum", "Slummers", "James LeThrone", Resources.Load("Sprites/James LeThrone", typeof(Sprite)) as Sprite,40,60,80,0,false,true));
            orgs[1].AddToActive(orgs[1].active[0]);
            orgs[2].AddToActive(new("gangster", "Eastmouth", "Hammers", "Lo Qian Ye", Resources.Load("Sprites/Lo Qian Ye", typeof(Sprite)) as Sprite, 90, 70, 50, 0, false, true));
            orgs[2].AddToActive(orgs[2].active[0]);
            orgs[3].AddToActive(new("gangster", "Westboro", "Suits", "Howard Phillips Jr.", Resources.Load("Sprites/Howard Phillips Jr.", typeof(Sprite)) as Sprite, 80, 100, 60, 0, false, true));
            orgs[3].AddToActive(orgs[3].active[0]);

            foreach (Org o in orgs)
                o.InitiatePolicies();

            districts.Add(new("Northslum", new List<int>() { 15, 40, 5, 30, 5, 5 }, 5, 20, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0), new OrgValuePair(orgs[3], 0) }));
            districts.Add(new("Eastmouth", new List<int>() { 20, 20, 30, 20, 5, 5 }, 10, 40, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0), new OrgValuePair(orgs[3], 0) }));
            districts.Add(new("Westboro", new List<int>() { 60, 5, 10, 5, 10, 10 }, 20, 80, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0), new OrgValuePair(orgs[3], 0) }));
        }
    }
    private void FixedUpdate()
    {
        foreach (Org o in orgs)
        {
            foreach (Char c in o.active)
            {
                if (c.strategy != "" && c.subordinates.Count > 0 && !c.squadLeader)
                    foreach (string sub in c.subordinates)
                        o.GetActive(sub).strategy = c.strategy;
                else if (c.subordinates.Count == 0 && !c.solo)
                    c.strategy = "";
                else if (c.order != "")
                {
                    //StartCoroutine(c.StartDialogue(dialogue));
                }
                else if (c.order == "" && c.strategy != "")
                {
                    if (GetOrg(c.org).player)
                    {
                        Strategy strategy = new PlayerGangsterStrategy();
                        strategy.Prepare(c);
                    }
                }
            }
        }
    }
    public Org GetOrg(string name)
    {
        return orgs.Find(i => i.name == name);
    }
    public District GetDistrict(string name)
    {
        return districts.Find(i => i.name == name);
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
