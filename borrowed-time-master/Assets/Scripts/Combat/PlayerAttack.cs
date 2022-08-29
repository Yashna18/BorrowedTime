using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Cooldown time for attacking
    public float attackCooldownTime;

    // How much time is left until able to attack again 
    float attackCooldownTimeLeft = 0;

    public Transform attackpoint;
    public Transform downpoint;
    public Transform uppoint;

    public float attackRange = 0.5f;
    public float downRange = 0.5f;
    public float upRange = 0.5f;

    private AudioSource audioSource;
    
    public AudioClip swingSound;
    public AudioClip hitSound;
    public AudioClip enemyHitSound;
    
    public LayerMask enemyLayers;

    public float Damage;

    public Animator anim;

    Collider2D[] hitEnimies;
    Collider2D[] hitEnimies2 = null;

    void Start() {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldownTimeLeft > 0)
        {
            attackCooldownTimeLeft -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !PlayerManager.isThrowingTimeBomb)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (attackCooldownTimeLeft == 0)
        {
            attackCooldownTimeLeft = attackCooldownTime;

            audioSource.volume = 0.7f;
            audioSource.PlayOneShot(swingSound);
            
            anim.SetTrigger("Attack");

            if (PlayerManager.isFacing == 1)
            {
                hitEnimies = Physics2D.OverlapCircleAll(downpoint.position, downRange, enemyLayers);
            }
            else if (PlayerManager.isFacing == 2)
            {
                hitEnimies = Physics2D.OverlapCircleAll(uppoint.position, upRange, enemyLayers);
            }
            else
            {
                hitEnimies = Physics2D.OverlapCircleAll(attackpoint.position, attackRange, enemyLayers);
            }

            foreach (Collider2D enemy in hitEnimies)
            {
                enemy.GetComponent<EnemyCombat>().TakeDamage(Damage);
                enemy.GetComponent<EnemyCombat>().Knockback(this.gameObject);
                
                audioSource.volume = 0.1f;
                audioSource.PlayOneShot(enemyHitSound);
            }          
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackpoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackpoint.position, attackRange);
        Gizmos.DrawWireSphere(downpoint.position, downRange);
        Gizmos.DrawWireSphere(uppoint.position, upRange);
    }
}
