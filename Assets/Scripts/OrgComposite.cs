using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrgComposite : OrgComponent
{
    List<OrgComponent> children = new List<OrgComponent>();


    public override void Add(OrgComponent comp)
    {
        children.Add(comp);
    }

    public override void Remove(OrgComponent comp)
    {
        children.Remove(comp);
    }
}
