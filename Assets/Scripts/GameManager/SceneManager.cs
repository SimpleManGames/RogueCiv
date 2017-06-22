using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField]
    [Tooltip("List of scenes that will be loaded when starting this scene")]
    public List<string> scenesToLoadAdditivily = new List<string>();

    new private void Awake()
    {
        base.Awake();

        foreach (string scene in scenesToLoadAdditivily)
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
