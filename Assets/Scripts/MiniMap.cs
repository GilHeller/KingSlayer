using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private Transform player;

    void LateUpdate()
    {
        Debug.Log("Updating MiniMap position and rotation");
        if (player == null && GameManager.Instance != null && GameManager.Instance.gameData != null && GameManager.Instance.gameData.activePlayer != null)
        {
            player = GameManager.Instance.gameData.activePlayer?.transform;
        }
        
        if (player == null)
        {
            Debug.LogWarning("Player transform not found for MiniMap.");
            return; 
        }
         // Set the position of the minimap to the player's position, but keep the height constant
        Vector3 newPosition = player.position;
        newPosition.y = player.position.y + 2; // Keep the minimap at a fixed height
        transform.position = newPosition;

        // Rotate the minimap to always face upwards
        // transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
