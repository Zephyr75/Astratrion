using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public GameObject zeph, king;

    private void OnTriggerEnter(Collider col)
    {
        if (col.name.Contains("mixamo") && col.transform.root.gameObject.name != "Zephyr")
        {
            king.GetComponent<KingBoss>().SetDamage(zeph.GetComponent<Zeph>().attackDmg, col.name);
            if (zeph.GetComponent<Zeph>().attackDmg > 0)
            {
                StartCoroutine(zeph.GetComponent<Zeph>().Shake(0, .7f));
            }
        }
    }
}
