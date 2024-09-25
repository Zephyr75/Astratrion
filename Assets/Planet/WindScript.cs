using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    float limSupX, limInfX, limSupY, limInfY, limSupZ, limInfZ;
    float offset;

    void Awake()
    {
        limInfX = transform.eulerAngles.x - 5;
        limSupX = transform.eulerAngles.x + 5;
        limInfZ = transform.eulerAngles.z - 5;
        limSupZ = transform.eulerAngles.z + 5;
    }

    void Update()
    {
        offset = Random.Range(-1f, 1f);
        if (transform.eulerAngles.x + offset > limInfX && transform.eulerAngles.x + offset < limSupX)
        {
            transform.eulerAngles += new Vector3(offset, 0, 0);
        }
        offset = Random.Range(-1f, 1f);
        if (transform.eulerAngles.z + offset > limInfZ && transform.eulerAngles.z + offset < limSupZ)
        {
            transform.eulerAngles += new Vector3(0, 0, offset);
        }
    }
}
