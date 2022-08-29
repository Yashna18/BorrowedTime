using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestCount : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Text>().text = "CHESTS: " + PlayerManager.playerScore;
        PlayerManager.playerScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
