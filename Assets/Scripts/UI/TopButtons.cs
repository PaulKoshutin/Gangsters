using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopButtons : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Save()
    {
        SaveLoader.Instance.Save();
    }
    public void ToCity()
    {
        go(c:true);
    }
    public void ToGang()
    {
        go(a:true);
    }
    public void ToOrders()
    {
        go(b:true);
    }
    void go(bool a=false, bool b = false, bool c = false)
    {
        transform.Find("Gang Panel").gameObject.SetActive(a);
        transform.Find("Order Panel").gameObject.SetActive(b);
        transform.Find("City Panel").gameObject.SetActive(c);
    }
}
