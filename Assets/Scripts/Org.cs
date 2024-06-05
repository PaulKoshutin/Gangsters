using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class Org
{
    public string name;
    public string orgFeature;
    public List<Char> members = new List<Char>();
    public int money;

    public Org(string name, string orgFeature, int money)
    {
        this.name = name;
        this.orgFeature = orgFeature;
        this.money = money;
    }
}
