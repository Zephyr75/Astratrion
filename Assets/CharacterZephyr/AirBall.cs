using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBall : MonoBehaviour
{
    public GameObject king, spark;

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 20;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name.Contains("mixamo") && col.transform.root.gameObject.name != "Zephyr")
        {
            king.GetComponent<KingBoss>().SetDamage(1, col.name);
            Instantiate(spark, transform.position, new Quaternion(0, 0, 0, 0));
            gameObject.SetActive(false);
        }
    }
}
