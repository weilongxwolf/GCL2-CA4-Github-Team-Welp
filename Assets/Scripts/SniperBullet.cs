using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    public int baseDamage = 50;
    public float sweetSpotMultiplier = 4f;

    void OnCollisionEnter(Collision collision)
    {
        Enemy targetEnemy = collision.gameObject.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            int damage = baseDamage;

            if (collision.collider.CompareTag("Head"))
            {
                damage = (int)(damage * sweetSpotMultiplier);
                Debug.Log("Headshot!");
            }

            targetEnemy.Hit(damage);
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
