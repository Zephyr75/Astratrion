using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class KingBoss : MonoBehaviour {
    public GameObject trigger, rubbleFx, rubbleSfx;
    public Rigidbody king, player;
    public Animator animator;
    public NavMeshAgent agent;
    public bool isActive = false;
    bool isHit = false;
    public int attackDmg = 0;
    bool canMove = false;
    float timeS = 0, timeE = 0;
    public int health;

    private void Start()
    {
        animator.Play("MovementTree", 0, 0);
    }


    void FixedUpdate () {

        if (health <= 0)
        {
            StartCoroutine(ActivateAnimation("Die", 10f));
            StartCoroutine(Die());
        }

        if (canMove && king.transform.localScale.x >= 0.00006)
        {
            king.transform.localScale -= new Vector3(0.00000005f, 0.00000005f, 0.00000005f);
        }

        isActive = trigger.gameObject.GetComponent<TriggerScript>().isActive;
        if (isActive)
        {
            StartCoroutine(Wait());
        }

        if (canMove)
        {
            agent.SetDestination(player.position);
        }

        if (Vector3.Distance(player.position, king.position) < 10 && attackDmg == 0 && canMove)
        {
            if (Random.Range(1, 3) == 1)
            {
                StartCoroutine(ActivateAnimation("Jump", 3.8f));
                StartCoroutine(SetAttack(2.1f, .9f, 2));
            }
            else
            {
                StartCoroutine(ActivateAnimation("Kick", 1f));
                StartCoroutine(SetAttack(.5f, .4f, 1));
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump1"))
        {
            StartCoroutine(PlayFx());
        }

    }

    public void SetDamage(int damage, string partHit)
    {
        if (!isHit && damage > 0)
        {
            health -= damage;
            if ((partHit == "mixamorig:Neck" || damage >= 3) && health > 0)
            {
                StartCoroutine(ActivateAnimation("Stun", 2.33f));
            }
            StartCoroutine(ResetHit());
        }
    }

    IEnumerator ResetHit()
    {
        isHit = true;
        yield return new WaitForSeconds(2f);
        isHit = false;
    }

    IEnumerator Wait()
    {
        foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        yield return new WaitForSeconds(5f);
        animator.SetFloat("Action", 1);
        transform.position += transform.forward * Time.deltaTime * 40;
        yield return new WaitForSeconds(2f);
        canMove = true;
        king.GetComponent<NavMeshAgent>().enabled = true;
        foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
    }

    IEnumerator ActivateAnimation(string name, float time)
    {
        animator.SetBool(name, true);
        yield return new WaitForSeconds(time);
        animator.SetBool(name, false);
    }

    IEnumerator SetAttack(float wait, float hold, int dmg)
    {
        yield return new WaitForSeconds(wait);
        attackDmg = dmg;
        yield return new WaitForSeconds(hold);
        attackDmg = 0;
    }

    IEnumerator PlayFx()
    {
        yield return new WaitForSeconds(1.1f);
        rubbleFx.SetActive(true);
        rubbleSfx.SetActive(true);
        yield return new WaitForSeconds(3f);
        rubbleFx.SetActive(false);
        rubbleSfx.SetActive(false);
    }

    IEnumerator Die()
    {
        canMove = false;
        attackDmg = 0;
        animator.SetFloat("Action", 2);
        yield return new WaitForSeconds(1.9f);
        animator.SetFloat("Action", 3);
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

}
