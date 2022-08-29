using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossCrystalBehavior : EnemyBehavior
{
	public float wakeDistance;

	public Transform UICanvas;

	public Transform BossHealthBarPrefab;
	public Vector2 HealthBarPosition = new Vector2(0, -185f);

	public float travelSpeed = 4f;
	public float waitTime = 2.5f;

	public Sprite defaultSprite;
	public Sprite deadSprite;

	Transform BossUI;
	Slider BossUISlider;
	string BossHealthBarName = "BossHealthBar";

	EnemyCombat enemyCombat;
	float maxHealth;

	Animator animator;

	GameObject enemyContainer;

	protected override void InitStateList() {
		stateList.AddRange(new List<State> {
			new State(State.Action.Idle, wakeDistance, 0f, 0f),

            new State(State.Action.Wait, 0f, 0f, waitTime),

            new State(State.Action.ChaseTo, 6f, travelSpeed, 0f),
            new State(State.Action.Wait, 0f, 0f, 0.05f),
            new State(State.Action.RetreatFor, 0f, travelSpeed*2, 0.7f),
            new State(State.Action.Attack1, 0f, attack1ChargeSpeed, attack1Length),
            new State(State.Action.Wait, 0f, 0f, waitTime),

            new State(State.Action.ChaseInView, 7f, travelSpeed, 0f),
			new State(State.Action.ChaseFor, 5f, travelSpeed, 0.8f),
			new State(State.Action.Wait, 0f, 0f, 0.5f),
			new State(State.Action.Attack2, 0f, 0f, attack2Length),
			new State(State.Action.Wait, 0f, 0f, waitTime),

            new State(State.Action.Attack3, 0f, 5f, attack3Length)
        });
	}

	protected override void InitEnemy() {
		enemyCombat = gameObject.GetComponent<EnemyCombat>();
		animator = gameObject.GetComponent<Animator>();
		maxHealth = enemyCombat.Health;
		enemyContainer = new GameObject("EnemyContainer");

		if(UICanvas == null) {
			UICanvas = GameObject.Find("Canvas").transform;
		}

		sr.color = BossCol.idleCol;
		sr.sprite = defaultSprite;
	}

    protected override void UpdateImplement(float delta) {
		float timeEffect = tb.timeSpeed * PlayerManager.globalTime;

		animator.SetFloat("timeFactor", timeEffect);

    }

    protected override void HurtImplement() {
		if (BossUISlider != null) {
			BossUISlider.value = enemyCombat.Health / maxHealth;
		}
	}

    protected override void KillImplement() {
		if (BossUISlider != null) {
			DestroyUI();
		}
		animator.SetTrigger("Death");
		sr.color = BossCol.deadCol;

		foreach(Transform child in enemyContainer.transform) {
			child.gameObject.GetComponent<EnemyBehavior>().Kill();
        }
		Destroy(enemyContainer.gameObject, deathLingerTime * 10f);
	}

    protected override void IdleWake() {
		DisplayUI();
		sr.color = BossCol.defaultCol;
	}

	Vector2 target;
	bool attack1Start = false;
	bool attack1Hit = false;
	float attack1Timer;
	public float attack1Length;
	public float attack1Offset;
	public float attack1ChargeSpeed;

	protected override void Attack1Implement(float dist, float vel, float dur, float delta) {
		// Charge attack

		animator.SetTrigger("Attack1");
		sr.color = BossCol.attack1Col;

		if((attack1Timer*2)/dur > attack1Offset) {
			Debug.Log(attack1Timer + ", " + dur);

			if (!attack1Start) {
				attack1Start = true;
				target = pf.GetObjectDirection(gameObject, player);
			}

			if (!attack1Hit && playerHit) {
				// Hit!
				rb2D.velocity = Vector2.zero;
				attack1Hit = true;
				PlayerManager.DamagePlayer();
			} else {
				// Keep rushing!
				rb2D.velocity = target.normalized * vel;
			}
		}

		attack1Timer += delta;
	}
	protected override void Attack1End() {
		attack1Start = false;
		attack1Hit = false;
		attack1Timer = 0;
		animator.SetTrigger("Default");
		sr.color = BossCol.defaultCol;
	}

	float attack2Timer = 0;
	int attack2Counter = 0;
	float attack2TargetAngle;
	float attack2Direction = 1;

	public float attack2Length = 3f;
	public float attack2Offset = 0.4f;
	public int attack2Rounds = 3;
	public int attack2ArrowCount = 3;
	public float angleIncrement = 25;
	public float angleInterval = 37;
	public float projectileSpeed = 10f;
	public Transform projectilePrefab;
	

	protected override void Attack2Implement(float dist, float vel, float dur, float delta) {
		// Projectile attack
		animator.SetTrigger("Attack2");
		sr.color = BossCol.attack2Col;

		float offset = dur * attack2Offset;

		if (attack2Timer*2 - offset > ((dur-offset)/(attack2Rounds + 1))*(attack2Counter + 1)) {

			if(attack2Counter == 0) {
				attack2TargetAngle = Vec2ToDeg(pf.GetObjectDirection(gameObject, player));
			} else {
				attack2TargetAngle += angleIncrement*attack2Direction;
            }

			ShootProjectile(attack2TargetAngle);

			for(int i = 1; i < attack2ArrowCount; i++) {
				ShootProjectile(attack2TargetAngle + (i - 1) * angleInterval);
				ShootProjectile(attack2TargetAngle - (i - 1) * angleInterval);
            }
			attack2Counter++;
		}

		attack2Timer += delta;
	}

	protected override void Attack2End() {
		attack2Counter = 0;
		attack2Timer = 0;
		animator.SetTrigger("Default");
		sr.color = BossCol.defaultCol;
		attack2Direction *= -1;
	}

	float attack3Timer = 0;
	int attack3Count = 0;
	bool attack3Started;
	Vector2 attack3Target;

	public float attack3Length = 3f;
	public float attack3Offset = 0.3f;
	public Transform minionPrefab;
	public int minionCount = 3;
	public int maxMinions = 10;

	protected override void Attack3Implement(float dist, float vel, float dur, float delta) {
		// Summon minions
		animator.SetTrigger("Attack3");
		sr.color = BossCol.attack3Col;

		if(!attack3Started) {
			attack3Target = transform.position;
			attack3Started = true;
        }

		float offset = dur * attack3Offset;

		if(attack3Timer*2 - offset > ((dur - offset)/(minionCount+1))*(attack3Count + 1) && attack3Count < minionCount) {
			if(enemyContainer.transform.childCount < maxMinions) {
				SpawnMinion();
            }

			attack3Count++;
		}

		rb2D.velocity = pf.GetObjectDirection(gameObject.transform.position, attack3Target);

		attack3Timer += delta;
	}

	protected override void Attack3End() {
		attack3Timer = 0;
		attack3Count = 0;
		attack3Started = false;
		animator.SetTrigger("Default");
		sr.color = BossCol.defaultCol;
	}

	void DisplayUI() {
		BossUI = Instantiate(BossHealthBarPrefab);
		BossUI.SetParent(UICanvas, false);
		BossUISlider = BossUI.Find(BossHealthBarName).GetComponent<Slider>();
	}

	void DestroyUI() {
		foreach (Transform child in BossUI) {
			Destroy(child.gameObject);
		}
		// Destroy(BossUI.gameObject);
	}

	bool playerHit = false;

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision.gameObject.name == "Player") {
			playerHit = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if(collision.gameObject.name == "Player") {
			playerHit = false;
		}
	}

	void ShootProjectile(float a) {
		Transform p = Instantiate(projectilePrefab);
		p.position = transform.position;
		p.GetComponent<Projectile>().SetAngleSpeed(DegToVec2(a), projectileSpeed);
	}

	Vector2 DegToVec2(float a) {
		float x = Mathf.Cos(a * Mathf.Deg2Rad);
		float y = Mathf.Sin(a * Mathf.Deg2Rad);
		return new Vector2(x, y).normalized;
	}

	float Vec2ToDeg(Vector2 a) {
		return Mathf.Atan2(a.y, a.x) * Mathf.Rad2Deg;
	}

	void SpawnMinion() {
		Transform m = Instantiate(minionPrefab, enemyContainer.transform);
		m.position = transform.position;
	}

	static class BossCol {
		public static Color defaultCol = new Color32(0xad, 0x75, 0xc7, 0xff);
		public static Color attack1Col = new Color32(0xad, 0x6f, 0x6f, 0xff);
		public static Color attack2Col = new Color32(0x1e, 0xb2, 0xe3, 0xff);
		public static Color attack3Col = new Color32(0xa8, 0xda, 0xe3, 0xff);

		public static Color idleCol = new Color32(0x59, 0x67, 0x6b, 0xff);
		public static Color deadCol = new Color32(0x95, 0x9d, 0xa3, 0xff);
    }
}
