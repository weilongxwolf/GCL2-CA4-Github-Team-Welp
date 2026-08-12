using UnityEngine;

public class SpreadShot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float bulletPower;
    public float shotsPerSecond = 1f;
    private float fireRate;
    private float nextFireTime = 0f;

    public int maxAmmo = 8;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireRate = 1f / shotsPerSecond;
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Fire();
        }
    }
    void Fire()
    {
        currentAmmo--;

        int bulletCount = 8;
        float spreadAngle = 10f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = ((float)i - (bulletCount - 1) / 2f) * spreadAngle;

            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, angle, 0);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = rotation * Vector3.forward * bulletSpeed;
        }
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
