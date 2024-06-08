using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

[Serializable]
public class Org
{
    public bool player;
    public string name;
    public string color;
    public Sprite emblem;
    public Char leader;
    public List<Char> members = new List<Char>();
    public int money;

    public Org(string name, string color, int money, bool player = false)
    {
        this.player = player;
        this.name = name;
        this.color = color;
        this.money = money;

        this.emblem = Resources.Load("Sprites/"+name, typeof(Sprite)) as Sprite;
    }
}
