using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrgComposite : OrgComponent
{
    private Char officeholder;
    private OrgComponent superior;
    List<OrgComponent> subordinates = new List<OrgComponent>();

    public override void Add(OrgComponent comp)
    {
        subordinates.Add(comp);
    }

    public override void Remove(OrgComponent comp)
    {
        subordinates.Remove(comp);
    }
}
