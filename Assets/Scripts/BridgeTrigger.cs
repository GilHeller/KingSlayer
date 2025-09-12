using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    public BridgeOpener opener;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Make sure your player has tag "Player"
        {
            opener.TriggerOpen();
        }
    }
}
