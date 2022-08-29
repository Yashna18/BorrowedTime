using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public static bool followPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer) {
          this.transform.position = GameObject.Find("Player").transform.position + new Vector3(0, 0, -10);
        }
    }
}
