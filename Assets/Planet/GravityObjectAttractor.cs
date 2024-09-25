
using UnityEngine;
using System.Collections;

public class GravityObjectAttractor : MonoBehaviour
{

    public float gravity = -9.8f;


    public void AttractObject(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 localUp = body.transform.up;
        body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
    }
}