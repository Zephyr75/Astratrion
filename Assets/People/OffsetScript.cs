using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetScript : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ActivateAnimator(Random.Range(1, 20)));
    }

    IEnumerator ActivateAnimator(float time)
    {
        yield return new WaitForSeconds(time);
        animator.enabled = true;
    }
}
