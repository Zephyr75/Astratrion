using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour {

    public float gravity = -12;
    private bool isFlying = false;

    public void Attract(Transform player)
    {
        if (player.name == "Zephyr")
        {
            isFlying = player.gameObject.GetComponent<Zeph>().isFlying;
        }
        
        if (!isFlying || player.name != "Zephyr")
        {
            Vector3 gravityUp = (player.position - transform.position).normalized;
            Vector3 localUp = player.up;

            player.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

            Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * player.rotation;
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, 50f * Time.deltaTime);
        }
    }
}
