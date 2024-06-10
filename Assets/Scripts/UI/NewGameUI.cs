using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameUI : MonoBehaviour
{
    public string orgName;
    public string color;
    public string feature;

    public string charName;
    public string background;
    public string race;
    public string gender;

    private void Awake()
    {
        orgName = "Lords";
        color = "purple";
        feature = "";

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
    public void getFeature(int val)
    {
        if (val == 0)
            this.feature = "";
        else if (val == 1)
            this.feature = "_top_hat";
        else if (val == 2)
            this.feature = "_beanie";
        else if (val == 3)
            this.feature = "_sunglasses";
        else if (val == 4)
            this.feature = "_hair";
    }
    public void getCharName(string name)
    {
        this.charName = name;
    }
    public void getBackground(int val)
    {
        if (val == 0)
            this.background = "poor";
        else if (val == 1)
            this.background = "middle-class";
        else if (val == 2)
            this.background = "rich";
    }
    public void getRace(int val)
    {
        if (val == 0)
            this.race = "white-american";
        else if (val == 1)
            this.race = "african-american";
        else if (val == 2)
            this.race = "asian-american";
        else if (val == 3)
            this.race = "latino-american";
        else if (val == 4)
            this.race = "arab-american";
        else if (val == 5)
            this.race = "indian-american";
    }
    public void getGender(int val)
    {
        if (val == 0)
            this.gender = "male";
        else if (val == 1)
            this.gender = "female";
    }
    public void generateOrg()
    {
        Facade.Instance.GenerateOrg(orgName, color + feature);
    }
    public void generateChar()
    {
        Facade.Instance.GenerateCharManually(background,race,gender,color + feature, charName, orgName);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
