using System;
using UnityEngine;

public class ZombieChase : MonoBehaviour
{
    public Transform player;
    public float speed = 3.0f;
    public float stoppingDistance = 1.5f;


    private void Start()
    {
        Debug.Log(player);
    }
    void Update()
    {
        if (player == null)
        {
            Debug.Log("Player not found!");
            return;
        }

        // Calculate direction to player
        Vector3 direction = (player.position - transform.position).normalized;

        // Face the player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Move toward the player if far enough
        if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
