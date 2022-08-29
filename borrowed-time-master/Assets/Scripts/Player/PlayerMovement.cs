using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int speed;
    public bool faceTowardsMouse;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.playerHealth > 0 & EndSceneTransition.cutscene == false) {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            this.transform.position += input.normalized * speed * Time.deltaTime * PlayerManager.globalTime;

            if (Input.GetAxisRaw("Horizontal") < -0.1) {
                // moving backwards, flip sprite
                anim.SetInteger("direction", 0);
                PlayerManager.isFacing = 0;
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
            } else if (Input.GetAxisRaw("Horizontal") > 0.1)  {
                anim.SetInteger("direction", 0);
                PlayerManager.isFacing = 0;
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            } else if (Input.GetAxisRaw("Vertical") < - 0.1)
            {
                anim.SetInteger("direction", 1);
                PlayerManager.isFacing = 1;
            } else if (Input.GetAxisRaw("Vertical") > 0.1)
            {
                anim.SetInteger("direction", 2);
                PlayerManager.isFacing = 2;
            }

                //this.transform.position += input * speed * Time.deltaTime * this.transform.forward;

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (faceTowardsMouse) {
                transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
            }
        }
    }
}
