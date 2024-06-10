using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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


    public Char (string type, string district, string org, string name, Sprite image, int mental, int social, int physical, int pay, bool squadLeader=false, bool solo=false)
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
        strategy = "";
        order = "";
        orderTarget = "";
        this.description = "Name: " + name + "\nGang: " + org + "\nUpkeep: " + pay + "\nMental: " + mental + "\nSocial: " + social + "\nPhysical: " + physical;
    }
    public void Fire()
    {
        if (ActiveEntities.Instance.GetOrg(name).reserve.Contains(this))
            ActiveEntities.Instance.GetOrg(name).reserve.Remove(this);
        if (ActiveEntities.Instance.GetOrg(name).active.Contains(this))
            ActiveEntities.Instance.GetOrg(name).active.Remove(this);
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