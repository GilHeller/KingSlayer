using UnityEngine.VFX;

using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int health = 100;
    // public GameObject vfxPrefab;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Zombie hit! Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
