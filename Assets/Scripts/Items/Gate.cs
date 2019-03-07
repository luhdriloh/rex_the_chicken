using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    private void OnTriggerEnter2D()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
