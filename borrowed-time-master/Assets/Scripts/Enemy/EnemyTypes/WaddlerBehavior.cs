using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCombat))]
public class WaddlerBehavior : EnemyBehavior
{
    public float wakeDistance;

    public Animator anim;

    protected override void InitStateList() {
        stateList.AddRange(new List<State> {
            new State(State.Action.Idle, wakeDistance, 0f, 0f),

            new State(State.Action.Wait, 0f, 0f, 1f),
            new State(State.Action.ChaseTo, 2.5f, 3f, 0f),
            new State(State.Action.Wait, 0f, 0f, 0.05f),
            new State(State.Action.RetreatFor, 0f, 0.5f, 0.7f),
            new State(State.Action.Wait, 0f, 0f, 0.2f),
            new State(State.Action.Attack1, 0f, 6f, 1.5f)
        });
    }

    public GameObject slash;

    protected override void InitEnemy() {
       // Debug.Log(pf.GetNearestShelter(gameObject, player));
    }

    public float attackDelay;
    float attackTimer = 0;
    Vector2 target;
    bool hasAttacked = false;

    protected override void Attack1Implement(float dist, float vel, float dur, float delta) {
        if (!hasAttacked) {
            anim.SetTrigger("Attack");
            hasAttacked = true;
            target = pf.GetObjectDirection(gameObject, player);
            //slash.SetActive(true);
        }

        if (pf.GetObjectDistance(gameObject, player) > 0.8f) {
            rb2D.velocity = target.normalized * vel;
            //UpdateSpriteDirection();
        } else {
            rb2D.velocity = new Vector2(0, 0);
        }

        if(attackTimer < 0) {
            if(ec.Attack()) {
                attackTimer = attackDelay;
            }
        }

        attackTimer -= delta;
    }

    protected override void Attack1End() {
        slash.SetActive(false);
        hasAttacked = false;
    }

    protected override void ResetState() {
        slash.SetActive(false);
        attackTimer = 0;
        rb2D.velocity = new Vector2(0, 0);
    }
}
