using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // This function can be called from the button click event
    public void LoadScene(string sceneName)
    {
        // Load the scene with the given name
        SceneManager.LoadScene(sceneName);
    }
}