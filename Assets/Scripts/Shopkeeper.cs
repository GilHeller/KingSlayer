using UnityEngine;
public class Shopkeeper : MonoBehaviour
{
    [Header("Shopkeeper Settings")]
    public string welcomeMessage = "Welcome to my weapon shop!";
    public float interactionRange = 3f;
    public KeyCode interactionKey = KeyCode.E;
    
    private bool playerInRange = false;
    private WeaponStoreUI storeUI;
    
    void Start()
    {
        storeUI = FindObjectOfType<WeaponStoreUI>();
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            OpenStore();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log(welcomeMessage);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    void OpenStore()
    {
        if (storeUI != null)
        {
            storeUI.gameObject.SetActive(true);
        }
    }
}