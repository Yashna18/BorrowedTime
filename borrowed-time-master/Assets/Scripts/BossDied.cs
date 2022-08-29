using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDied : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<EnemyCombat>().Health <= 0) {
            //SceneManager.LoadScene("FinalCutScene");
        }
    }
}
