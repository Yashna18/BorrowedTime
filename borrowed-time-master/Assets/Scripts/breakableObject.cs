using UnityEngine;

public class breakableObject : MonoBehaviour
{

    public GameObject secondItem;
    private bool triggered = false;

    // Update is called once per frame
    void Update()
    {
        if (triggered == true && Input.GetMouseButtonDown(0) && this.CompareTag("vase"))
        {
            BreakIt();
        }

        else if (triggered == true && Input.GetButtonDown("Interact") && this.CompareTag("chest"))
        {
            
            OpenIt();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            triggered = false;
        }
    }

    public void BreakIt()
    {
        Destroy(gameObject);
        GameObject broke = (GameObject)Instantiate(secondItem, transform.position, Quaternion.identity);
        foreach(Transform child in broke.transform)
        {
            child.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        }
    }

    public void OpenIt()
    {
        Destroy(gameObject);
        Instantiate(secondItem, transform.position, Quaternion.identity);
    }
}
