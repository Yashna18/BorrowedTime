using UnityEngine;

public class OpenChest : MonoBehaviour
{
    public GameObject item;
    GameObject pickUp;

    private AudioSource audioSource;

    public AudioClip chestOpenSound;

    // Start is called before the first frame update
    void Start()
    {
         audioSource = this.GetComponent<AudioSource>();
        audioSource.PlayOneShot(chestOpenSound);
        //pickUp = Instantiate(item, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);

        PlayerManager.PlayerScore(1);
    }


    // Update is called once per frame
    void Update()
    {
        if (pickUp != null)
        {
            float dy = 1f + (0.09f * Mathf.Sin(5f * Time.time));
            pickUp.transform.position = new Vector3(transform.position.x, transform.position.y + dy, transform.position.z);

            if (Input.GetButtonDown("Interact"))
            {
                Destroy(pickUp);
                
            }
        }
    }   
}