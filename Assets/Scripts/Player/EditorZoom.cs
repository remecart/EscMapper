using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorZoom : MonoBehaviour
{
    public float zoomSpeed = 1f;
    public float panSpeed = 1f;
    public float moveSmoothTime = 0.1f; // smooth movement time

    private Vector3 getScreenPos;
    private Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private Vector3 targetInput;

    private void Update()
    {
        if (targetInput.sqrMagnitude > 1f) targetInput.Normalize();

        Vector3 targetVelocity = targetInput * panSpeed * Camera.main.orthographicSize;
        velocity = Vector3.Lerp(velocity, targetVelocity, 1 - Mathf.Exp(-Time.deltaTime / moveSmoothTime));

        Vector3 newPos = transform.position + velocity * Time.deltaTime;

        var floorSizeFinal = new Vector2(
            TextureManagement.instance.groundTexture.sprite.texture.width / 16f,
            TextureManagement.instance.groundTexture.sprite.texture.height / 16f
        );

        transform.position = new Vector3(
            Mathf.Clamp(newPos.x, -15, floorSizeFinal.x + 15),
            Mathf.Clamp(newPos.y, -floorSizeFinal.y - 15, 10),
            transform.position.z
        );

        if (PreventPlaceBehindGUI.instance.behindUI)
        {
            targetInput = new Vector3();
            return;
        }

        // --- ZOOM ---
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

            var floorSize = new Vector2(
                TextureManagement.instance.groundTexture.sprite.texture.width / 16f,
                TextureManagement.instance.groundTexture.sprite.texture.height / 16f
            );

            clampedPos.x = Mathf.Clamp(clampedPos.x, -15, floorSize.x + 15);
            clampedPos.y = Mathf.Clamp(clampedPos.y, -floorSize.y - 15, 10);
            Camera.main.transform.position = clampedPos;

            Camera.main.orthographicSize = newSize;
        }

        // --- MIDDLE MOUSE DRAG ---
        if (Input.GetMouseButtonDown(2))
        {
            getScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position + getScreenPos - currentPos;

            var floorSize = new Vector2(
                TextureManagement.instance.groundTexture.sprite.texture.width / 16f,
                TextureManagement.instance.groundTexture.sprite.texture.height / 16f
            );

            transform.position = new Vector3(
                Mathf.Clamp(position.x, -15, floorSize.x + 15),
                Mathf.Clamp(position.y, -floorSize.y - 15, 10),
                transform.position.z
            );
        }

        targetInput = new Vector3();
        // --- WASD MOVEMENT (SMOOTHED) ---
        if (Input.GetKey(KeyCode.W)) targetInput.y += 1;
        if (Input.GetKey(KeyCode.S)) targetInput.y -= 1;
        if (Input.GetKey(KeyCode.A)) targetInput.x -= 1;
        if (Input.GetKey(KeyCode.D)) targetInput.x += 1;
    }
}