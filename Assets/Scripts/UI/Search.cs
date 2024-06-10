using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Search : MonoBehaviour
{
    private string charName;
    private bool player;
    private bool nonPlayer;
    private bool gangster;
    private bool policeman;
    private bool businessman;
    private void Awake()
    {
        charName = "";
        player = true;
        nonPlayer = true;
        gangster = true;
        policeman = true;
        businessman = true;
    }
    public void GetName(string charName)
    {
        this.charName = charName;
    }
    public void GetPlayer(bool player)
    {
       this.player = player;
    }
    public void GetNonPlayer(bool nonPlayer)
    {
        this.nonPlayer = nonPlayer;
    }
    public void GetGangster(bool gangster)
    { this.gangster = gangster;}
    public void GetPoliceman(bool policeman)
    {  this.policeman = policeman;}
    public void GetBusinessman(bool businessman)
    { this.businessman = businessman;}
    public void StartSearch()
    {
        List<Char> found = new List<Char>();
        Transform content = transform.Find("Char List Panel").Find("Scroll View").Find("Viewport").Find("Content");
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        if (player) 
        {
            if (gangster)
            {
                found.AddRange(ActiveEntities.Instance.orgs[0].active);
                found.AddRange(ActiveEntities.Instance.orgs[0].reserve);
            }
            if (businessman)
                found.AddRange(ActiveEntities.Instance.orgs[0].controlled);
        }
        if (nonPlayer)
        {
            List<Char> known = ActiveEntities.Instance.orgs[0].known;
            if (gangster)
                foreach (Char c in known)
                    if (c.type == "gangster")
                        found.Add(c);
            if (policeman)
                foreach (Char c in known)
                    if (c.type == "policeman")
                        found.Add(c);
            if (businessman)
                foreach (Char c in known)
                    if (c.type == "businessman")
                        found.Add(c);
        }
        if (charName != "")
            for (int i = found.Count-1; i >= 0; i--)
                if (!found[i].name.Contains(charName))
                    found.RemoveAt(i);
        foreach (Char c in found)
        {
            GameObject row = Instantiate(Resources.Load("Prefabs/SearchRow", typeof(GameObject)), parent: content) as GameObject;
            row.transform.GetChild(0).GetComponent<DraggableChar>().SetChar(c);
            row.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = c.description;
        }
    }
}
