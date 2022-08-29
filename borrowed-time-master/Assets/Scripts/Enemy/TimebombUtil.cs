using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimebombUtil : MonoBehaviour
{
    public float timeSpeed = 1;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "TimeBomb") {
            timeSpeed *= other.GetComponent<Path>().timeSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "TimeBomb") {
            timeSpeed /= other.GetComponent<Path>().timeSpeed;
        }
    }
}
