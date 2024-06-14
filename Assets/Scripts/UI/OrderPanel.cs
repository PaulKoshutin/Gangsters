using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanel : MonoBehaviour
{
    private Sprite defaultSprite;
    private Char executor;
    private Image exeImage;
    private Char target;
    private Image tarImage;
    private string order;

    private void Awake()
    {
        exeImage = transform.Find("Executor Image").GetComponent<Image>();
        tarImage = transform.Find("Target Image").GetComponent<Image>();
        defaultSprite = exeImage.sprite;
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
            order = "Extort";
        else if (val == 2)
            order = "Punish";
        else if (val == 3)
            order = "Rob";
        else if (val == 4)
            order = "Hunt";
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
            exeImage.sprite = defaultSprite;
            tarImage.sprite = defaultSprite;
            order = "";
            transform.Find("Order Dropdown").GetComponent<TMP_Dropdown>().value = 0;
        }
    }
}
