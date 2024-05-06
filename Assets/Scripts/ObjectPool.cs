using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static readonly ObjectPool instance = new ObjectPool();

    public static ObjectPool Instance
    {
        get { return instance; }
    }
    private List<Gangster> pool = new List<Gangster>();



    private Gangster GetObjectFromPool()
    {
        if (pool.Count == 0)
            generateObject();
        Gangster retObject = pool[0];
        pool.RemoveAt(0);
        return retObject;
    }
    private void ReturnObjectToPool(Gangster gangster)
    {
        pool.Add(gangster);
    }
    private void generateObject()
    {
        Gangster gangster = new Gangster();
        pool.Add(gangster);
    }

    protected ObjectPool() {}


    void Update()
    {
        if (pool.Count<100)
        {
            generateObject();
        }
    }
}
