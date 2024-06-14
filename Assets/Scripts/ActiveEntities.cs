using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ActiveEntities : MonoBehaviour
{
    public static ActiveEntities Instance { get; private set; }
    public List<Org> orgs = new List<Org>();
    public List<District> districts = new List<District>();
    public List<Char> patrols = new List<Char>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;


        if (!SaveLoader.Instance.loading)
        {
            /*
            orgs.Add(Generator.Instance.o);
            orgs.Add(new("Slummers", "green_baseball_cap", 3000));
            //orgs.Add(new("Hammers", "red_hair", 4000));
            //orgs.Add(new("Suits", "black_bowtie", 5000));

            orgs[0].AddToActive(orgs[0].active[0]);
            orgs[1].AddToActive(new ("gangster","Northslum", "Slummers", "James LeThrone", Resources.Load("Sprites/James LeThrone", typeof(Sprite)) as Sprite,40,60,80,0,false,true, "Recruit"));
            orgs[1].AddToActive(orgs[1].active[0]);
            //orgs[2].AddToActive(new("gangster", "Eastmouth", "Hammers", "Lo Qian Ye", Resources.Load("Sprites/Lo Qian Ye", typeof(Sprite)) as Sprite, 90, 70, 50, 0, false, true, "Recruit"));
            //orgs[2].AddToActive(orgs[2].active[0]);
            //orgs[3].AddToActive(new("gangster", "Westboro", "Suits", "Howard Phillips Jr.", Resources.Load("Sprites/Howard Phillips Jr.", typeof(Sprite)) as Sprite, 80, 100, 60, 0, false, true, "Recruit"));
            //orgs[3].AddToActive(orgs[3].active[0]);

            foreach (Org o in orgs)
                o.InitiatePolicies();

            districts.Add(new("Northslum", new List<int>() { 15, 40, 5, 30, 5, 5 }, 60, 5, 20, new List<OrgValuePair>() { new OrgValuePair(orgs[0].name, 0), new OrgValuePair(orgs[1].name, 0)}));
            //districts.Add(new("Eastmouth", new List<int>() { 20, 20, 30, 20, 5, 5 }, 40, 10, 40, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0), new OrgValuePair(orgs[3], 0) }));
            //districts.Add(new("Westboro", new List<int>() { 60, 5, 10, 5, 10, 10 }, 20, 20, 80, new List<OrgValuePair>() { new OrgValuePair(orgs[0], 0), new OrgValuePair(orgs[1], 0), new OrgValuePair(orgs[2], 0), new OrgValuePair(orgs[3], 0) }));
            */

            orgs.Add(Generator.Instance.o);
            orgs.Add(new("Slummers", "green_baseball_cap", 3000));
            orgs.Add(new("Hammers", "red_hair", 4000));
            orgs.Add(new("Suits", "black_bowtie", 5000));

            orgs[0].AddToActive(orgs[0].active[0]);
            orgs[1].AddToActive(new("gangster", "Northslum", "Slummers", "James LeThrone", Resources.Load("Sprites/James LeThrone", typeof(Sprite)) as Sprite, 40, 60, 80, 0, false, true, "Recruit"));
            orgs[1].AddToActive(orgs[1].active[0]);
            orgs[2].AddToActive(new("gangster", "Eastmouth", "Hammers", "Lo Qian Ye", Resources.Load("Sprites/Lo Qian Ye", typeof(Sprite)) as Sprite, 90, 70, 50, 0, false, true, "Recruit"));
            orgs[2].AddToActive(orgs[2].active[0]);
            orgs[3].AddToActive(new("gangster", "Westboro", "Suits", "Howard Phillips Jr.", Resources.Load("Sprites/Howard Phillips Jr.", typeof(Sprite)) as Sprite, 80, 100, 60, 0, false, true, "Recruit"));
            orgs[3].AddToActive(orgs[3].active[0]);

            foreach (Org o in orgs)
                o.InitiatePolicies();

            districts.Add(new("Northslum", new List<int>() { 15, 40, 5, 30, 5, 5 }, 60, 5, 20, new List<OrgValuePair>() { new OrgValuePair(orgs[0].name, 0), new OrgValuePair(orgs[1].name, 0), new OrgValuePair(orgs[2].name, 0), new OrgValuePair(orgs[3].name, 0) }));
            districts.Add(new("Eastmouth", new List<int>() { 20, 20, 30, 20, 5, 5 }, 40, 10, 40, new List<OrgValuePair>() { new OrgValuePair(orgs[0].name, 0), new OrgValuePair(orgs[1].name, 0), new OrgValuePair(orgs[2].name, 0), new OrgValuePair(orgs[3].name, 0) }));
            districts.Add(new("Westboro", new List<int>() { 60, 5, 10, 5, 10, 10 }, 20, 20, 80, new List<OrgValuePair>() { new OrgValuePair(orgs[0].name, 0), new OrgValuePair(orgs[1].name, 0), new OrgValuePair(orgs[2].name, 0), new OrgValuePair(orgs[3].name, 0) }));
            
        }
    }
    private void FixedUpdate()
    {
        foreach (var district in districts)
        {
            foreach (Char p in district.policemen)
            {
                if (p != null && p.wounded)
                    if (Random.Range(1, 100) < 5)
                        p.Heal();
            }
            foreach (Char b in district.businessmen)
            {
                if (b != null && b.wounded)
                    if (Random.Range(1, 100) < 5)
                        b.Heal();
            }
            if (Random.Range(1, 100) < 10)
                district.CriminalityChange(-Random.Range(1, 5));
            district.UpdatePolice();
        }
        foreach (Org o in orgs)
        {
            foreach (Char c in o.controlled)
                if (Timer.Instance.day == 30)
                    o.money += c.pay;
            foreach (Char c in o.active)
            {
                if (Timer.Instance.day == 30)
                    o.money -= c.pay;
                if (c.wounded)
                    if (Random.Range(1, 100) < 5)
                        c.Heal();
                if (c.strategy != "" && c.subordinates.Count > 0 && !c.squadLeader)
                    foreach (string sub in c.subordinates)
                        o.GetActive(sub).strategy = c.strategy;
                else if (c.subordinates.Count == 0 && !c.solo)
                    c.strategy = "";
                if (GetOrg(c.org).player && c.strategy == "Patrol")
                {
                    Strategy strategy = new PlayerGangsterStrategy();
                    strategy.Prepare(c);
                }
                else if (!GetOrg(c.org).player && c.strategy == "Patrol")
                {
                    Strategy strategy = new AIGangsterStrategy();
                    strategy.Prepare(c);
                }
            }
        }
        foreach (Org o in orgs)
        {
            for (int i = o.active.Count-1; i >= 0 ; i--)
            {  
                Char a = o.active[i];
                if (GetOrg(a.org).player && a.strategy != "Patrol")
                {
                    Char target = null;
                    if (a.order != "")
                        target = o.known.Find(i => i.name == a.orderTarget);
                    Strategy strategy = new PlayerGangsterStrategy();
                    strategy.Prepare(a, false, target);
                }
                else if (!GetOrg(a.org).player && a.strategy != "Patrol")
                {
                    Strategy strategy = new AIGangsterStrategy();
                    strategy.Prepare(a);
                }
            }
            for (int i = o.active.Count - 1; i >= 0; i--)
                if (o.active[i].dead)
                    o.active.Remove(o.active[i]);
        }
        patrols.Clear();
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
