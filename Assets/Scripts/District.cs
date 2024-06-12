using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[Serializable]
public class District : ISerializationCallbackReceiver
{
    public string name;
    public Sprite view;
    public List<KeyValuePair> racial_distribution = new List<KeyValuePair>() { new KeyValuePair("White-American", 0) , new KeyValuePair("African-American", 0 ), new KeyValuePair("Asian-American", 0), new KeyValuePair("Latino-American", 0), new KeyValuePair("Arab-American", 0), new KeyValuePair("Indian-American", 0)};
    public int wealth;
    public int criminality;
    public int number_of_businesses;
    public List<Char> businessmen = new List<Char>();
    public List<Char> policemen = new List<Char>();
    public List<OrgValuePair> district_control = new List<OrgValuePair>();

    public District(string name, List<int> race, int wealth, int criminality, int number_of_businesses, List<OrgValuePair> district_control)
    {
        this.name = name;
        for (int i = 0; i < race.Count; i++) 
            racial_distribution[i].val = race[i];
        this.wealth = wealth;
        this.criminality = criminality;
        this.number_of_businesses = number_of_businesses;
        this.district_control = district_control;

        this.view = Resources.Load("Sprites/" + name, typeof(Sprite)) as Sprite;
    }
    public string GetControlData()
    {
        UpdateControlData();
        string output = "";
        foreach (OrgValuePair pair in district_control) 
        { 
            output += "\n" + pair.org.name +": "+ pair.val;
        }
        return output;
    }
    public void UpdateControlData()
    {
        foreach (OrgValuePair pair in district_control)
        {
            int sum = 0;
            foreach (Char c in pair.org.controlled)
                if (c.district == name)
                    sum++;
            pair.val = sum;
        }
    }
    public void UpdatePolice()
    {
        if (policemen.Count == 0)
        {
            Char p = CharPool.Instance.GetCharFromPool("policeman", name, "");
            if (p != null)
            {
                policemen.Add(p);
                p.squadLeader = true;
            }
        }
        else if (policemen.Count < 5)
        {
            Char p = CharPool.Instance.GetCharFromPool("policeman", name, "");
            if (p != null)
            {
                policemen.Add(p);
                policemen[0].subordinates.Add(p.name);
                p.superior = policemen[0].name;
            }
        }
        if (policemen.Count > 0)
        {
            if (!policemen[0].squadLeader)
            {
                policemen[0].squadLeader = true;
                for (int i = 1; i < policemen.Count; i++)
                {
                    policemen[0].subordinates.Add(policemen[i].name);
                    policemen[i].superior = policemen[0].name;
                }
            }
            if (criminality < 80)
                policemen[0].strategy = "Patrol";
            else
                policemen[0].strategy = "Hunt";

            Strategy strategy = new PlayerGangsterStrategy();
            strategy.Prepare(policemen[0]);
        }
    }
    public Char GetExtortable(Org o)
    {
        foreach (Char b in businessmen)
            if (b.org != o.name)
                if (b.org == "" || o.GetPolicyTowards(b.org) != "Peace")
                    return b;
        return null;
    }
    public void CriminalityChange(int val)
    {
        criminality += val;
        if (criminality > 100)
            criminality = 100;
        else if (criminality < 0)
            criminality = 0;
    }
    public void OnAfterDeserialize()
    {
        try
        {
            this.view = Resources.Load("Sprites/" + name, typeof(Sprite)) as Sprite;
        }
        catch { }
    }
    public void OnBeforeSerialize()
    {
    }
}

[Serializable]
public class KeyValuePair
{
    public String key;
    public int val;

    public KeyValuePair(string key, int val)
    {
        this.key = key;
        this.val = val;
    }
}

[Serializable]
public class OrgValuePair
{
    public Org org;
    public int val;

    public OrgValuePair(Org org, int val)
    {
        this.org = org;
        this.val = val;
    }
}