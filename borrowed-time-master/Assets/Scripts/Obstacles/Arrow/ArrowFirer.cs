using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFirer : MonoBehaviour
{
    float timeSpeed = 1;
    float fireClock = 0;
    public float fireRate = 1;
    public float arrowSpeed = 10;

    public Transform arrowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fireClock += Time.deltaTime*timeSpeed*PlayerManager.globalTime;

        if (fireClock > fireRate) {
            fireClock = 0;

            // arrow script code stuff

            Transform shot = Instantiate(arrowPrefab);
			shot.position = transform.position;
            shot.GetComponent<Projectile>().lingerDuration = 0;
			shot.GetComponent<Projectile>().SetAngleSpeed(Vector3.up, arrowSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "TimeBomb") {
            timeSpeed *= other.GetComponent<Path>().timeSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "TimeBomb") {
            timeSpeed /= other.GetComponent<Path>().timeSpeed;
        }
    }
}
