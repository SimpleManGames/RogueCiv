using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField]
    [Tooltip("List of scenes that will be loaded when starting this scene")]
    public List<string> scenesToLoadAdditivily = new List<string>();

    private Player _playerRef;
    public Player PlayerRef
    {
        get {
            if (_playerRef == null)
                _playerRef = FindObjectOfType<Player>();

            return _playerRef;
        }
    }

    new private void Awake()
    {
        base.Awake();

        foreach (string scene in scenesToLoadAdditivily)
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
