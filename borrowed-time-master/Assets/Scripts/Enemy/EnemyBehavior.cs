using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPathfinding))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyCombat))]
[RequireComponent(typeof(TimebombUtil))]
public abstract class EnemyBehavior : MonoBehaviour
{
	public GameObject player;
	public bool globalTime = true;
	public bool localTime = true;
	public bool pathFindingEnemy = true;
	public bool interruptable = true;
	public float deathLingerTime = 3f;

	protected EnemyPathfinding pf;
	protected TimebombUtil tb;
	protected EnemyCombat ec;
	protected SpriteRenderer sr;

	protected State currentState;
	protected int currentStateIndex;
	protected Rigidbody2D rb2D;
	protected List<State> stateList;

	protected float stateTimer;

	Vector3 startingPosition;

	// Start is called before the first frame update
	private void Start()
	{
		stateList = new List<State>();
		InitStateList();

		if(stateList.Count > 0) {
			currentState = stateList[0];
		} else {
			currentState = new State(State.Action.Idle, 0, 0, 0);
		}

		rb2D = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		pf = gameObject.GetComponent(typeof(EnemyPathfinding)) as EnemyPathfinding;
		tb = gameObject.GetComponent(typeof(TimebombUtil)) as TimebombUtil;
		ec = gameObject.GetComponent(typeof(EnemyCombat)) as EnemyCombat;
		sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

		if(player == null) {
			player = GameObject.Find("Player");
		}

		startingPosition = gameObject.transform.position;

		InitEnemy();
	}

	protected abstract void InitStateList();
	protected virtual void InitEnemy() { }

	// Update is called once per frame
	private void Update()
	{
		// Debug.Log(currentState.mode);
		// Debug.Log(tb.timeSpeed);

		float adjustedDelta = Time.deltaTime;
		float adjustedSpeed = currentState.velocity;
		
		if(globalTime) {
			adjustedDelta *= PlayerManager.globalTime;
			adjustedSpeed *= PlayerManager.globalTime;
		}

		if(localTime) {
			adjustedDelta *= tb.timeSpeed;
			adjustedSpeed *= tb.timeSpeed;
        }

		if(hurtTimer < 0) {
			switch (currentState.mode) {
				case State.Action.Idle:
					Idle(currentState.distance);
					break;
				case State.Action.Attack1:
					Attack1(currentState.distance, adjustedSpeed, currentState.duration, adjustedDelta);
					break;
				case State.Action.Attack2:
					Attack2(currentState.distance, adjustedSpeed, currentState.duration, adjustedDelta);
					break;
				case State.Action.Attack3:
					Attack3(currentState.distance, adjustedSpeed, currentState.duration, adjustedDelta);
					break;
				case State.Action.ChaseInView:
					ChaseInView(adjustedSpeed, adjustedDelta);
					break;
				case State.Action.ChaseTo:
					ChaseTo(adjustedSpeed, currentState.distance, adjustedDelta);
					break;
				case State.Action.ChaseFor:
					ChaseFor(adjustedSpeed, adjustedDelta);
					break;
				case State.Action.Hide:
					Hide(adjustedSpeed, adjustedDelta);
					break;
				case State.Action.RetreatFor:
					RetreatFor(adjustedSpeed, adjustedDelta);
					break;
				case State.Action.RetreatTo:
					RetreatTo(adjustedSpeed, currentState.distance, adjustedDelta);
					break;
				case State.Action.Wait:
					Wait();
					break;
			}
			stateTimer -= adjustedDelta;

			UpdateImplement(adjustedDelta);
		} else {
			hurtTimer -= adjustedDelta;
			//rb2D.velocity = new Vector2(0, 0);
			// TODO: Hurt animation here
        }

	}

	protected virtual void UpdateImplement(float delta) { }

	protected void IncrementState() {
		do {
			currentStateIndex++;
			currentStateIndex = currentStateIndex % stateList.Count;

			currentState = stateList[currentStateIndex];
			stateTimer = currentState.duration;
		} while (currentState.mode == State.Action.Idle);

		ResetState();
	}

	void Attack1(float dist, float vel, float dur, float delta) {
		Attack1Implement(dist, vel, dur, delta);

		stateTimer -= delta;

		if(stateTimer < 0) {
			Attack1End();
			IncrementState();
		}
	}

	void Attack2(float dist, float vel, float dur, float delta) {
		Attack2Implement(dist, vel, dur, delta);

		stateTimer -= delta;

		if (stateTimer < 0) {
			Attack2End();
			IncrementState();
		}
	}

	void Attack3(float dist, float vel, float dur, float delta) {
		Attack3Implement(dist, vel, dur, delta);

		stateTimer -= delta;

		if (stateTimer < 0) {
			Attack3End();
			IncrementState();
		}
	}

	protected virtual void Attack1Implement(float dist, float vel, float dur, float delta) { }
	protected virtual void Attack1End() { }
	protected virtual void Attack2Implement(float dist, float vel, float dur, float delta) { }
	protected virtual void Attack2End() { }

	protected virtual void Attack3Implement(float dist, float vel, float dur, float delta) { }
	protected virtual void Attack3End() { }

	void Idle(float distance) {
		if(pf.IsPathUnoccluded(gameObject, player) && 
			pf.GetObjectDistance(gameObject, player) < distance) {
			IdleWake();
			IncrementState();
		}
	}

	protected virtual void IdleWake() { }

	void Wait() {
		rb2D.velocity = new Vector2(0, 0);

		if(stateTimer < 0) {
			IncrementState();
		}
	}

	void RetreatFor(float velocity, float delta) {
		Retreat(velocity, delta);

		if(stateTimer < 0) {
			IncrementState();
		}
	}

	void RetreatTo(float velocity, float distance, float delta) {
		Retreat(velocity, delta);
		if(pf.GetObjectDistance(gameObject, player) > distance) {
			IncrementState();
		}
	}

	void ChaseFor(float velocity, float delta) {
		Chase(velocity, delta);

		if (stateTimer < 0) {
			IncrementState();
		}
	}

	void ChaseTo(float velocity, float distance, float delta) {
		Chase(velocity, delta);

		if(pf.IsPathUnoccluded(gameObject, player) && 
			pf.GetObjectDistance(gameObject, player) < distance) {
			IncrementState();
		}
	}

	void ChaseInView(float velocity, float delta) {
		Chase(velocity, delta);

		if (pf.IsPathUnoccluded(gameObject, player)) {
			IncrementState();
		}
	}

	float chaseUpdateTime = 0.5f;
	float chaseTimer = 0;
	Vector2 targetVector;

	void Chase(float velocity, float delta) {
		// Debug.Log(velocity + ", " + delta);

		if ((chaseTimer <= 0 && pathFindingEnemy) || velocity != currentState.velocity) {
			Vector2 targetCoord = pf.GetTargetCoord(player);

			targetVector = pf.GetObjectDirection(gameObject.transform.position, targetCoord) * velocity;
			chaseTimer = Random.value * chaseUpdateTime;
		} 
		
		if(!pathFindingEnemy) {
			targetVector = pf.GetObjectDirection(gameObject, player) * velocity;
		}

		targetVector = ChaseImplement(targetVector);
		// Debug.Log(targetVector);

		rb2D.velocity = targetVector;
		UpdateSpriteDirection();
		chaseTimer -= delta;
	}

	protected virtual Vector2 ChaseImplement(Vector2 val) { return val; }

	void Hide(float velocity, float delta) {
		if(chaseTimer < 0 || velocity != currentState.velocity) {
			targetVector = pf.GetObjectDirection(gameObject.transform.position,
				pf.GetTargetCoord(
					pf.GetNearestShelter(gameObject, player))) * velocity;
			chaseTimer = Random.value * chaseUpdateTime;
		}

		rb2D.velocity = targetVector;
		UpdateSpriteDirection();
		chaseTimer -= delta;

		if(!pf.IsPathUnoccluded(gameObject, player)) {
			IncrementState();
		}
	}

	void Retreat(float velocity, float delta) {
		if(chaseTimer < 0 || velocity != currentState.velocity) {
			targetVector = pf.GetObjectDirection(gameObject.transform.position, pf.GetTargetCoord(player)) * velocity * -1;
			chaseTimer = Random.value * chaseUpdateTime;
		}

		rb2D.velocity = targetVector;
		UpdateSpriteDirection(true);
		chaseTimer -= delta;
	}

	protected void UpdateSpriteDirection(bool reverse = false) {
		if (rb2D.velocity.x < 0) {
			if (reverse) {
				gameObject.transform.localScale = new Vector3(1, 1, 1);
			} else {
				gameObject.transform.localScale = new Vector3(-1, 1, 1);
			}
		} else {
			if (reverse) {
				gameObject.transform.localScale = new Vector3(-1, 1, 1);
			} else {
				gameObject.transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Vector2 colPoint = collision.GetContact(0).point;
		Vector2 bounceVector = pf.GetObjectDirection(colPoint, gameObject.transform.position);
		rb2D.AddForce(bounceVector * 1000f);
	}

	float hurtTimer;

	public void HurtEnemy(float duration) {
		HurtImplement();

        if (interruptable) {
			hurtTimer = duration;
			currentStateIndex = 0;
			IncrementState();
			ResetState();
		}
		//TODO: Hurt animation here
    }

	protected virtual void HurtImplement() { }

	/// <summary>
	/// Must call Destroy(gameObject) eventually
	/// </summary>
	public void Kill() {
		KillImplement();

		// Make non-physical
		rb2D.velocity = new Vector2(0, 0);
		transform.Rotate(new Vector3(0, 0, -90));
		rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
		rb2D.isKinematic = true;
		gameObject.GetComponent<Collider2D>().isTrigger = true;

		// Stop all activity
		stateList = new List<State>();
		stateList.Add(new State(State.Action.Wait, 0f, 0f, 0f));
		IncrementState();

		// Fade sprite
		sr.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		// Destroy after delay
		Destroy(gameObject, deathLingerTime); 
	}

	protected virtual void KillImplement() { }

	protected virtual void ResetState() { }

	public void ResetEnemy() {
		ResetState();
		// gameObject.transform.position = startingPosition;
		currentStateIndex = 0;
		currentState = stateList[currentStateIndex];
    }

	protected class State {
		public enum Action { Idle, Dead, ChaseInView, ChaseTo, 
			ChaseFor, Wait, RetreatTo, RetreatFor, Hide, Attack1, Attack2, Attack3 };

		/// <summary>
		/// Public constructor for state.
		/// </summary>
		/// <param name="m">Mode of state. Choose from enum, State.Action</param>
		/// <param name="dist">Distance, relevant for: ChaseTo</param>
		/// <param name="sp">Speed, relevant for: ChaseInView, ChaseTo, ChaseFor, Retreat</param>
		/// <param name="dur">Duration, relevant for: ChaseFor, Wait, Retreat, Attack</param>
		public State(Action m, float dist, float v, float dur) {
			mode = m;
			distance = dist;
			velocity = v;
			duration = dur;
		}

		public Action mode { get; set; }
		public float distance { get; set; }
		public float velocity { get; set; }
		public float duration { get; set; }
	}
}
