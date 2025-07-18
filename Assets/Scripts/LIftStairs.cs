using UnityEngine;

public class LIftStairs : MonoBehaviour
{
    public Animator liftAnimation;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            liftAnimation.SetTrigger("ShouldLiftStairs");
            Debug.Log("Stairs activated");
        }
    }
}
