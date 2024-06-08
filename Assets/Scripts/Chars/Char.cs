using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[Serializable]
public class Char : OrgComponent, ISerializationCallbackReceiver
{
    public string type;
    public string name;
    public Sprite image;
    public string district;
    public string org;

    public int mental;
    public int social;
    public int physical;

    public int upkeep;

    public int wealth;

    public Char (string type, string district, string org, string name, Sprite image, int mental, int social, int physical, int upkeep, int wealth)
    {
        this.type = type;
        this.district = district;
        this.org = org;
        this.name = name;
        this.image = image;
        this.mental = mental;
        this.social = social;
        this.physical = physical;

        this.upkeep = upkeep;
        this.wealth = wealth;
    }


    public override void Add(OrgComponent comp)
    {
        throw new NotImplementedException();
    }

    public override void Remove(OrgComponent comp)
    {
        throw new NotImplementedException();
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