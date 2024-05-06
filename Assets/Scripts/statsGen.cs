using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statsGen : MonoBehaviour
{
    private static readonly statsGen instance = new statsGen();

    public static statsGen Instance
    {
        get { return instance; }
    }

    protected statsGen() { }

    public List<int> generateStats()
    {
        return null;
    }
}
