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

        yield return new WaitForSeconds(attackInterval);

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
            StartCoroutine(PlayHitAnimation());

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
            Hit(10);
        }
    }

    void Die()
    {
        isDead = true;
        Agent.isStopped = true;
        if (Agent.hasPath) Agent.ResetPath();

        anim.SetBool("isWalking", false);
        anim.SetTrigger("dead");

        // Disable components so corpse doesn't block player or navigation
        Agent.enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    public void Stumble(Vector3 pushDirection, float force)
    {
        if (isDead) return;
        StartCoroutine(PlayStumbleAnimation(pushDirection, force));
    }

    IEnumerator PlayStumbleAnimation(Vector3 direction, float force)
    {
        isKnockedBack = true;
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        if (Agent.hasPath) Agent.ResetPath();

        anim.SetBool("isWalking", false);
        anim.SetTrigger("Stumbling");

        float elapsed = 0f;
        while (elapsed < stumbleDuration)
        {
            float currentForce = force * (1f - (elapsed / stumbleDuration));
            Agent.Move(direction * currentForce * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
        Agent.isStopped = false;
    }
}
