using UnityEngine;

[ExecuteAlways]
public class ScaleWithResolutionHeight : MonoBehaviour
{
    public float referenceHeight = 1080f; // your design resolution height
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale; // remember original scale
    }

    void Update()
    {
        float scaleFactor = (float)Screen.height / referenceHeight;
        transform.localScale = baseScale * scaleFactor;
    }
}