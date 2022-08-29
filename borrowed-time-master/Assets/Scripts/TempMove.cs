using UnityEngine;
using System.Collections;

public class TempMove : MonoBehaviour
{

    // Rate of the 'bob' movement
    public float bobRate;

    // Scale of the 'bob' movement
    public float bobScale;

    // Update is called once per frame
    void Update()
    {
        // Change in vertical distance 
        float dx = 0f;

        if (Input.GetKey(KeyCode.Space))
        {
            //Change in vertical distance when spacebar is pressed
            dx = bobScale * Mathf.Sin(bobRate * Time.time);
        }


        // Move the game object on the vertical axis
        transform.Translate(new Vector3(-dx, 0, 0));
    }
}