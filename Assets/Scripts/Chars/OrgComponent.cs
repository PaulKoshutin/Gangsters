using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OrgComponent
{
    public abstract void Add(OrgComponent c);
    public abstract void Remove(OrgComponent c);
}
