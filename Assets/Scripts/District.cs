using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class District
{
    public string name;
    public List<KeyValuePair> racial_distribution = new List<KeyValuePair>() { new KeyValuePair("White-American", 0) , new KeyValuePair("African-American", 0 ), new KeyValuePair("Asian-American", 0), new KeyValuePair("Latino-American", 0), new KeyValuePair("Arab-American", 0), new KeyValuePair("Indian-American", 0)};
    public int overall_wealth;
    public int number_of_businesses;
    public List<OrgValuePair> district_control = new List<OrgValuePair>();

    public District(string name, List<int> race, int overall_wealth, int number_of_businesses, List<OrgValuePair> district_control)
    {
        this.name = name;
        for (int i = 0; i < race.Count; i++) 
            racial_distribution[i].val = race[i];
        this.overall_wealth = overall_wealth;
        this.number_of_businesses = number_of_businesses;
        this.district_control = district_control;

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