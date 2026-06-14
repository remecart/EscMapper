// ShadowStencilClear.cs
// Attach this to the same GameObject as your shadow Tilemap.
//
// Unity doesn't automatically clear the stencil buffer between frames
// in all configurations. This script injects a Camera.onPreRender callback
// that clears the stencil so the shadow shader starts fresh every frame.
//
// SETUP:
//   1. Add this component to your shadow Tilemap GameObject (or any active GO).
//   2. Assign the camera that renders your tilemap (usually Main Camera).

using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class ShadowStencilClear : MonoBehaviour
{
    [Tooltip("The camera rendering your tilemap. Defaults to Camera.main.")]
    public Camera targetCamera;

    private CommandBuffer _cmd;

    void OnEnable()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null) return;

        _cmd = new CommandBuffer { name = "Clear Shadow Stencil" };
        // Clear only the stencil (depth=false, color=false, stencil=true, value=0)
        _cmd.ClearRenderTarget(false, false, Color.clear, 0f);

        targetCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _cmd);
    }

    void OnDisable()
    {
        if (targetCamera != null && _cmd != null)
            targetCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _cmd);

        _cmd?.Dispose();
        _cmd = null;
    }

    void OnValidate()
    {
        // Re-hook if the camera reference changes in the Inspector.
        OnDisable();
        OnEnable();
    }
}
