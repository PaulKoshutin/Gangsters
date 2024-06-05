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

    private string charName;
    private string type;
    private District district;
    private string org;
    private string background;
    private string race;
    private string gender;
    private string orgFeature;
    private Sprite image;

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
                listToGen.Add(new CharsToGen("gangster", ActiveEntities.Instance.districts[i], ActiveEntities.Instance.orgs[i].name, 1));

            foreach (District d in ActiveEntities.Instance.districts)
                foreach (string t in new List<string>() { "businessman", "policeman" })
                    listToGen.Add(new CharsToGen(t, d, "", 1));
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

    private Char GetCharFromPool(string type, District district, string org="")
    {
        AddToList(type, district, org);
        foreach (Char c in pool) 
        {
            if (c.type == type && c.district == district.name && c.org == org)
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
    void Update()
    {
        if (generationIsDone)
        {
            if (charName != null)
            {
                RemoveFromList(type, district, org);
                FinishChar();
            }
            foreach (CharsToGen c in listToGen)
            {
                if (c.num > 0)
                {
                    GenerateChar(c.type, c.district, c.org);
                    break;
                }
            }
        }
    }

    private void AddToList(string type, District district, string org)
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
    private void RemoveFromList(string type, District district, string org)
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
    
    private async void GenerateChar(string type, District district, string org)
    {
        GetCharData(type, district, org);
        Task task = new Task(StartWorking);
        task.Start();
        Debug.Log("My exe file is running right now type:" + type + ", district:" + district.name + ", org:"+org);
        await task;
    }
    public void StartWorking()
    {
        generationIsDone = false;

        Process p = new Process();
        if (orgFeature != "")
            p.StartInfo = new ProcessStartInfo("python.exe", "t2i.py " + background + " " + race + " " + gender + " " + type + " " + orgFeature)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        else
            p.StartInfo = new ProcessStartInfo("python.exe", "t2i.py " + background + " " + race + " " + gender + " " + type)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        p.Start();

        string name = p.StandardOutput.ReadLine();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        Debug.Log(name);
        Debug.Log(output);

        if (name == null || output == "")
            charName = null;
        else
            charName = name;

        generationIsDone = true;
    }
    private void GetCharData(string type, District district, string org)
    {
        this.type = type;
        this.district = district;
        this.org = org;
        if (org == "" && type == "policeman")
            orgFeature = "blue";
        else if (org == "")
            orgFeature = "";
        else { orgFeature = ActiveEntities.Instance.GetOrg(org).orgFeature; }
        if (UnityEngine.Random.Range(0, 100) <= district.overall_wealth)
            background = "rich";
        else background = "poor";
        if (UnityEngine.Random.Range(0, 100) <= 80)
            gender = "male";
        int raceRoll = UnityEngine.Random.Range(0, 100);
        int raceSum = 0;
        for (int i = 0; i < district.racial_distribution.Count; i++)
        {
            if (raceRoll <= district.racial_distribution[i].val + raceSum)
            {
                race = district.racial_distribution[i].key;
                break;
            }
            else
                raceSum += district.racial_distribution[i].val;
        }
    }
    private void FinishChar()
    {
        Char c = new Char();

        int mental = UnityEngine.Random.Range(10, 100);
        if (background == "rich")
            mental += UnityEngine.Random.Range(5, 20);
        if (mental > 100)
            mental = 100;

        int social = UnityEngine.Random.Range(10, 100);

        int physical = UnityEngine.Random.Range(10, 100);
        if (background == "poor")
            physical += UnityEngine.Random.Range(5, 20);
        if (physical > 100)
            physical = 100;

        int hiring_price = 0;
        if (type == "gangster")
            hiring_price = mental + social + physical;

        int wealth = 0;
        if (type == "businessman")
            wealth = (UnityEngine.Random.Range(10, 100) + UnityEngine.Random.Range(5, district.overall_wealth)) * 2;

        c.SetData(type, district.name, org, charName, c.LoadNewSprite("Images/" + charName + ".png"), mental, social, physical, hiring_price, wealth);

        pool.Add(c);
        charName = null;
    }

    [Serializable]
    private class CharsToGen
    {
        public string type;
        public District district;
        public string org;
        public int num;

        public CharsToGen(string type, District district, string org, int num)
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
