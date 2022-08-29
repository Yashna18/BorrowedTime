using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    //public Vector3[] pathArray;
    Vector3[] pathArray;

    int pathIndex = 1;
    int lerpIndex = 0;
    float delay = 0;
    bool allowedToMove = true;

    int lerpDivisions = 5; // how many frames per second for moving time bomb
    int rotateSpeed = 50;

    float timeLanded = 0;

    public int throwTime = 1;
    public float timeSpeed = 0.3f;
    private float bombDuration;
    public Animator anim;

    private AudioSource audioSource;
    public AudioClip timeExplosionSound;


    // Start is called before the first frame update
    void Start()
    {
        LineRenderer lineRenderer = GameObject.Find("LaunchArc").GetComponent<LineRenderer>();

        audioSource = this.GetComponent<AudioSource>();

        pathArray = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(pathArray);

        if ((pathArray[0].x - pathArray[1].x) > 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        } else if ((pathArray[0].x - pathArray[1].x) < 0)
            this.transform.rotation = Quaternion.Euler(0, 0, 0);

        bombDuration = (0.7f - this.timeSpeed) * 5;
        print(bombDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (allowedToMove == true) {
            lerpDivisions = (int) (200 / pathArray.Length);
          
            this.transform.position = Vector3.Lerp(pathArray[pathIndex-1], pathArray[pathIndex], (float)lerpIndex/(float)lerpDivisions);
            this.transform.Rotate(0, 0, -rotateSpeed*Time.deltaTime, Space.Self);
            
           // Quaternion rot = Quaternion.LookRotation(pathArray[pathIndex+1]);
            //print(rot.z);  
            //this.transform.rotation = Quaternion.Euler(0, rot.y*Mathf.Rad2Deg, rot.z*Mathf.Rad2Deg);

            float distance = Vector3.Distance(pathArray[0], pathArray[pathArray.Length-1]);

            delay += Time.deltaTime;
            // print(throwTime/lerpDivisions);
            if (delay > distance/10000) {
                delay = 0;
                lerpIndex++;
            }

            if (lerpIndex > lerpDivisions) {
                lerpIndex = 0;
                pathIndex++;

                if (pathIndex >= pathArray.Length) {
                    allowedToMove = false;
                    timeLanded = 0;
                    anim.SetTrigger("landed");

                    audioSource.volume = 1f;
                    audioSource.PlayOneShot(timeExplosionSound);
                    //this.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }

        } else {
            timeLanded += Time.deltaTime;

            this.GetComponent<CircleCollider2D>().enabled = true;

            if (timeLanded > bombDuration) {
                print("Destroyed");
                Destroy(this.gameObject);
            }
        }

    }
}
