using UnityEngine.Playables;
using UnityEngine;

public class DevilPick : MonoBehaviour
{
    public PlayableDirector timeline;
    private bool trigger = false;

    void Update()
    {
        if (trigger == true && Input.GetButtonDown("Interact"))
        {
            timeline.Play();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            trigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            trigger = false;
        }
    }
}

