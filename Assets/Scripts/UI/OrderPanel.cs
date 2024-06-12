using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanel : MonoBehaviour
{
    private Char executor;
    private Image exeImage;
    private Char target;
    private Image tarImage;
    private string order;

    private void Awake()
    {
        exeImage = transform.Find("Executor Image").GetComponent<Image>();
        tarImage = transform.Find("Target Image").GetComponent<Image>();
    }
    public void SetExecutor(Char c)
    {
        executor = c;
        exeImage.sprite = c.image;
    }
    public void SetTarget(Char c)
    {
        target = c;
        tarImage.sprite = c.image;
    }
    public void GetOrder(int val)
    {
        if (val == 0)
            order = "";
        else if (val == 1)
            order = "Kill";
    }
    public void GiveTheOrder()
    {
        if (executor != null && target != null && order != "")
        {
            executor.order = order;
            executor.orderTarget = target.name;
            target.targetedByOrder = executor.order;

            executor = null;
            target = null;
            exeImage = null;
            tarImage = null;
            order = "";
            transform.Find("Order Dropdown").GetComponent<TMP_Dropdown>().value = 0;
        }
    }
}
