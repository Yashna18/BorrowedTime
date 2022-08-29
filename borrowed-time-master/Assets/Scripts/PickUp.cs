/**
 * This script is to be added to any item that is set as the pickup item for
 * the open chest Game Object
 **/
using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        Color c = rend.material.color;
        c.a = 0f;
        rend.material.color = c;

        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && this != null)
        {
            PlayerManager.PlayerScore(100);
            
        }

        
        
    }
}