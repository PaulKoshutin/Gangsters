using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyManager : MonoBehaviour
{
    private void Start()
    {
        transform.Find("Hierarchy Panel").Find("Viewport").Find("Content").Find("Top").Find("Main").Find("CharSlotMain").GetChild(0).GetComponent<DraggableChar>().SetChar(ActiveEntities.Instance.orgs[0].leader);
    }

}
