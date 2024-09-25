using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class LeviathanScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player, planet;
    public Rigidbody leviathan;
    private float movementSpeed = 10;
    public bool isSwimming = false;
    private void Start()
    {
        StartCoroutine(Move());
    }

    void Update ()
    {
        //agent.SetDestination(player.position);
        if (isSwimming)
        {
            Swim();
        }
    }

    void Swim()
    {
        Vector3 directionHaut = (planet.position - transform.position).normalized;
        if (Mathf.Sqrt(Mathf.Pow(transform.position.x - transform.position.x, 2) +
                                                  Mathf.Pow(transform.position.y - planet.position.y, 2) +
                                                  Mathf.Pow(transform.position.z - planet.position.z, 2)) < 803)
        {
            leviathan.position += transform.up * Time.deltaTime * 15;
        }
        leviathan.AddForce(leviathan.velocity * -2);
    }

    IEnumerator Move()
    {
        agent.speed = 8f;
        yield return new WaitForSeconds(1.3f);
        agent.speed = 1f;
        yield return new WaitForSeconds(1.3f);
        StartCoroutine(Move());
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name.Contains("Water"))
        {
            isSwimming = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.name.Contains("Water"))
        {
            isSwimming = false;
        }
    }
}
