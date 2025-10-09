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
    private Canvas canvas;

    void Start()
    {
        tooltipInstance = Instantiate(tooltipPrefab, transform);
        tooltipText = tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
        tooltipRect = tooltipInstance.GetComponent<RectTransform>();
        canvas = tooltipInstance.GetComponentInParent<Canvas>();

        tooltipInstance.SetActive(false);
    }

    void Update()
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out anchoredPos
        );

        tooltipRect.anchoredPosition = anchoredPos + new Vector2(15f, -15f);
        var screenMidX = Screen.width / 2f;
        
        if (Input.mousePosition.x > screenMidX)
        {
            tooltipRect.pivot = new Vector2(1f, 1f);
        }
        else
        {
            tooltipRect.pivot = new Vector2(0f, 1f);
        }

        GameObject hovered = GetHoveredUIElement() ?? GetHovered2DWorldObject();
        if (hovered != null && hovered.CompareTag(hoverTag))
        {
            int bracketEnd = hovered.name.IndexOf("]");
            tooltipText.text = hovered.name.Substring(bracketEnd + 1).Trim().Replace("(Clone)", "");

            if (tooltipText.text.Contains("RawImage"))
            {
                tooltipInstance.SetActive(false);
                tooltipText.text = "";
            }
            else
            {
                tooltipInstance.SetActive(true);
            }
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
