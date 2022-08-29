using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumper : EnemyActions {
	public override void EnemyHurt(float delta) {
		Debug.Log("Hurt!!");
	}

	protected override void EnemyExitChasing() {
		currentState = State.Wait;
		SetWait(1f);
	}

	protected override void EnemyExitWait() {
		currentState = State.Attacking;
	}

    protected override void EnemyExitAttacking() {
		currentState = State.Chasing;
    }
}
