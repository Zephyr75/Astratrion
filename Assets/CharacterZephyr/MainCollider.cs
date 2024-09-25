using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCollider : MonoBehaviour
{
    public GameObject zephyr;

    private void OnTriggerStay(Collider col)
    {
        if (col.name.Contains("Water"))
        {
            zephyr.GetComponent<Zeph>().isSwimming = true;
        }
        /*if (col.gameObject.name == "Terrain" && isFalling)
        {
            StartCoroutine(ActivateAnimation("Roll", 1.3f));
            isFalling = false;
        }*/
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.name.Contains("Water"))
        {
            zephyr.GetComponent<Zeph>().isSwimming = false;
            zephyr.GetComponent<Zeph>().newFly = 0;
        }
    }
}
