using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartVisual : MonoBehaviour
{
    // Start is called before the first frame update
    public int heartNumber;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      //  print(heartNumber);
        GameObject player = GameObject.Find("Player");

        if (PlayerManager.playerHealth >= heartNumber*2) {
            this.GetComponent<Image>().color = Color.white;
        } else if (PlayerManager.playerHealth >= heartNumber*2-1) {
            this.GetComponent<Image>().color = new Color(130/255f, 130/255f, 130/255f, 1f);
        } else {
            this.GetComponent<Image>().color = new Color(130/255f, 130/255f, 130/255f, 0f);
        }



    }
}
