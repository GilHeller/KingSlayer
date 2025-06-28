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
            Debug.Log("DoorCollider: OnTriggerEnter called");
            doorAnimator.SetTrigger("Open");
            if (doorAudio != null) doorAudio.Play(); 
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
