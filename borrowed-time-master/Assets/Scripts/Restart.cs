using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
	void Start () {
		Button btn = this.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameObject player = GameObject.Find("Player");
        GameObject checkpoints = GameObject.Find("Checkpoints");
        GameObject checkpoint = GameObject.Find(PlayerManager.checkpoint.ToString());

        player.transform.position = checkpoint.transform.position;

        PlayerManager.playerHealth = 6;
        PlayerManager.globalTime = 1;
        PlayerManager.mana = 100;

        PlayerManager.NormalRealm();

        foreach(Transform enemy in GameObject.Find("Enemies").transform) {
            if(enemy.GetComponent<EnemyBehavior>() != null) {
                enemy.GetComponent<EnemyBehavior>().ResetEnemy();
            }
        }
	}

        void Update () {
        if (PlayerManager.playerHealth <= 0) {
            this.GetComponent<Button>().interactable = true;
        } else {
            this.GetComponent<Button>().interactable = false;
        }
    }
}
