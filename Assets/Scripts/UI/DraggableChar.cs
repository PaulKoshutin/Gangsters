using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableChar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image image;
    CanvasGroup group;
    public Transform parentAfterDrag;
    public Char c;

    private void Awake()
    {
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
        gameObject.GetComponent<Button>().onClick.AddListener(delegate { ShowOnLeftPanel(c); });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ActiveEntities.Instance.orgs[0].money > c.pay)
        {
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();

            group.alpha = .5f;
            image.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ActiveEntities.Instance.orgs[0].money > c.pay)
            transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ActiveEntities.Instance.orgs[0].money > c.pay)
        {
            transform.SetParent(parentAfterDrag);

            group.alpha = 1f;
            image.raycastTarget = true;
        }
    }
    public void SetChar(Char c, bool exclusive=true)
    {
        this.c = c;
        this.image.sprite = c.image;
        if (exclusive)
            c.draggableIcon = this;
    }
    public void Fire()
    {
        if (transform.parent.name != "Content")
        {
            if (transform.parent.GetComponent<OrgComponent>().superior != null && transform.parent.GetComponent<OrgComponent>().superior.officeholder != null)
                transform.parent.GetComponent<OrgComponent>().superior.officeholder.subordinates.Remove(c.name);
            if (transform.parent.name == "CharSlotMain")
                foreach (OrgComponent sub in transform.parent.GetComponent<OrgComposite>().subordinates)
                    if (sub.officeholder != null)
                        sub.officeholder.superior = "";
            transform.parent.GetComponent<OrgComponent>().officeholder = null;
        }
        Destroy(gameObject);
    }
    private void ShowOnLeftPanel(Char c)
    {
        LeftPanel.Instance.Char(c);
    }
    private void OnEnable()
    {
        this.image.sprite = c.image;
    }
}