using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	float rotationSpeed = 0.5f, distance = 10, yValue, xValue, yInput, xInput, zValue;
    public Transform focus;
    public GameObject player;
    public Transform noyau;
    public Transform cutsceneManager;
    public Animator animator;
    public RaycastHit newHit;
    public RaycastHit oldHit;
    bool inCutscene = false;
    int loopNbr = 0;
    public Component[] renderers;
    float verticalLookRotation;
    RaycastHit hit;
    bool wasFlying = false;


    void FixedUpdate ()
    {
        playCutscene();
    }

    void playCutscene()
    {
        if (cutsceneManager.GetComponent<CutsceneScript>().triggerName == "TriggerClimb")
        {
            StartCoroutine(PlayAnimator(40));
        }
    }

    /*void clearView()
    {
        if (Physics.Raycast(transform.position + offset, transform.forward, out newHit, 3))
        {
            if (newHit.collider.gameObject.tag != "AlwaysVisible")
            {
                if (loopNbr == 0 && newHit.collider.gameObject.name != "Zephyr")
                {
                    oldHit = newHit;
                    renderers = newHit.transform.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer hit in renderers)
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;

                    }
                    loopNbr = 1;
                }
                if (newHit.collider.gameObject.name != oldHit.collider.gameObject.name || newHit.collider.gameObject.name == "Zephyr")
                {
                    if (newHit.collider.gameObject.name != "Zephyr")
                    {
                        renderers = newHit.transform.GetComponentsInChildren<Renderer>();
                        foreach (Renderer hit in renderers)
                        {
                            hit.transform.gameObject.GetComponent<Renderer>().enabled = false;

                        }
                    }

                    renderers = oldHit.transform.GetComponentsInChildren<Renderer>();
                    foreach (Renderer hit in renderers)
                    {
                        hit.transform.gameObject.GetComponent<Renderer>().enabled = true;

                    }
                    oldHit = newHit;
                }
            }

        }
    }*/

    IEnumerator PlayAnimator(float time)
    {
        inCutscene = true;
        gameObject.GetComponent<Animator>().SetFloat("Blend", 1);
        gameObject.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<Animator>().enabled = false;
        inCutscene = false;
    }
}
