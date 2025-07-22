using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentEventSystem : MonoBehaviour
{
    private void Awake()
    {
        // Ensure only one instance exists
        if (FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
