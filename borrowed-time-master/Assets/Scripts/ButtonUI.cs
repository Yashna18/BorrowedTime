using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    public Transform pixelartprefab;
    private float tempX;
    private float gapBetween = 60f;

    public void NewGameButton()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void OnEnter(Transform buttonS)
    {
        Transform pixelart = Instantiate(pixelartprefab);
        pixelart.parent = buttonS;
        pixelart.localPosition = new Vector3(buttonS.position.x - gapBetween, 1, -1);


    }

    public void OnExit(Transform buttonS)
    {
        foreach (Transform child in buttonS.transform)
        {
            if (child.gameObject.tag != "text")
            {
                GameObject.Destroy(child.gameObject);

            }
        }
    }


}
