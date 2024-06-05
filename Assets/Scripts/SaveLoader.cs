using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoader: MonoBehaviour
{
    public static SaveLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    public void Save()
    {
        CharPool.Instance.Save();
    }
    public void Load()
    {
        CharPool.Instance.Load();
    }
}
