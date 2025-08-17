using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SmoothScrollRectTransform : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform targetRect;
    public float baseScrollSpeed = 5f;
    public float scrollDuration = 0.1f;
    private Vector2 targetPosition;
    private Coroutine currentScrollCoroutine;
    private float timeSinceLastScroll;
    private bool canScroll;

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && canScroll)
        {
            float dynamicSpeed = baseScrollSpeed + (1f / Mathf.Max(0.01f, timeSinceLastScroll)); // Increase speed based on frequency
            targetPosition = targetRect.anchoredPosition + new Vector2(0, -Input.mouseScrollDelta.y * dynamicSpeed);

            if (currentScrollCoroutine != null) StopCoroutine(currentScrollCoroutine);
            currentScrollCoroutine = StartCoroutine(SmoothScrollCoroutine(targetPosition));

            timeSinceLastScroll = 0f;
        }

        timeSinceLastScroll += Time.deltaTime;
    }

    public void RecalculateHeight()
    {
        var count = transform.GetChild(0).childCount;
        targetRect.sizeDelta = new Vector2(350, count * 20);
    }

    private IEnumerator SmoothScrollCoroutine(Vector2 target)
    {
        Vector2 startPosition = targetRect.anchoredPosition;
        float duration = scrollDuration;
        if (duration > timeSinceLastScroll) duration = timeSinceLastScroll;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            targetRect.anchoredPosition = Vector2.Lerp(startPosition, target, t);
            yield return null;
        }

        targetRect.anchoredPosition = target;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canScroll = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canScroll = false;
    }
}