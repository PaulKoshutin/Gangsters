using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Org:ISerializationCallbackReceiver
{
    public bool player;
    public string name;
    public string color;
    public Sprite emblem;
    public List<Char> active = new List<Char>();
    public List<Char> reserve = new List<Char>();
    public List<Char> recruitable = new List<Char>();
    public List<Char> controlled = new List<Char>();
    public List<Char> known = new List<Char>();
    public List<OrgPolicyPair> policies = new List<OrgPolicyPair>();
    public int money;
    public int respect;

    public Org(string name, string color, int money, bool player = false, Sprite emblem=null)
    {
        this.player = player;
        this.name = name;
        this.color = color;
        this.money = money;
        if (emblem == null)
            this.emblem = Resources.Load("Sprites/"+name, typeof(Sprite)) as Sprite;
        else
            this.emblem = Utils.Instance.LoadNewSprite("Images/" + name + ".png");
        respect = 0;
    }
    public void InitiatePolicies()
    {
        foreach (Org o in ActiveEntities.Instance.orgs)
            if (o.name != name)
                policies.Add(new(o.name, "Peace"));
    }
    public void AddToKnown(Char c)
    {
        if (!known.Contains(c))
            known.Add(c);
    }
    public void AddToRecruitable(Char c)
    {
        if (!recruitable.Contains(c))
            recruitable.Add(c);
    }
    public void AddToReserve(Char c, bool firstTime)
    {
        if (!reserve.Contains(c))
            reserve.Add(c);
        if(active.Contains(c))
            active.Remove(c);
        if (recruitable.Contains(c))
            recruitable.Remove(c);
        if (firstTime)
            HierarchyManager.Instance.AddToReserve(c);
    }
    public void AddToActive(Char c)
    {
        if (!active.Contains(c))
            active.Add(c);
        if (reserve.Contains(c))
            reserve.Remove(c);
        if (known.Contains(c))
            known.Remove(c);
        if (recruitable.Contains(c))
            recruitable.Remove(c);
    }
    public Char GetRecruitable()
    {
        foreach (Char c in recruitable)
        {
            if (c.type == "gangster" && c.org == name)
                return c;
        }
        return null;
    }
    public Char GetActive(string name)
    {
        return active.Find(i => i.name == name);
    }
    public void OnAfterDeserialize()
    {
        try
        {
            emblem = Resources.Load("Sprites/" + name, typeof(Sprite)) as Sprite;
            if (emblem == null)
                emblem = Utils.Instance.LoadNewSprite("Images/" + name + ".png");
        }
        catch {
            
        }
    }
    public void OnBeforeSerialize()
    {
    }
    [Serializable]
    public class OrgPolicyPair
    {
        public string org;
        public string policy;

        public OrgPolicyPair(string org, string policy)
        {
            this.org = org;
            this.policy = policy;
        }
    }
}
