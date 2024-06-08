using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameUI : MonoBehaviour
{
    public string orgName;
    public string color;

    public string charName;
    public string background;
    public string race;
    public string gender;

    private void Awake()
    {
    orgName = "Lords";
    color = "purple";

    charName = "Joe Masters";
    background = "poor";
    race = "white-american";
    gender = "male";
    }
    public void getOrgName(string name)
    {
        this.orgName = name;
    }
    public void getColor(int val)
    {
        if (val == 0)
            this.color = "purple";
        else if (val == 1)
            this.color = "yellow";
        else if (val == 2)
            this.color = "orange";
    }
    public void getCharName(string name)
    {
        this.charName = name;
    }
    public void getBackground(int val)
    {
        if (val == 0)
            this.color = "poor";
        else if (val == 1)
            this.color = "middle-class";
        else if (val == 2)
            this.color = "rich";
    }
    public void getRace(int val)
    {
        if (val == 0)
            this.color = "white-american";
        else if (val == 1)
            this.color = "african-american";
        else if (val == 2)
            this.color = "asian-american";
        else if (val == 3)
            this.color = "latino-american";
        else if (val == 4)
            this.color = "arab-american";
        else if (val == 5)
            this.color = "indian-american";
    }
    public void getGender(int val)
    {
        if (val == 0)
            this.color = "male";
        else if (val == 1)
            this.color = "female";
    }
    public void generateOrg()
    {
        Facade.Instance.GenerateOrg(orgName, color);
    }
    public void generateChar()
    {
        Facade.Instance.GenerateCharManually(background,race,gender,color, charName, orgName);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
