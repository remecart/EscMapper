using System.Collections.Generic;
using UnityEngine;

public class ZiplineJeepCheck : MonoBehaviour
{
    public List<int> searchIDs;
    public float checkDistance = 10f;
    public Vector2 direction = Vector2.right;
    public LayerMask detectionLayer;

    private LineRenderer line => this.GetComponent<LineRenderer>();

    void Update()
    {
        Vector2 origin = transform.position;

        // Use a ray with layer filtering (make sure your objects are on the correct layer)
        RaycastHit2D[] hits =
            Physics2D.RaycastAll(transform.position, direction.normalized, checkDistance, detectionLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                CustomObject co = hit.collider.GetComponent<CustomObject>();
                if (co != null && searchIDs.Contains(co.id))
                {
                    this.transform.GetChild(0).gameObject.SetActive(false);

                    line.positionCount = 2;
                    line.sortingOrder = 600;
                    line.SetPosition(0, transform.position + (Vector3)direction * 0.5f);
                    line.SetPosition(1, hit.point);
                    return;
                }
            }

            this.transform.GetChild(0).gameObject.SetActive(true);
            line.positionCount = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction.normalized * checkDistance);
    }
}