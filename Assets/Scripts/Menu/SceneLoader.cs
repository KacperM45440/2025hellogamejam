using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadScene(bool setActive, GameObject loadCanvas)
    {
        loadCanvas.SetActive(setActive);

    }
}