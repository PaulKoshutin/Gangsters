using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Facade : MonoBehaviour
{
    public static Facade Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
    public void GenerateOrg(string name, string color)
    {
        Generator.Instance.GenerateOrg(name, color);
    }
    public void GenerateCharManually(string background, string race, string gender, string color, string charName, string orgName)
    {
        Generator.Instance.GenerateChar("gangster", "Northslum", orgName, true, background,race,gender,color, charName);
    }
}
