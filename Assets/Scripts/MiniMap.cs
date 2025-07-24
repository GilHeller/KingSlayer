using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        Debug.Log("Updating MiniMap position and rotation");
        if (player != null)
        {
            // Set the position of the minimap to the player's position, but keep the height constant
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y; // Keep the minimap at a fixed height
            transform.position = newPosition;

            // Rotate the minimap to always face upwards
            // transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
