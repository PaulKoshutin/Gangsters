using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharPool : MonoBehaviour
{
    private static readonly CharPool instance = new CharPool();

    public static CharPool Instance
    {
        get { return instance; }
    }
    private List<Char> pool = new List<Char>();

    private Char GetCharFromPool()
    {
        if (pool.Count == 0)
            GenerateChar();
        Char retChar = pool[0];
        pool.RemoveAt(0);
        return retChar;
    }
    private void ReturnCharToPool(Char c)
    {
        pool.Add(c);
    }
    private void GenerateChar()
    {
        Char c = CharGen.Instance.Generate();
        pool.Add(c);
    }


    void Update()
    {
        if (pool.Count<100)
        {
            GenerateChar();
        }
    }
}
