using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            if (PlayerManager.checkpoint < int.Parse(this.name)) {
                PlayerManager.checkpoint = int.Parse(this.name);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") {
            if (PlayerManager.checkpoint < int.Parse(this.name)) {
                PlayerManager.checkpoint = int.Parse(this.name);
            }
        }
    }
}
