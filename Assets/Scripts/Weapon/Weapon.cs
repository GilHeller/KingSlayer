using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponData weaponData;

    public void Attack()
    {
        // Implement attack logic here
        if (weaponData.damage > 0)
        {
            Debug.Log($"Attacking with {weaponData.weaponName}");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                ThirdPersonController controller = player.GetComponent<ThirdPersonController>();
                if (controller != null)
                {
                    controller.Hit(weaponData.damage);
                }
            }   
        }
        else
        {
            Debug.Log($"Blocking with {weaponData.weaponName}");

        }
    }
    public abstract void StartAiming();
    public abstract void StopAiming();
    
    protected virtual void Start()
    {
        // Weapon-specific initialization
    }
}