using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGhostBehavior : EnemyBehavior
{
	public Transform particle;

	protected override void InitStateList() {
		stateList.AddRange(new List<State> {
			new State(State.Action.ChaseTo, 2.5f, 3f, 0f),
			// new State(State.Action.Wait, 0f, 0f, 0.05f),
			new State(State.Action.Attack1, 0f, 15f, 1f),
			new State(State.Action.Wait, 0f, 0f, 0.8f)
		});
	}

	// SpriteRenderer sr;

	protected override void InitEnemy() {
		pathFindingEnemy = false;
		sr.enabled = false;
		// sr = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
	}

	protected override void UpdateImplement(float delta) {
		//if(!PlayerManager.isThrowingTimeBomb ) {
		//	sr.enabled = false;
		//} else {
		//	sr.enabled = true;
		//}

		if(PlayerManager.isThrowingTimeBomb != sr.enabled) {
			sr.enabled = PlayerManager.isThrowingTimeBomb;
			Transform ps = Instantiate(particle);
			ps.position = transform.position;
        }
	}

	protected override Vector2 ChaseImplement(Vector2 val) {
		// Debug.Log(PlayerManager.isThrowingTimeBomb);

		if (!PlayerManager.isThrowingTimeBomb) {
			return new Vector2(0, 0);
		}

		return val;
	}

	bool attackStart = false;
	bool attacked = false;
	Vector2 target;

	protected override void Attack1Implement(float dist, float vel, float dur, float delta) {
		if(PlayerManager.isThrowingTimeBomb) {
			if(!attackStart) {
				target = pf.GetObjectDirection(gameObject, player).normalized * vel;
				attackStart = true;
			}

			rb2D.velocity = target;

			if(!attacked && playerHit) {
				attacked = true;
				PlayerManager.DamagePlayer();
			}
		}
	}

	protected override void Attack1End() {
		attacked = false;
		attackStart = false;
	}

	bool playerHit;

	private void OnTriggerEnter2D(Collider2D collision) {
		if(collision.gameObject.name == "Player") {
			playerHit = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if(collision.gameObject.name == "Player") {
			playerHit = false;
		}
	}
}
