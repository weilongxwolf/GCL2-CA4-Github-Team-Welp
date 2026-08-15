using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    public float baseDamage = 50f;
    public float sweetSpotMultiplier = 4f;

    void OnCollisionEnter(Collision collision)
    {
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            float damage = baseDamage;

            if (collision.collider.CompareTag("Head"))
            {
                damage *= sweetSpotMultiplier;
                Debug.Log("Headshot!");
            }

            targetHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
