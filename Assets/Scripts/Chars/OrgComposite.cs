using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrgComposite : OrgComponent
{
    private string officeName;
    public bool squadLeader;
    public List<OrgComponent> subordinates = new List<OrgComponent>();
    private void Awake()
    {
        officeName = "Leutenant";
        squadLeader = false;
    }

    public override void Add(OrgComponent comp)
    {
        subordinates.Add(comp);
        comp.superior = this;
    }

    public override void Remove(OrgComponent comp)
    {
        subordinates.Remove(comp);
    }
}
