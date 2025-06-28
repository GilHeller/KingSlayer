using UnityEngine;
using System;


public class DoorCollider : MonoBehaviour
{
    public Animator doorAnimator;
    public AudioSource doorAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            doorAnimator.SetTrigger("Open");
            if (doorAudio != null) doorAudio.Play();

            // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.health = Math.Max(100, playerHealth.health * 2);
            //     playerHealth.UpdateHealthText();
            // }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            doorAnimator.SetTrigger("Close");
        }
    }
}
