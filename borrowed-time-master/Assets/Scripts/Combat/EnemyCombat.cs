using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public float Health;

    protected Rigidbody2D rb2D;

    public LayerMask attackLayer;

    public float knockbackamount;

    public Transform attackpoint;

    public float attackRange = 0.5f;

    //for red flash
    static float flashRedOnDamageDuration = 0.08f;
    static float flashClock = 0;

    private void Update()
    {
        // added clock for red flash
        // if (flashClock > 0)
        // {
        //     flashClock -= Time.deltaTime;
        // }
        // else
        // {
        //     if (this.GetComponent<Renderer>().material.color == Color.red)
        //     {   
        //     this.GetComponent<Renderer>().material.color = Color.white;
        //     }
        // }dd

        if (flashClock >= flashRedOnDamageDuration) {
            this.GetComponent<Renderer>().material.color = Color.white;
        }
        flashClock += Time.deltaTime;
    }



    public void TakeDamage(float delta)
    {
        Health -= delta;
        FlashRed();

        gameObject.GetComponent<EnemyBehavior>().HurtEnemy(0.8f);

        if (Health <= 0)
        {
            // Call EnemyBehavior.Kill() to handle object destruction
            gameObject.GetComponent<EnemyBehavior>().Kill();
        }
    }

    protected Vector2 GetDirection(GameObject attacker)
    {
        Vector2 pos = this.transform.position;
        Vector2 target = attacker.transform.position;
        return (target - pos).normalized;
    }

    public void Knockback(GameObject attacker)
    {
       
        rb2D = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2D.velocity = -GetDirection(attacker) * knockbackamount;
    }

    public bool Attack()
    {
        Collider2D[] hitEnimies = Physics2D.OverlapCircleAll(attackpoint.position, attackRange, attackLayer);

        foreach (Collider2D enemy in hitEnimies)
        {
            if (enemy.gameObject == GameObject.Find("Player"))
            {
                PlayerManager.DamagePlayer(this.transform.position);
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackpoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackpoint.position, attackRange);
    }

    //function for flashing red
    public void FlashRed()
    {

        flashClock = 0;

        this.GetComponent<Renderer>().material.color = Color.red;
    }
}
