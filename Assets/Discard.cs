using UnityEngine;

public class Discard : MonoBehaviour
{
    public void Close()
    {
        transform.parent.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
