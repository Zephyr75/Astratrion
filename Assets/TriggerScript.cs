using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public bool isActive = false, forCutscene;
    public string colName;
    public int cutsceneTime;

    private void OnTriggerEnter(Collider col)
    {
        colName = col.name;
        if (colName == "ZephCollider")
        {
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        isActive = false;
        colName = "";
    }
}
