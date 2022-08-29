using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneTransition : MonoBehaviour
{
    private bool toggle = false;
    private float clock = 0;

    public static bool cutscene = false;

    private Vector3 endPos;
    private Vector3 targetPos = new Vector3(-2f, 27.5f, 34);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject boss = GameObject.Find("Behavior_Boss");

        if (boss != null && boss.GetComponent<EnemyCombat>().Health <= 0) {
            toggle = true;
            cutscene = true;
            endPos = this.transform.localPosition;
        }

        if (toggle == true) {
            //toggle = false;

            CameraFollowPlayer.followPlayer = false;

            if (clock > 200 & clock < 290) {
                this.GetComponent<Camera>().orthographicSize += -0.1f/3;
            }
            clock += 1;

            if (clock > 400 & clock <= 800) {
                this.transform.localPosition = Vector3.Lerp(endPos, targetPos, (clock-400)/400);
                print(this.transform.localPosition);
            }

            if (clock > 900) {
                GameObject door = GameObject.Find("Door");

                door.GetComponent<SpriteRenderer>().color = Color.white;
                door.GetComponent<BoxCollider2D>().isTrigger = true;
            }

            if (clock > 1200) {
                cutscene = false;
                CameraFollowPlayer.followPlayer = true;
                this.GetComponent<Camera>().orthographicSize = 5;
            }
        }
    }
}
