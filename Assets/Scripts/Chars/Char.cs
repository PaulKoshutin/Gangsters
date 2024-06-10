using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[Serializable]
public class Char : ISerializationCallbackReceiver
{
    public string type;
    public string name;
    public Sprite image;
    public string district;
    public string org;

    public int mental;
    public int social;
    public int physical;

    public int pay;

    public bool squadLeader;
    public string superior;
    public List<string> subordinates;


    public Char (string type, string district, string org, string name, Sprite image, int mental, int social, int physical, int pay)
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

        squadLeader = false;
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