using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour
{
    public GameObject king, zeph;

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "ZephCollider" && gameObject.tag == "Damage")
        {
            zeph.GetComponent<Zeph>().SetDamage(king.GetComponent<KingBoss>().attackDmg);
        }
    }
}
