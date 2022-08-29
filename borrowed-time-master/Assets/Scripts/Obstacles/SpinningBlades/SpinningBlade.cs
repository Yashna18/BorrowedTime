using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBlade : MonoBehaviour
{

    public int rotateSpeed;
    public float timeSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, rotateSpeed*Time.deltaTime*timeSpeed*PlayerManager.globalTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("hit " + other);

        if (other.tag == "TimeBomb") {
            timeSpeed *= other.GetComponent<Path>().timeSpeed;
        }

        if (other.tag == "Player") {
            PlayerManager.DamagePlayer(this.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        print("hit ended " + other);

        if (other.tag == "TimeBomb") {
            timeSpeed /= other.GetComponent<Path>().timeSpeed;
        }
    }
}
