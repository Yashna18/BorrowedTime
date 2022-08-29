using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiateDialogue : MonoBehaviour
{

    public bool alreadyRead = false;

    public string[] dialogue;
    public string[] speaker;

    private bool active = false;

    private int dialogueIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active) {
            GameObject.Find("Speech").GetComponent<Text>().text = dialogue[dialogueIndex];
            GameObject.Find("Title").GetComponent<Text>().text = speaker[dialogueIndex];

            if (Input.GetKeyDown("e") & PlayerManager.inConversation == false) {
                print("here");
                GameObject.Find("Dialogue").GetComponent<CanvasGroup>().alpha = 1f;
                GameObject.Find("NPCDialogue").GetComponent<Image>().sprite = this.GetComponent<SpriteRenderer>().sprite;

                dialogueIndex = 0;
                PlayerManager.inConversation = true;
                active = true;

                PlayerManager.globalTime = 0;
            } else if (Input.GetKeyDown("e") & PlayerManager.inConversation == true)
            {
                dialogueIndex += 1;

                if (dialogueIndex > dialogue.Length-1) {
                    dialogueIndex = dialogue.Length-1;
                    alreadyRead = true;

                    endDialogue();
                }
            }

            if (speaker[dialogueIndex] == "Player") {
                GameObject.Find("NPCDialogue").GetComponent<CanvasGroup>().alpha = 0f;
                GameObject.Find("PlayerDialogue").GetComponent<CanvasGroup>().alpha = 1f;
            } else {
                GameObject.Find("NPCDialogue").GetComponent<CanvasGroup>().alpha = 1f;
                GameObject.Find("PlayerDialogue").GetComponent<CanvasGroup>().alpha = 0f;
            }

        }

    }

    private void endDialogue() {
        GameObject.Find("Dialogue").GetComponent<CanvasGroup>().alpha = 0f;

        PlayerManager.inConversation = false;
        active = false;

        PlayerManager.globalTime = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") {
            GameObject.Find("Dialogue").GetComponent<CanvasGroup>().alpha = 0f;

            PlayerManager.inConversation = false;
            active = false;

            PlayerManager.globalTime = 1;
        }
    }
}
