using UnityEngine;
using System;
using UnityEngine.SceneManagement;



public class CastleCollider : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene("PlayerWin");
        }
    }
}
