using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLookupTable : MonoBehaviour
{
    public static ObjectLookupTable instance;
    public List<GameObject> objects; 

    void Start() 
    {
        instance = this;
    }
}