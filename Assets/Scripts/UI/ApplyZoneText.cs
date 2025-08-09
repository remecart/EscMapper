using TMPro;
using UnityEngine;

public class ApplyZoneText : MonoBehaviour
{
    private TextMeshPro text => GetComponent<TextMeshPro>();

    void Start()
    {
        text.text = gameObject.transform.parent.gameObject.name;
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().sortingOrder = 700;
        }
    }
    
    void LateUpdate()
    {
        SetPosition();
        SetWorldScale(text.transform, new Vector3(0.16f, 0.16f, 1f));
    }

    public static void SetWorldScale(Transform transform, Vector3 targetWorldScale)
    {
        Vector3 currentWorldScale = transform.lossyScale;
        transform.localScale = new Vector3(
            targetWorldScale.x / currentWorldScale.x * transform.localScale.x,
            targetWorldScale.y / currentWorldScale.y * transform.localScale.y,
            targetWorldScale.z / currentWorldScale.z * transform.localScale.z
        );
    }

    public void SetPosition()
    {
        text.alignment = TextAlignmentOptions.TopLeft;
        Renderer parentRenderer = transform.parent?.GetComponent<Renderer>();
        if (parentRenderer == null) return;

        Bounds bounds = parentRenderer.bounds;
        Vector3 topLeftWorld = new Vector3(
            bounds.min.x,
            bounds.max.y,
            bounds.center.z
        );

        Vector3 offset = new Vector3(0.6f, -0.45f, 0f);
        transform.position = topLeftWorld + offset;
    }
    

}
