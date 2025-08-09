using UnityEngine;
using UnityEngine.UI;

public class GetObjectTexture : MonoBehaviour
{
    public int id;

    public void GetTexture()
    {
        gameObject.GetComponent<RawImage>().texture = TextureManagement.instance.ReturnObject(id,
            ObjectLookupTable.instance.objects[id].GetComponent<CustomObject>().rect);
    }
}
