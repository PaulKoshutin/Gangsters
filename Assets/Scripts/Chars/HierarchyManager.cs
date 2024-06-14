using UnityEngine;

public class HierarchyManager : MonoBehaviour
{
    public static HierarchyManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
        private void Start()
    {
        transform.Find("Hierarchy Panel").Find("Viewport").Find("Content").Find("Top").Find("Main").Find("CharSlotMain").GetChild(0).GetComponent<DraggableChar>().SetChar(ActiveEntities.Instance.orgs[0].active[0]);
        transform.Find("Hierarchy Panel").Find("Viewport").Find("Content").Find("Top").Find("Main").Find("CharSlotMain").GetComponent<OrgComposite>().officeholder = ActiveEntities.Instance.orgs[0].active[0];
    }
    public void AddToReserve(Char c)
    {
        GameObject newReserveChar = Instantiate(Resources.Load("Prefabs/Char", typeof(GameObject)), parent: transform.Find("Reserve Panel").Find("Scroll View").Find("Viewport").Find("Content")) as GameObject;
        newReserveChar.GetComponent<DraggableChar>().SetChar(c);
    }
}
