using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableChar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image image;
    CanvasGroup group;
    public Transform parentAfterDrag;
    public Char c;

    private void Start()
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

        if (parentAfterDrag.gameObject.GetComponent<OrgComponent>().superior != null)
            c.superior = parentAfterDrag.gameObject.GetComponent<OrgComponent>().superior.officeholder.name;
        if (parentAfterDrag.gameObject.GetComponent<OrgComposite>().subordinates != null)
            foreach (OrgComponent oc in parentAfterDrag.gameObject.GetComponent<OrgComposite>().subordinates)
                c.subordinates.Add(oc.officeholder.name);
        parentAfterDrag.gameObject.GetComponent<OrgComponent>().officeholder = c;

        group.alpha = 1f;
        image.raycastTarget = true;
    }
    public void SetChar(Char c)
    {
        this.c = c;
        this.image.sprite = c.image;
    }
    private void ShowOnLeftPanel(Char c)
    {
        LeftPanel.Instance.Char(c);
    }
}