using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple scene manager used to restart the scene
/// with an input.
/// </summary>
public class SceneManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    private void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
