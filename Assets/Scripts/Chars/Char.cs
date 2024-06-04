using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Char : OrgComponent
{
    private string type;
    private string name;
    private Image image;
    private District district;
    private Org org;

    private float mental;
    private float social;
    private float physical;

    private int hiring_price;
    private int upkeep;

    private float wealth;
    private float guards;

    public Char(string type, District district, Org org=null)
    {
        this.type = type;
        this.district = district;
    }

    public override void Add(OrgComponent comp)
    {
        throw new NotImplementedException();
    }

    public override void Remove(OrgComponent comp)
    {
        throw new NotImplementedException();
    }
}
