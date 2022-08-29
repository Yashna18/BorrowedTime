using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissappearTile : MonoBehaviour
{
    public float offset;
    public float pauseTime;
    public float speed;

    float clock = 0;
    public float transitionClock = 0;
    public float timeSpeed = 1;
    float direction = 1f;
    float pauseClock = 0;

    bool playerStandingOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool canDamage() {
        return ((pauseClock <= pauseTime & pauseClock > 0) & direction == -1);
    }


    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        pauseClock += Time.deltaTime*timeSpeed * PlayerManager.globalTime;

        if (clock > offset & pauseClock > pauseTime) {
            transitionClock = transitionClock + Time.deltaTime*direction*timeSpeed * PlayerManager.globalTime;

            Color lerpedColor = Color.Lerp(Color.white, Color.gray, transitionClock*speed);
            this.GetComponent<SpriteRenderer>().color = lerpedColor;

            if (transitionClock*speed >= 1 | transitionClock*speed <= 0) {
                transitionClock = Mathf.Clamp(transitionClock, 0, 1);
                pauseClock = 0;
                direction *= -1;
                if (transitionClock > 0.5 && !canDamage())
                {
                    this.GetComponentInChildren<AudioSource>().Play();
                }
            }
        }

        if (canDamage()) {
                this.GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (playerStandingOn == true & canDamage()) {
            print("Damage");
            playerStandingOn = false;

            PlayerManager.DamagePlayer(this.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TimeBomb") {
            timeSpeed *= other.GetComponent<Path>().timeSpeed;
        }

        if (other.tag == "Player") {
            playerStandingOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "TimeBomb") {
            timeSpeed /= other.GetComponent<Path>().timeSpeed;
        }

        if (other.tag == "Player") {
            playerStandingOn = false;
        }
    }
}
