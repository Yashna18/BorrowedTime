using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TimeTaken : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Text>().text = "TIME TAKEN: " + PlayerManager.totalTime;
        PlayerManager.playing = false;
        PlayerManager.totalTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
             SceneManager.LoadScene("MainMenuNew");
        }
    }
}
