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

        Vector2 offset = new Vector2(-5f, -15f);
        tooltipRect.anchoredPosition = localPoint + offset;

        GameObject hovered = GetHoveredUIElement();
        if (hovered != null && hovered.CompareTag(hoverTag))
        {
            tooltipText.text = hovered.name;
            tooltipInstance.SetActive(true);
        }
        else
        {
            tooltipInstance.SetActive(false);
        }
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