using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class HoverToolboxTMP : MonoBehaviour
{
    public string hoverTag = "Hoverable";
    public GameObject tooltipPrefab;

    private GameObject tooltipInstance;
    private TextMeshProUGUI tooltipText;
    private RectTransform tooltipRect;
    private RectTransform canvasRect;

    void Start()
    {
        tooltipInstance = Instantiate(tooltipPrefab, transform);
        tooltipText = tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
        tooltipRect = tooltipInstance.GetComponent<RectTransform>();
        canvasRect = tooltipInstance.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        tooltipInstance.SetActive(false);
    }
    void Update()
    {
        RectTransform parentRect = tooltipRect.parent as RectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Input.mousePosition,
            tooltipInstance.GetComponentInParent<Canvas>().worldCamera,
            out localPoint
        );

        if (tooltipRect.anchoredPosition.x < 0) tooltipRect.pivot = new Vector2(0, -0.2f);
        else tooltipRect.pivot = new Vector2(1, -0.2f);
        
        Vector2 offset = new Vector2(-5f * ((tooltipRect.pivot.x - 0.5f) * 2), -15f);
        tooltipRect.anchoredPosition = localPoint + offset;

        GameObject hovered = GetHoveredUIElement() ?? GetHovered2DWorldObject();
        if (hovered != null && hovered.CompareTag(hoverTag))
        {
            int bracketEnd = hovered.name.IndexOf("]");
            tooltipText.text = hovered.name.Substring(bracketEnd + 1).Trim().Replace("(Clone)", "");
            tooltipInstance.SetActive(true);
        }
        else
        {
            tooltipInstance.SetActive(false);
        }
    }
    
    GameObject GetHovered2DWorldObject()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(hoverTag))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    GameObject GetHoveredUIElement()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag(hoverTag))
                return result.gameObject;
        }

        return null;
    }
}