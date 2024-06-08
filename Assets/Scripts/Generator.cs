using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class Generator : MonoBehaviour
{
    public static Generator Instance { get; private set; }

    private string charName;
    private string type;
    private string districtName;
    private string orgName;
    private string background;
    private string race;
    private string gender;
    private string color;
    public Char c;
    public Org o;
    private bool manual;
    private bool generationOfOrgIsDone;
    private bool generationOfCharIsDone;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        DontDestroyOnLoad(this.gameObject);

        manual = false;
        generationOfOrgIsDone = false;
        generationOfCharIsDone = false;
    }
    public async void GenerateOrg(string orgName, string color)
    {
        this.orgName = orgName;
        this.color = color;

        Task task = new Task(StartWorkingOnOrg);
        task.Start();
        Debug.Log("Working on org. name:" + orgName + ", color:" + color);
        await task;
    }
    private void StartWorkingOnOrg()
    {
        generationOfOrgIsDone = false;

        Process p = new Process();
        p.StartInfo = new ProcessStartInfo("python.exe", "t2i.py org " + orgName.Replace(" ", "_") + " " + color)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        p.Start();

        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        Debug.Log(output);

        if (output == "")
            GenerateOrg(orgName, color);
        else
            generationOfOrgIsDone = true;
    }
    public void GenerateCharManually(string background, string race, string gender, string color, string charName, string orgName)
    {
        this.background = background;
        this.race = race;   
        this.gender = gender;
        this.color = color;
        this.charName = charName;
        this.orgName = orgName;
        this.type = "gangster";
        this.districtName = "Northslum";
        manual = true;

        GenerateChar(type, districtName, orgName);     
    }
    public async void GenerateChar(string type, string district, string orgName)
    {
        if (!manual)
            GetCharData(type, district, orgName);
        Task task = new Task(StartWorkingOnChar);
        task.Start();
        Debug.Log("Working on char. type:" + type + ", district:" + district + ", org:" + orgName);
        await task;
    }
    public void StartWorkingOnChar()
    {
        generationOfCharIsDone = false;

        Process p = new Process();
        p.StartInfo = new ProcessStartInfo("python.exe", "t2i.py " + background + " " + race + " " + gender + " " + type + " " + color + " " + charName.Replace(" ", "_"))
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
        {
            if (!manual)
                GenerateChar(type, districtName, orgName);
            else
                GenerateChar("gangster", "Northslum", orgName);
        }
        else
        {
            generationOfCharIsDone = true;

            if (!manual)
            {
                charName = name;
                CharPool.Instance.RemoveFromList(type, districtName, orgName);
            }
        }   
    }
    private void GetCharData(string type, string districtName, string orgName)
    {
        this.type = type;
        this.districtName = districtName;
        District district = ActiveEntities.Instance.districts.Find(i => i.name == districtName);
        this.orgName = orgName;
        if (orgName == "" && type == "policeman")
            color = "blue";
        else if (orgName == "")
            color = "";
        else { color = ActiveEntities.Instance.GetOrg(orgName).color; }
        int wealthRoll = Random.Range(0, 100);
        if (wealthRoll <=  district.overall_wealth)
            background = "rich";
        else if (wealthRoll <= district.overall_wealth + 30)
            background = "middle-class";
        else background = "poor";
        if (Random.Range(0, 100) <= 80)
            gender = "male";
        int raceRoll = Random.Range(0, 100);
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
        int mental = Random.Range(10, 100);
        int social = Random.Range(10, 100);
        int physical = Random.Range(10, 100);
        if (background == "middle-class")
        {
            mental += Random.Range(5, 20);
            social -= Random.Range(5, 20);
        }     
        if (background == "rich")
        {
            social += Random.Range(5, 20);
            physical -= Random.Range(5, 20);
        }  
        if (background == "poor")
        {
            physical += Random.Range(5, 20);
            mental -= Random.Range(5, 20);
        }
        if (mental > 100)
            mental = 100;
        else if (mental < 10) { mental = 10; }
        if (social > 100)
            social = 100;
        else if (social < 10) { social = 10; }
        if (physical > 100)
            physical = 100;
        else if (physical < 10) {  physical = 10; }

        int upkeep = 0;
        if (type == "gangster")
            upkeep = mental + social + physical;

        int wealth = 0;
        if (type == "businessman")
            wealth = Random.Range(50, 200);
        if (background == "middle-class")
            wealth += Random.Range(50, 200);
        if (background == "rich")
            wealth += Random.Range(100, 400);

        c = new (type, districtName, orgName, charName, Utils.Instance.LoadNewSprite("Images/" + charName + ".png"), mental, social, physical, upkeep, wealth);

        if (!manual)
            CharPool.Instance.AddCharToPool(c);
        else
        {
            o.leader = c;
            GameObject.FindGameObjectWithTag("New Char Image").GetComponent<Image>().sprite = Utils.Instance.LoadNewSprite("Images/" + charName + ".png");
        }
        generationOfCharIsDone = false;
        manual = false;
    }
    private void FinishOrg()
    {
        GameObject.FindGameObjectWithTag("New Org Image").GetComponent<Image>().sprite = Utils.Instance.LoadNewSprite("Images/" + orgName + ".png");
        o = new Org(orgName, color, 500, true);
        generationOfOrgIsDone = false;
    }
    private void Update()
    {
        if (generationOfCharIsDone)
            FinishChar();
        if (generationOfOrgIsDone)
            FinishOrg();
    }
}
