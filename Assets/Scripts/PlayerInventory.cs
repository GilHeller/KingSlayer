using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasWeapon;
    public GameObject weapon;

    public void AddWeapon()
    {
        print("AddWeapon called");
        hasWeapon = true;
        weapon.GetComponent<Renderer>().enabled = true;
    }

    public void Start()
    {
        hasWeapon = false;
        weapon.GetComponent<Renderer>().enabled = false;

        // FIXME: Remove this
         hasWeapon = true;
        weapon.GetComponent<Renderer>().enabled = true;
    }   
}
