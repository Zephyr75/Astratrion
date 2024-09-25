using UnityEngine;
using System.Collections;

public class GravityObject : MonoBehaviour
{

    public GravityObjectAttractor planet;

    void FixedUpdate()
    {
        planet.AttractObject(transform);
        GetComponent<GravityObject>().enabled = false;
    }
}