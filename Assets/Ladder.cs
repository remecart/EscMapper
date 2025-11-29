using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public bool up;
    public GameObject visual;
    public GameObject thing;

    void Start()
    {
        // Start a coroutine instead of running immediately
        StartCoroutine(DelayedInit());
    }

    public bool FindLadder(int layerIndex, Vector2 position)
    {
        var layer = this.transform.parent.parent.parent.GetChild(layerIndex).GetChild(3);

        foreach (Transform child in layer.transform)
        {
            GameObject ladderObject = child.gameObject;

            if (Vector2.Distance(position, ladderObject.transform.position) < 0.01f)
            {
                var obj = ladderObject.GetComponent<CustomObject>();
                if (obj != null && (obj.id == 52 || obj.id == 53))
                {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator DelayedInit()
    {
        // Wait one frame
        yield return null;

        while (true) // infinite loop
        {
            Destroy(thing);
            var go = Instantiate(visual);

            if (transform.parent.parent.name == "Ground" && up)
            {
                if (FindLadder(2, new Vector2(this.transform.position.x, this.transform.position.y + 1)))
                {
                    go.transform.SetParent(this.transform.parent.parent.parent.GetChild(2).GetChild(0));
                    go.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1,
                        this.transform.position.z);
                }
                else
                {
                    go.transform.SetParent(this.transform.parent.parent.parent.GetChild(3).GetChild(0));
                    go.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1,
                        this.transform.position.z);
                }
            }

            if (transform.parent.parent.name == "Vent" && up)
            {
                go.transform.SetParent(this.transform.parent.parent.parent.GetChild(3).GetChild(0));
                go.transform.position = this.transform.position;
            }

            if (transform.parent.parent.name == "Rooftop" && !up)
            {
                go.transform.SetParent(this.transform.parent.parent.parent.GetChild(2).GetChild(0));
                go.transform.position = this.transform.position;
            }

            if (transform.parent.parent.name == "Vent" && !up)
            {
                go.transform.SetParent(this.transform.parent.parent.parent.GetChild(1).GetChild(0));
                go.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1,
                    this.transform.position.z);
            }

            go.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 25;

            thing = go;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDestroy()
    {
        Destroy(thing.gameObject);
    }
}