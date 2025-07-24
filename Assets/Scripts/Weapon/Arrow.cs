using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bx;
    bool disableRotation;
    public float destroyTime = 10f;
    AudioSource arrowAudio;

    public GameObject impactEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bx = GetComponent<BoxCollider>();
        arrowAudio = GetComponent<AudioSource>();

        Destroy(this.gameObject, destroyTime);
    }
    void Update()
    {
        if(!disableRotation)
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            arrowAudio.Play();
            disableRotation = true;
            rb.isKinematic = true;
            bx.isTrigger = true;

            // Instantiate impact effect if collison raycast layer is Enemy
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impactEffect, 2f); // Destroy the effect after 2 seconds 

                // Take enemy damage
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(GameManager.Instance.gameData.currentEquippedWeapon.damage);
                }

            }
        }

    }
}
