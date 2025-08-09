using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSuddleMovement : MonoBehaviour
{
    public Vector3 changePos;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SuddleMovement());
    }

    IEnumerator SuddleMovement()
    {
        while (true)
        {
            transform.position = transform.position + changePos;

            yield return new WaitForSeconds(time);

            transform.position = transform.position - changePos;

            yield return new WaitForSeconds(time);

            yield return null;
        }
    }
}