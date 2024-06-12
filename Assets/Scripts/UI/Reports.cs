using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Reports : MonoBehaviour
{
    public static Reports Instance { get; private set; }
    private TMP_Text text;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        text = gameObject.GetComponent<TMP_Text>();
    }
    public void AddReport(string report)
    {
        string old = text.text;
        text.text = report + "\n" + old;
        if (text.text.Length > 300)
        {
            int lastPos = text.text.LastIndexOf("\n");
            text.text = text.text.Substring(0, text.text.LastIndexOf("\n", lastPos - 1));
        }
    }
}
