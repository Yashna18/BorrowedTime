using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.parent.GetComponent<InitiateDialogue>().alreadyRead == false) {
            this.transform.localPosition = new Vector3(0, 1 + Mathf.PingPong(Time.time, 0.5f)*0.25f, 0);
        } else {
            this.transform.GetComponent<SpriteRenderer>().enabled = false;
            this.transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
        }
    }
}
