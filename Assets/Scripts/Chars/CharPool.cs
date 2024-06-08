using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

public class CharPool : MonoBehaviour
{
    public static CharPool Instance { get; private set; }
    [SerializeField]
    private List<Char> pool = new List<Char>();
    private bool generationIsDone;
    [SerializeField]
    private List<CharsToGen> listToGen = new List<CharsToGen>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        if (!SaveLoader.Instance.loading)     
        {
            for (int i = 0; i < 3; i++)
                listToGen.Add(new CharsToGen("gangster", ActiveEntities.Instance.districts[i].name, ActiveEntities.Instance.orgs[i].name, 1));

            foreach (District d in ActiveEntities.Instance.districts)
                foreach (string t in new List<string>() { "businessman", "policeman" })
                    listToGen.Add(new CharsToGen(t, d.name, "", 1));
        }

        generationIsDone = true;


        /*
        foreach (District d in ActiveEntities.Instance.districts)
            foreach (string t in new List<string>() { "gangster", "businessman", "policeman" })
                if (t == "gangster")
                    foreach (Org o in ActiveEntities.Instance.orgs)                        
                        listToGen.Add(new CharsToGen(t, d, o, 1));
                else
                    listToGen.Add(new CharsToGen(t, d, null, 1));
        */
    }

    private Char GetCharFromPool(string type, string district, string org="")
    {
        AddToList(type, district, org);
        foreach (Char c in pool) 
        {
            if (c.type == type && c.district == district && c.org == org)
            {  
                pool.Remove(c);
                return c; 
            }
        }

        GenerateChar(type, district, org);
        Char ch = pool[-1];
        pool.Remove(ch);
        return ch;
    }
    public void AddCharToPool(Char c)
    {
        pool.Add(c);
        generationIsDone = true;
    }

    void Update()
    {
        if (generationIsDone)
        {
            foreach (CharsToGen c in listToGen)
            {
                if (c.num > 0)
                {
                    generationIsDone = false;
                    GenerateChar(c.type, c.district, c.org);
                    break;
                }
            }
        }
    }
    private void GenerateChar(string type, string district, string org)
    {
        Generator.Instance.GenerateChar(type, district, org); 
    }

    private void AddToList(string type, string district, string org)
    {
        foreach(CharsToGen c in listToGen)
        {
            if (c.type == type && c.district == district && c.org == org)
            {
                c.num += 1;
                break;
            }
        }
    }
    public void RemoveFromList(string type, string district, string org)
    {
        foreach (CharsToGen c in listToGen)
        {
            if (c.type == type && c.district == district && c.org == org)
            {
                c.num -= 1;
                break;
            }
        }
    }
    [Serializable]
    private class CharsToGen
    {
        public string type;
        public string district;
        public string org;
        public int num;

        public CharsToGen(string type, string district, string org, int num)
        {
            this.type = type;
            this.district = district;
            this.org = org;
            this.num = num;
        }
    }
    public void Save()
    {
        string temp = JsonUtility.ToJson(this);
        Directory.CreateDirectory("Save");
        File.WriteAllText("Save/CharPool.json", temp);
    }
    public void Load()
    {
        if (Directory.Exists("Save"))
        {
            string temp = File.ReadAllText("Save/CharPool.json");
            JsonUtility.FromJsonOverwrite(temp, this);
        }
    }
}
