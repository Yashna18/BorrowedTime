using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;
    public float pauseFor;
    public float timeSpeed = 1;

    float affectedSpeed = 0.05f * 1;

    bool active = true;
    int direction = 1;
    float clock = 0;
    float reach = 0.6f;
    float pause = 0;


    void Start()
    {
        this.transform.localPosition = new Vector3(0, 0.6f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float fakeAcceleration = 1f + affectedSpeed;

        if (active & pause > pauseFor) {

            clock += Time.deltaTime;
            clock *= fakeAcceleration;

            if (clock > reach/speed) {
                clock = reach/speed;

                direction = -1;
                active = false;
                pause = 0;
                this.GetComponentInChildren<AudioSource>().Play();
            }

            this.transform.localPosition = new Vector3(0, reach - (float) (speed*clock * PlayerManager.globalTime), 0);

            if (pause == 0) {
                clock = 0;
            }
        } else if (!active & direction == -1 & pause > pauseFor) {
            clock += Time.deltaTime;
            clock *= fakeAcceleration;

            if (clock > reach/speed) {
                clock = reach/speed;

                direction = 1;
                active = true;
                pause = 0;
            }

            this.transform.localPosition = new Vector3(0, 0 + (float) (speed*clock * PlayerManager.globalTime), 0);

            if (pause == 0) {
                clock = 0;
            }
        } else {
            pause += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TimeBomb") {
            timeSpeed *= other.GetComponent<Path>().timeSpeed;
            affectedSpeed = 0.05f * timeSpeed;
        }

        if (other.tag == "Player") {
            PlayerManager.DamagePlayer(this.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "TimeBomb") {
            timeSpeed /= other.GetComponent<Path>().timeSpeed;
            affectedSpeed = 0.05f * timeSpeed;
        }
    }
}
