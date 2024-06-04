using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrgSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            // Commander assignment
            if (transform.parent.name == "Main" && transform.parent.childCount > 1)
            {
                Transform sub = transform.parent.parent.Find("Sub");
                if (sub.GetChild(0).childCount == 0)
                {
                    GameObject newTop = Instantiate(Resources.Load("Prefabs/Top", typeof(GameObject)), parent:sub) as GameObject;
                    eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = newTop.transform.Find("Main").Find("Char1");
                    sub.GetComponent<GridLayoutGroup>().padding.top = 150;
                    sub.GetComponent<GridLayoutGroup>().padding.right = 200;
                    Destroy(sub.GetChild(0).gameObject);
                }
                else if (sub.childCount == 1)
                {
                    GameObject newTop = Instantiate(Resources.Load("Prefabs/Top", typeof(GameObject)), parent: sub) as GameObject;
                    eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = newTop.transform.Find("Main").Find("Char1");
                    sub.GetComponent<GridLayoutGroup>().padding.right = 0;
                    sub.GetChild(0).GetComponent<GridLayoutGroup>().padding.right = 250;
                    sub.GetChild(1).GetComponent<GridLayoutGroup>().padding.left = 0;
                }
                else if (sub.childCount == 2)
                {
                    GameObject newTop = Instantiate(Resources.Load("Prefabs/Top", typeof(GameObject)), parent: sub) as GameObject;
                    eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = newTop.transform.Find("Main").Find("Char1");
                    sub.GetComponent<GridLayoutGroup>().padding.right = 220;
                    sub.GetChild(0).GetComponent<GridLayoutGroup>().padding.right = 250;
                    sub.GetChild(1).GetComponent<GridLayoutGroup>().padding.left = 0;
                    sub.GetChild(2).GetComponent<GridLayoutGroup>().padding.left = 250;
                    Destroy(gameObject);
                }
            }
            // Squad reinforcement
            else if (transform.parent.name == "Sub" && transform.parent.childCount > 1)
            {
                if (transform.parent.childCount < 4)
                {
                    GameObject newChar = Instantiate(Resources.Load("Prefabs/Char", typeof(GameObject))) as GameObject;
                    newChar.transform.SetParent(transform.parent);
                }                  
                eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
            }
            // Squad creation
            else if (transform.parent.name == "Sub" && transform.parent.childCount == 1)
            {
                eventData.pointerDrag.GetComponent<DraggableItem>().parentAfterDrag = transform;
                transform.parent.GetComponent<GridLayoutGroup>().padding.top = 50;
                transform.parent.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
                GameObject newChar = Instantiate(Resources.Load("Prefabs/Char", typeof(GameObject)), parent:transform.parent) as GameObject;
                Destroy(transform.parent.parent.Find("Main").GetChild(1).gameObject);
            }
        }
    }
}