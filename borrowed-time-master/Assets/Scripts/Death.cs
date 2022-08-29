using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
        if (PlayerManager.playerHealth <= 0) {
            this.GetComponent<CanvasGroup>().alpha = 1f;
        } else {
            this.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }
}
