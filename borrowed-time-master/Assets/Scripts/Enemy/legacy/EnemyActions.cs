using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyActions : MonoBehaviour
{
	protected enum State { Idle, Chasing, Attacking, Hurt, Wait};

	protected GameObject player;
	protected State currentState;
	protected Rigidbody2D rb2D;

	public float enemyHealth = 10f;
	public float chaseDistance = 7f;
	public float attackDistance = 0.3f;

	public float chaseSpeed = 5f;
	public float attackSpeed = 10f;
	public float attackTime = 0.5f;
	public float waitTime = 0.5f;

	protected float attackTimer;
	protected float waitTimer;

	protected void InitEnemy() {
		if(GameObject.Find("Player") != null) {
			player = GameObject.Find("Player");
		}
		currentState = State.Idle;

		rb2D = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
	}

	// Start is called before the first frame update
	protected virtual void Start()
	{
		InitEnemy();
		Debug.Log("Angle: " + GetPlayerDirection());
		Debug.Log("Distance: " + GetPlayerDistance());
	}

	// Update is called once per frame
	protected void Update() {
		switch (currentState) {
			case State.Idle:
				if(GetPlayerDistance() < chaseDistance) {
					EnemyExitIdle();
					Debug.Log("From Idle to Chasing");
				} else {
					EnemyIdle(Time.deltaTime);
				}
				break;
			case State.Chasing:
				if(GetPlayerDistance() < attackDistance) {
					EnemyExitChasing();
					attackTimer = attackTime;
					Debug.Log("From Chasing to Attacking");
				} else {
					EnemyChasing(Time.deltaTime);
				}
				break;
			case State.Attacking:
				EnemyAttacking(Time.deltaTime);
				tickAttacking(Time.deltaTime);
				break;
			case State.Hurt:
				break;
			case State.Wait:
				EnemyWait(Time.deltaTime);
				break;
			default:
				break;
		}

		if (rb2D.velocity.x < 0) {
			gameObject.transform.localScale = new Vector3(-1, 1, 1);
			//gameObject.GetComponent<SpriteRenderer>().flipX = true;
		} else {
			gameObject.transform.localScale = new Vector3(1, 1, 1);
			//gameObject.GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	//Enemy is idle
	protected virtual void EnemyIdle(float delta) {
		return;
	}

	protected virtual void EnemyExitIdle() {
		currentState = State.Chasing;
    }

	//Enemy is chasing
	protected virtual void EnemyChasing(float delta) {
		// Actually, this should be called in FixedUpdate() instead of Update()
		// Might have to move it if it causes problems

		// rb2D.MovePosition(rb2D.position + GetPlayerDirection() * chaseSpeed * delta);
		rb2D.velocity = GetPlayerDirection() * chaseSpeed;
	}

	protected virtual void EnemyExitChasing() {
		currentState = State.Attacking;
		rb2D.velocity = new Vector2(0, 0);
	}

	//Enemy is attacking
	protected virtual void EnemyAttacking(float delta) {
		//rb2D.MovePosition(rb2D.position + GetPlayerDirection() * attackSpeed * delta);
		rb2D.velocity = GetPlayerDirection() * attackSpeed;
	}

	protected void tickAttacking(float delta) {
		attackTimer -= delta;
		if(attackTimer < 0) {
			EnemyExitAttacking();
		}
	}

	protected virtual void EnemyExitAttacking() {
		waitTimer = waitTime;
		currentState = State.Wait;
    }

	//Enemy is hurt
	public abstract void EnemyHurt(float delta);

	//Wait - A way to add a "break" to enemy action
	protected void EnemyWait(float delta) {
		waitTimer -= delta;
		if(waitTimer < 0) {
			EnemyExitWait();
        }
    }

	protected virtual void EnemyExitWait() {
		currentState = State.Chasing;
    }

	// For use when attack or other action is overridden
	protected void SetWait(float waitDuration) {
		currentState = State.Wait;
		waitTimer = waitDuration;
    }

	//Get direction to player
	protected Vector2 GetPlayerDirection() {
		Vector2 pos = this.transform.position;
		Vector2 target = player.transform.position;
		return (target - pos).normalized;
	}

	//Get distance to player
	protected float GetPlayerDistance() {
		return Vector2.Distance(this.transform.position, player.transform.position);
	}
}
