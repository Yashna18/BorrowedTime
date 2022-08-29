using UnityEngine;
using UnityEngine.Playables;

public class memoryTrigger : MonoBehaviour
{
    public PlayableDirector timeline;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            timeline.Play();
            Destroy(gameObject);

        }
    }
}
