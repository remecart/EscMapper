using UnityEngine;

public class ResizeZone : MonoBehaviour
{
    public Vector2 direction;
    private Vector3 oldCorner;
    private Vector3 oldSize;
    private bool _dragging = false;
    
    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        GameObject cornerHit = null;
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.name == "Corner")
            {
                cornerHit = hit.collider.gameObject;
                break;
            }
        }
        
        if (cornerHit == null  && !_dragging) return;
        if (cornerHit != this.gameObject && !_dragging) return;

        if (Input.GetMouseButtonDown(0))
        {
            _dragging = true;
            oldSize = transform.parent.localScale;

            // Calculate the opposite corner from the direction
            Vector3 center = transform.parent.position;
            Vector3 halfSize = oldSize * 0.5f;
            oldCorner = center - new Vector3(halfSize.x * direction.x, halfSize.y * direction.y, 0f);
        }

        if (_dragging)
        {
            Vector3 delta = mouseWorld - oldCorner;

            float newWidth = Mathf.Abs(delta.x) > 1 ? Mathf.Abs(delta.x) : 1;
            float newHeight = Mathf.Abs(delta.y) > 1 ? Mathf.Abs(delta.y) : 1;

            Vector3 newSize = new Vector3(Mathf.RoundToInt(newWidth), Mathf.RoundToInt(newHeight), 1f);
            transform.parent.localScale = newSize;
            

            // Reposition to keep the dragged corner fixed
            Vector3 newCenter = oldCorner + new Vector3(newSize.x * 0.5f * direction.x, newSize.y * 0.5f * direction.y, 0f);
            transform.parent.position = newCenter;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
        }
    }
    
    void LateUpdate()
    {
        // Maintain constant world scale
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );
    }
}
