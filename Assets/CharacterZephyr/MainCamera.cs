using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GravityBody))]
public class MainCamera : MonoBehaviour
{

    // public vars
    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;

    bool grounded;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    public float verticalLookRotation, horizontalLookRotation;
    public Transform cam;
    public Rigidbody rb;
    public Transform focus;
    Transform activeTransform;
    float shake = 0f;
    bool isShaking = false;

    public Transform cutsceneManager;
    public bool inCutscene = false;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        if (!inCutscene && !transform.GetComponent<Zeph>().isShaking)
        {
            activeTransform = transform.GetComponent<Zeph>().activeTransform;
            horizontalLookRotation += Input.GetAxis("Mouse X") * mouseSensitivityY;
            verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
            if (!transform.GetComponent<Zeph>().isFlying)
            {
                if (!transform.GetComponent<Zeph>().isSwimming)
                {
                    verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
                }
                else
                {
                    verticalLookRotation = Mathf.Clamp(verticalLookRotation, -15, 60);
                }
            }
            focus.localEulerAngles = Vector3.left * verticalLookRotation + Vector3.up * horizontalLookRotation;
            if (transform.GetComponent<Zeph>().isRotating)
            {
                if (transform.GetComponent<Zeph>().isFlying)
                {
                    activeTransform.localEulerAngles = focus.localEulerAngles;
                }
                else
                {
                    activeTransform.localEulerAngles = new Vector3(0, focus.localEulerAngles.y, 0);
                }
            }
            cam.localEulerAngles = focus.localEulerAngles;
            cam.position = focus.position - focus.forward * 7;
        }
    }

    public void PlayCutscene(string triggerName, int waitTime)
    {
        switch (triggerName)
        {

            case "TriggerReynal":
                StartCoroutine(PlayAnimator(waitTime, 1));
                break;
            case "TriggerReynal3":
                StartCoroutine(PlayAnimator(waitTime, 2));
                break;
            case "TriggerRope":
                StartCoroutine(PlayAnimator(waitTime, 3));
                break;
            case "TriggerClimb":
                StartCoroutine(PlayAnimator(waitTime, 4));
                break;
        }
    }

    IEnumerator PlayAnimator(float time, int index)
    {
        inCutscene = true;
        cam.GetComponent<Animator>().enabled = true;
        cam.GetComponent<Animator>().Play(index.ToString(), -1, 0f);
        yield return new WaitForSeconds(time);
        cam.GetComponent<Animator>().enabled = false;
        inCutscene = false;
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
}