using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class District : MonoBehaviour
{
    public List<KeyValuePair> racial_distribution = new List<KeyValuePair>() { new KeyValuePair("White", 0) , new KeyValuePair("African-American", 0 ), new KeyValuePair("Asian-American", 0), new KeyValuePair("Latino-American", 0), new KeyValuePair("Arab-American", 0), new KeyValuePair("Indian-American", 0), new KeyValuePair("Aboriginal-American", 0) };
    public int overall_wealth;
    public int number_of_businesses;
    public List<KeyValuePair> district_control = new List<KeyValuePair>() { new KeyValuePair("Player", 0), new KeyValuePair("Gang1", 0), new KeyValuePair("Gang2", 0), new KeyValuePair("Gang3", 0), new KeyValuePair("None", 0) };

}

[Serializable]
public class KeyValuePair
{
    public String key;
    public int val;

    public KeyValuePair(string key, int val)
    {
        this.key = key;
        this.val = val;
    }
}