using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBehavior : EnemyBehavior
{
	public float wakeDistance;
	public Transform arrowPrefab;
	public float arrowSpeed;

	bool fired = false;

	protected override void InitEnemy() {
		fired = false;
	}

	protected override void InitStateList() {
		stateList.AddRange(new List<State> {
			new State(State.Action.Idle, wakeDistance, 0f, 0f),

			new State(State.Action.Wait, 0f, 0f, 1f),
			new State(State.Action.ChaseInView, 3f, 3f, 0f),
			new State(State.Action.ChaseFor, 0f, 3f, 0.5f),
			new State(State.Action.Wait, 0f, 0f, 0.3f),
			new State(State.Action.Attack1, 0f, 0f, 0.5f)
		});
	}


	protected override void Attack1Implement(float dist, float vel, float dur, float delta) {
		if(!fired) {
			Transform shot = Instantiate(arrowPrefab);
			// Vector2 dir = pf.GetObjectDirection(gameObject, player);
			// float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
			// shot.rotation = Quaternion.Euler(Vector3.forward * angle);
			shot.position = transform.position;
			// shot.GetComponent<Projectile>().SetAngle(pf.GetObjectDirection(gameObject, player));
			// shot.GetComponent<Projectile>().speed = arrowSpeed;
			shot.GetComponent<Projectile>().SetAngleSpeed(pf.GetObjectDirection(gameObject, player), arrowSpeed);
			fired = true;
		}
	}

	protected override void Attack1End() {
		fired = false;
	}

}
