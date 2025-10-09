using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorZoom : MonoBehaviour
{
    public float zoomSpeed = 1f;
    public float panSpeed = 1f;

    private Vector3 getScreenPos;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (PreventPlaceBehindGUI.instance.behindUI) return;

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            float size = Camera.main.orthographicSize;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float newSize = Mathf.Clamp(size - scroll * zoomSpeed, 4f, 54f);
            float zoomFactor = size / newSize;

            Vector3 direction = mouseWorldPos - Camera.main.transform.position;
            Camera.main.transform.position += direction * (1 - 1 / zoomFactor);

            Vector3 clampedPos = Camera.main.transform.position;

            var floorSize = new Vector2(TextureManagement.instance.groundTexture.sprite.texture.width / 16f, TextureManagement.instance.groundTexture.sprite.texture.height / 16f);

            clampedPos.x = Mathf.Clamp(clampedPos.x, -15, floorSize.x + 15);
            clampedPos.y = Mathf.Clamp(clampedPos.y, -floorSize.y - 15, 10);
            Camera.main.transform.position = clampedPos;

            Camera.main.orthographicSize = newSize;
        }
        if (Input.GetMouseButtonDown(2))
        {
            getScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position + getScreenPos - currentPos;
            
            var floorSize = new Vector2(TextureManagement.instance.groundTexture.sprite.texture.width / 16f, TextureManagement.instance.groundTexture.sprite.texture.height / 16f);
            
            transform.position = new Vector3(
                Mathf.Clamp(position.x, -15, floorSize.x + 15),
                Mathf.Clamp(position.y, -floorSize.y - 15, 10),
                transform.position.z
            );
        }
    }
}