using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // zombie stats
    public int health = 100;
    public float detectionRange = 10f;
    public float attackDistance = 3f;
    public float attackInterval = 2f;
    public float stumbleDuration = 1.5f;
    public int attackDamage = 10; // damage zombie deals

    // Rerference
    public Transform Player;

    private NavMeshAgent Agent;
    Animator anim;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isChasing = false;
    private bool isKnockedBack = false;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (Player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) Player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (isDead || isKnockedBack || Player == null) return;

        float Distance = Vector3.Distance(transform.position, Player.position);

        if (Distance <= detectionRange)
        {
            // handle chase
            isChasing = true;

            // Omly move if not in attacking
            if (!isAttacking)
            {
                Agent.isStopped = false;
                Agent.SetDestination(Player.position);
                anim.SetBool("isWalking", true);
            }

            // handle attack
            if (Distance <= attackDistance && !isAttacking)
            {
                StartCoroutine(PlayAttackAnimation());
            }
        }
        else if (isChasing)
        {
            // Lost Player
            isChasing = false;
            if (Agent.hasPath) Agent.ResetPath();
            anim.SetBool("isWalking", false);
        }
    }

    IEnumerator PlayAttackAnimation()
    {
        isAttacking = true;
        Agent.isStopped = true;
        if (Agent.hasPath) Agent.ResetPath(); // clear path

        anim.SetBool("isWalking", false);
        anim.SetTrigger("Attack");

        // Wait brief moment for the animation swing to hit the screen
        yield return new WaitForSeconds(1f);

        // Check if player is still in range during the hit window
        if (Player != null && Vector3.Distance(transform.position, Player.position) <= attackDistance + 0.5f)
        {
            // FInd Game Manager
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                // This calls your GameManager's health function directly!
                gm.ChangeHealth(-attackDamage);
                Debug.Log("[Enemy AI] Swing connected! Sent -10 damage to GameManager.");
            }
        }

        // Wait out the remainder of the attack cooldown
        yield return new WaitForSeconds(attackInterval - 1f);

        isAttacking = false;

    }

    public void Hit(int Damage)
    {
        if (isDead) return;

        health -= Damage;

        if (health <=0)
        {
            Die();
        }
        else
        {
            if (!isKnockedBack) StartCoroutine(PlayHitAnimation());

        }
    }

    IEnumerator PlayHitAnimation()
    {
        anim.SetTrigger("Hit");
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Route collision damage through the central Hit function
        if (collision.gameObject.CompareTag("Damage"))
        {
            Hit(25);
        }
    }

    void Die()
    {
        isDead = true;
        if (Agent.isActiveAndEnabled)
        {
            Agent.isStopped = true;
            if (Agent.hasPath) Agent.ResetPath();
            Agent.enabled = false; // Disable pathfinding completely
        }

        anim.SetBool("isWalking", false);
        anim.SetTrigger("dead");

        // Disable components so corpse doesn't block player or navigation
        Agent.enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    public void Stumble(Vector3 pushDirection, float force)
    {
        if (isDead) return;

        // Interrupt regular attack windups if shoved backwards
        StopCoroutine(nameof(PlayAttackAnimation));
        isAttacking = false;

        StartCoroutine(PlayStumbleAnimation(pushDirection, force));
    }

    IEnumerator PlayStumbleAnimation(Vector3 direction, float force)
    {
        isKnockedBack = true;

        if (Agent.isActiveAndEnabled)
        {
            Agent.isStopped = true;
            if (Agent.hasPath) Agent.ResetPath();
        }

        anim.SetBool("isWalking", false);
        anim.SetTrigger("Stumbling");

        float elapsed = 0f;
        while (elapsed < stumbleDuration)
        {
            // Calculate declining knockback velocity arc curve
            float currentForce = force * (1f - (elapsed / stumbleDuration));

            if (Agent.isActiveAndEnabled)
            {
                Agent.Move(direction * currentForce * Time.deltaTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;

        // Give the Enemy its path targeting back immediately upon recovery
        if (Agent.isActiveAndEnabled && Player != null)
        {
            Agent.isStopped = false;
            Agent.SetDestination(Player.position);
        }
    }
}
