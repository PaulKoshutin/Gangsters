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
        transform.Find("Order Panel").gameObject.SetActive(true);
        transform.Find("Gang Panel").gameObject.SetActive(true);
        transform.Find("City Panel").gameObject.SetActive(true);
        if (a) 
            transform.Find("Gang Panel").localPosition = new Vector3(0, 0, 0);   
        else
            transform.Find("Gang Panel").localPosition = new Vector3(5000, 0, 0);

        if (b)
            transform.Find("Order Panel").localPosition = new Vector3(0, 0, 0);
        else
            transform.Find("Order Panel").localPosition = new Vector3(5000, 0, 0);

        if (c)
            transform.Find("City Panel").localPosition = new Vector3(0, 0, 0);
        else
            transform.Find("City Panel").localPosition = new Vector3(5000, 0, 0);
    }
}
