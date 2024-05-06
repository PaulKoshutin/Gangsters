using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textGen : MonoBehaviour
{
    private static readonly textGen instance = new textGen();

    public static textGen Instance
    {
        get { return instance; }
    }

    protected textGen() { }

    public string generateName()
    {
        return "b";
    }
}
