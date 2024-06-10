using System.Collections;
using System.Collections.Generic;
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
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        group.alpha = .5f;
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);

        group.alpha = 1f;
        image.raycastTarget = true;
    }
    public void SetChar(Char c)
    {
        this.c = c;
        this.image.sprite = c.image;
    }
    public void Fire()
    {
        c.Fire();
        transform.parent.GetComponent<OrgComponent>().superior.officeholder.subordinates.Remove(c.name);
        if (transform.parent.name == "CharSlotMain")
            foreach (OrgComponent sub in transform.parent.GetComponent<OrgComposite>().subordinates)
                sub.officeholder.superior = "";
        transform.parent.GetComponent<OrgComponent>().officeholder = null;
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