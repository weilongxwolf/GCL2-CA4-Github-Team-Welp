using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Gun gun;
    public Transform gunHolder;
    private bool isHoldingShoot = false;

    void OnShoot()
    {
        isHoldingShoot = true;
    }

    void OnShootRelease()
    {
        isHoldingShoot = false;
    }

    void OnReload()
    {
        if(gun != null)
        {
            gun.TryReload();
        }
    }
  
    void Update()
    {
        if (isHoldingShoot && gun != null)
        {
            gun.Shoot();
        }
    }
    void PickupGun(Gun newGun)
    {
        if (gun != null)
            gun.Drop(); // drop current gun first

        gun = newGun;
        gun.transform.SetParent(gunHolder);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;

        Rigidbody rb = gun.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void OnDrop()
    {
        if(gun != null)
        {
            gun.Drop();
            gun = null;
        }
    }
}
