using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[Serializable]
public class Char : ISerializationCallbackReceiver
{
    public string type;
    public string name;
    public string description;
    public Sprite image;
    public string district;
    public string org;

    public int mental;
    public int social;
    public int physical;

    public int pay;

    public bool squadLeader;
    public bool solo;
    public string superior;
    public List<string> subordinates = new List<string>();

    public string strategy;
    public string order;
    public string orderTarget;
    public string targetedByOrder;

    public DraggableChar draggableIcon;
    public bool wounded;
    public bool dead;


    public Char (string type, string district, string org, string name, Sprite image, int mental, int social, int physical, int pay, bool squadLeader=false, bool solo=false, string strategy="")
    {
        this.type = type;
        this.district = district;
        this.org = org;
        this.name = name;
        this.image = image;
        this.mental = mental;
        this.social = social;
        this.physical = physical;

        this.pay = pay;

        this.squadLeader = squadLeader;
        this.solo = solo;
        superior = "";
        this.strategy = strategy;
        order = "";
        orderTarget = "";
        targetedByOrder = "";
        this.wounded = false;
        this.dead = false;
        this.description = "Name: " + name + "\nGang: " + org + "\nUpkeep: " + pay + "\nMental: " + mental + "\nSocial: " + social + "\nPhysical: " + physical;
    }
    public Char(Char c, int mental, int social, int physical)
    {
        this.type = c.type;
        this.district = c.district;
        this.org = c.org;
        this.name = c.name;
        this.image = c.image;
        this.mental = mental;
        this.social = social;
        this.physical = physical;

        this.pay = c.pay;

        this.subordinates = c.subordinates;
        this.squadLeader = c.squadLeader;
        this.solo = c.solo;
        this.superior = c.superior;
        strategy = c.strategy;
        order = c.order;
        orderTarget = c.orderTarget;
        targetedByOrder = c.targetedByOrder;
        this.wounded = c.wounded;
        this.dead = c.dead;
        draggableIcon = c.draggableIcon;
        this.description = "Name: " + c.name + "\nGang: " + c.org + "\nUpkeep: " + c.pay + "\nMental: " + mental + "\nSocial: " + social + "\nPhysical: " + physical;
    }
    public void Fire()
    {
        Org o = ActiveEntities.Instance.GetOrg(org);
        District d = ActiveEntities.Instance.GetDistrict(district);
        foreach (Org or in ActiveEntities.Instance.orgs)
            if (or.known.Contains(this))
                or.known.Remove(this);
        if (d.businessmen.Contains(this))
        {
            d.businessmen.Remove(this);
            d.number_of_businesses -= 1;
        }
        if (d.policemen.Contains(this))
            d.policemen.Remove(this);
        if (o != null)
        {
            if (superior != "")
            {
                o.GetActive(superior).subordinates.Remove(name);
                if (subordinates.Count > 0)
                    foreach (string s in subordinates)
                        o.GetActive(superior).subordinates.Add(s);
            }
            if (subordinates.Count > 0)
                foreach (string s in subordinates)
                    o.GetActive(s).superior = this.superior;
            if (this.superior == "" && !o.reserve.Contains(this) && !o.destroyed)
                o.DestroyOrg();
            if (o.reserve.Contains(this))
                o.reserve.Remove(this);
        }
        else
        {
            if (superior != "")
            {
                d.policemen[0].subordinates.Remove(name);
                if (subordinates.Count > 0)
                    foreach (string s in subordinates)
                        d.policemen[0].subordinates.Add(s);
            }
            if (subordinates.Count > 0)
                foreach (string s in subordinates)
                    d.policemen.Find(i=>i.name == s).superior = this.superior;
        }
        if (draggableIcon != null)
            draggableIcon.Fire();
        this.dead = true;
    }
    public void PostFireCleanup()
    {
        Org o = ActiveEntities.Instance.GetOrg(org);
        if (o.active.Contains(this) && this.dead)
            o.active.Remove(this);
    }
    public void Free()
    {
        ActiveEntities.Instance.GetOrg(org).controlled.Remove(this);
        this.org = "";
    }
    public void Punishment(int val, bool robbery=false)
    {
        if (!robbery)
        {
            mental -= val;
            social -= val;
            physical -= val;

            if (mental > 100)
                mental = 100;
            else if (mental < 10) { mental = 10; }
            if (social > 100)
                social = 100;
            else if (social < 10) { social = 10; }
            if (physical > 100)
                physical = 100;
            else if (physical < 10) { physical = 10; }
        }
        else
        {
            pay -= val;
            if (pay < 50)
                pay = 50;
        }
    }
    public void AddSub(Char c)
    {
        solo = false;
        c.superior = this.name;
        if (!squadLeader)
            c.solo = solo;
        this.subordinates.Add(c.name);
    }
    public void Move(String district)
    {
        Org o = ActiveEntities.Instance.GetOrg(org);
        this.district = district;
        foreach (string s in this.subordinates)
            o.GetActive(s).district = district;
    }
    public void Wound()
    {
        wounded = true;

        mental /= 2;
        social /= 2;
        physical /= 2;
    }
    public void Heal()
    {
        wounded = false;

        mental *= 2; 
        social *= 2;
        physical *= 2;
    }
    public void OnAfterDeserialize()
    {
        try
        {
            image = Utils.Instance.LoadNewSprite("Images/" + name + ".png");
        } catch { }
    }

    public void OnBeforeSerialize()
    {
    }
}