using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class OrgComponent: MonoBehaviour, IDropHandler
{
    public Char officeholder;
    public OrgComposite superior;
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            // Commander assignment
            if (transform.parent.name == "Main" && transform.parent.childCount > 1)
            {
                Transform sub = transform.parent.parent.Find("Sub");
                GameObject newTop = Instantiate(Resources.Load("Prefabs/Top", typeof(GameObject)), parent: sub) as GameObject;
                Transform CharSlotMain = newTop.transform.Find("Main").Find("CharSlotMain");
                DraggableChar dc = eventData.pointerDrag.GetComponent<DraggableChar>();
                CharSlotMain.GetComponent<OrgComposite>().officeholder = dc.c;
                transform.parent.Find("CharSlotMain").GetComponent<OrgComposite>().Add(CharSlotMain.gameObject.GetComponent<OrgComposite>());

                dc.parentAfterDrag = CharSlotMain;
                dc.c.superior = CharSlotMain.GetComponent<OrgComposite>().superior.officeholder.name;
                CharSlotMain.GetComponent<OrgComposite>().superior.solo = false;
                CharSlotMain.GetComponent<OrgComposite>().superior.officeholder.solo = false;
                CharSlotMain.GetComponent<OrgComposite>().solo = true;
                dc.c.solo = true;
                ActiveEntities.Instance.GetOrg(dc.c.org).AddToActive(dc.c);
                CharSlotMain.GetComponent<OrgComposite>().superior.officeholder.subordinates.Add(dc.c.name);
                

                if (sub.GetChild(0).childCount == 0)
                {
                    sub.GetComponent<GridLayoutGroup>().padding.top = 150;
                    sub.GetComponent<GridLayoutGroup>().padding.right = 200;
                    Destroy(sub.GetChild(0).gameObject);
                }
                else if (sub.childCount == 2)
                {
                    sub.GetComponent<GridLayoutGroup>().padding.right = 0;
                    sub.GetChild(0).GetComponent<GridLayoutGroup>().padding.right = 250;
                    sub.GetChild(1).GetComponent<GridLayoutGroup>().padding.left = 0;
                }
                else if (sub.childCount == 3)
                {
                    sub.GetComponent<GridLayoutGroup>().padding.right = 220;
                    sub.GetChild(0).GetComponent<GridLayoutGroup>().padding.right = 250;
                    sub.GetChild(1).GetComponent<GridLayoutGroup>().padding.left = 0;
                    sub.GetChild(2).GetComponent<GridLayoutGroup>().padding.left = 250;
                    Destroy(gameObject);
                }
            }
            // Squad creation
            else if (transform.parent.name == "Sub" && transform.parent.childCount == 1)
            {
                transform.parent.GetComponent<GridLayoutGroup>().padding.top = 50;
                transform.parent.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
                GameObject newChar = Instantiate(Resources.Load("Prefabs/CharSlotLeaf", typeof(GameObject)), parent: transform.parent) as GameObject;
                GameObject newChar2 = Instantiate(Resources.Load("Prefabs/CharSlotLeaf", typeof(GameObject)), parent: transform.parent) as GameObject;
                transform.parent.parent.Find("Main").GetChild(0).GetComponent<OrgComposite>().Add(newChar.gameObject.GetComponent<OrgLeaf>());
                transform.parent.parent.Find("Main").GetChild(0).GetComponent<OrgComposite>().squadLeader = true;
                DraggableChar dc = eventData.pointerDrag.GetComponent<DraggableChar>();
                dc.parentAfterDrag = newChar.transform;
                newChar.GetComponent<OrgLeaf>().officeholder = dc.c;
                dc.c.superior = newChar.GetComponent<OrgLeaf>().superior.officeholder.name;
                newChar.GetComponent<OrgLeaf>().superior.officeholder.squadLeader = true;
                newChar.GetComponent<OrgLeaf>().superior.solo = false;
                newChar.GetComponent<OrgLeaf>().superior.officeholder.solo = false;
                ActiveEntities.Instance.GetOrg(dc.c.org).AddToActive(dc.c);
                newChar.GetComponent<OrgLeaf>().superior.officeholder.subordinates.Add(dc.c.name);
                Destroy(transform.parent.parent.Find("Main").GetChild(1).gameObject);
                Destroy(gameObject);
            }
            // Squad reinforcement
            else if (transform.parent.name == "Sub" && transform.parent.childCount > 1)
            {
                DraggableChar dc = eventData.pointerDrag.GetComponent<DraggableChar>();
                if (transform.parent.childCount < 4)
                {
                    GameObject newChar = Instantiate(Resources.Load("Prefabs/CharSlotLeaf", typeof(GameObject))) as GameObject;
                    newChar.transform.SetParent(transform.parent);
                }
                dc.parentAfterDrag = transform;
                transform.parent.parent.Find("Main").GetChild(0).GetComponent<OrgComposite>().Add(gameObject.GetComponent<OrgLeaf>());
                dc.c.superior = gameObject.GetComponent<OrgLeaf>().superior.officeholder.name;
                gameObject.GetComponent<OrgLeaf>().officeholder = dc.c;
                ActiveEntities.Instance.GetOrg(dc.c.org).AddToActive(dc.c);
            }
            // Replacement
            else if (transform.name == "CharSlotMain")
            {
                DraggableChar dc = eventData.pointerDrag.GetComponent<DraggableChar>();
                dc.parentAfterDrag = transform;
                dc.c.superior = gameObject.GetComponent<OrgComponent>().superior.officeholder.name;
                foreach (OrgComponent o in gameObject.GetComponent<OrgComposite>().subordinates)
                    dc.c.subordinates.Add(o.officeholder.name);
                gameObject.GetComponent<OrgComponent>().officeholder = dc.c;
                ActiveEntities.Instance.GetOrg(dc.c.org).AddToActive(dc.c);
            }
        }
    }
    public abstract void Add(OrgComponent c);
    public abstract void Remove(OrgComponent c);
}
