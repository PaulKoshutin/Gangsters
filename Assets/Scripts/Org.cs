using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

[Serializable]
public class Org:ISerializationCallbackReceiver
{
    public bool player;
    public string name;
    public string color;
    public Sprite emblem;
    public Char leader;
    public List<Char> members = new List<Char>();
    public List<Char> reserve = new List<Char>();
    public List<Char> knownChars = new List<Char>();
    public int money;

    public Org(string name, string color, int money, bool player = false, Sprite emblem=null)
    {
        this.player = player;
        this.name = name;
        this.color = color;
        this.money = money;
        if (emblem == null)
            this.emblem = Resources.Load("Sprites/"+name, typeof(Sprite)) as Sprite;
        else
            this.emblem = Utils.Instance.LoadNewSprite("Images/" + name + ".png"); ;
    }
    public void OnAfterDeserialize()
    {
        try
        {
            emblem = Utils.Instance.LoadNewSprite("Images/" + name + ".png");
        }
        catch { }
    }
    public void OnBeforeSerialize()
    {
    }
}
