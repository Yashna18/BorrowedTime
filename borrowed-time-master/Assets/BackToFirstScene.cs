using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToFirstScene : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("Score");
    }
}
