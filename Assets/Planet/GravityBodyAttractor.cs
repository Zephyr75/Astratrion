
using UnityEngine;
using System.Collections;

public class GravityBodyAttractor : MonoBehaviour
{

    public float gravity = -9.8f;


    public void Attract(Rigidbody body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 localUp = body.transform.up;
        //Debug.Log(body.name);
        if (body.name == "Zephyr")
        {
            if (!body.GetComponent<Zeph>().isFlying &&
                !body.GetComponent<Zeph>().isSwimming &&
                !body.GetComponent<Zeph>().isClimbing &&
                !body.GetComponent<Zeph>().animatorRig.GetBool("Jump"))
            {
                body.AddForce(gravityUp * gravity);
            }
        }
        else if (body.name == "Leviathan")
        {
            if (!body.GetComponent<LeviathanScript>().isSwimming)
            {
                body.AddForce(gravityUp * gravity);
            }
        }
        else
        {
            body.AddForce(gravityUp * gravity);
        }
        body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
    }
}