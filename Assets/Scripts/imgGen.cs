using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class imgGen : MonoBehaviour
{
    private static readonly imgGen instance = new imgGen();

    public static imgGen Instance
    {
        get { return instance; }
    }

    protected imgGen() { }

    public Image generateImage()
    {
        return null;
    }
}
