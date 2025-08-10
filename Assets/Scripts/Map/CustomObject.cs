using UnityEngine;

public class CustomObject : MonoBehaviour
{
    public int id;
    public Rect rect;
    public Vector2 offset;

    public void Start() 
    {    
        var tex = TextureManagement.instance.ReturnObject(id, rect);
        this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 16);
        this.GetComponent<BoxCollider2D>().size = new Vector2(tex.width / 16, tex.height / 16);
    }
}