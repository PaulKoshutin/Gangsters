using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OrgComponent : MonoBehaviour
{
    protected string charName;
    protected Image image;
    protected List<int> stats = new List<int>();

    public OrgComponent()
    {
        charName = textGen.Instance.generateName();
        image = imgGen.Instance.generateImage();
        stats = statsGen.Instance.generateStats();
    }

    public abstract void Add(OrgComponent c);
    public abstract void Remove(OrgComponent c);
}
