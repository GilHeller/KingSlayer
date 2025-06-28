using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
    public GameObject[] doors;

    public GameObject[] doorsWood;

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            // doors[i].GetComponent<DoorCollider>().enabled = status[i];
            doors[i].SetActive(status[i]);
            doorsWood[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
