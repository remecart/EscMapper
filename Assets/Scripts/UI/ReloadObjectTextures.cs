using UnityEngine;

public class ReloadObjectTextures : MonoBehaviour
{
    public static ReloadObjectTextures instance;

    private void Start()
    {
        instance = this;
    }

    public void ReloadTextures()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Possible
        foreach (Transform page in this.transform)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Possible
            foreach (Transform entry in page)
            {
                entry.GetComponent<GetObjectTexture>().GetTexture();
            }
        }
    }
}
