using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimebombUtil))]
public class Projectile : MonoBehaviour
{
	public float speed;
	public float lingerDuration;
	public Vector2 angle;

	public bool ignoreEnemies = true;

    private AudioSource audioSource;
    
    public AudioClip fireSound;

	Rigidbody2D rb2D;
	TimebombUtil tb;
	bool alive;

	public void SetAngleSpeed(Vector2 angle, float speed) {
		SetAngle(angle);
		this.speed = speed;
	}

	void SetAngle(Vector2 a) {
		angle = a;

		Vector2 dir = angle;
		float angleDeg = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
		gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * angleDeg);
	}

	// Start is called before the first frame update
	void Start()
	{
		rb2D = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		tb = gameObject.GetComponent(typeof(TimebombUtil)) as TimebombUtil;
		alive = true;    

        audioSource = this.GetComponent<AudioSource>();

        audioSource.PlayOneShot(fireSound);
	}

	// Update is called once per frame
	void Update()
	{
		if (alive) {
			rb2D.velocity = angle.normalized * speed * tb.timeSpeed * PlayerManager.globalTime;
		} else {
			lingerDuration -= Time.deltaTime;

			if(lingerDuration < 0) {
				Destroy(gameObject);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Projectile" || 
			(collision.gameObject.tag == "Enemy" && ignoreEnemies)) {
			// Ignore collisions with this object if object is enemy and ignore enemies is true;
			Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(),
				GetComponent<Collider2D>());
		} else {

			if (collision.gameObject.name == "Player") {
				if (tb.timeSpeed * PlayerManager.globalTime >= 0.7f) {
					// Debug.Log("I hit a player!");
					PlayerManager.DamagePlayer();
					AffixProjectile(collision.gameObject.transform);
				}
			} else {
				if (collision.gameObject.tag == "Enemy" && !ignoreEnemies) {
					// Debug.Log("I hit an enemy!");
					collision.gameObject.GetComponent<EnemyCombat>().TakeDamage(1);
					collision.gameObject.GetComponent<EnemyCombat>().Knockback(this.gameObject);
					AffixProjectile(collision.gameObject.transform);
				} else {
					AffixProjectile(collision.gameObject.transform);
				}
			}
		}
		
	}

	private void AffixProjectile(Transform parent) {
		rb2D.velocity = new Vector2(0, 0);
		gameObject.transform.parent = parent;
		alive = false;
		gameObject.GetComponent<Collider2D>().isTrigger = true;
		rb2D.isKinematic = true;
	}
}
