using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMove : MonoBehaviour
{
    float pos = 0;
    float flip = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = pos + 0.5f*flip*Time.deltaTime;

        this.transform.localPosition = new Vector3(-11.09f + pos, -1.03f, -5);

        if (pos > 17.25 | pos < 0) {
            flip *= -1;
        }
    }
}
