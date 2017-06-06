using UnityEngine;

/// <summary>
/// Singleton Helper class
/// Inherit from Singleton and use the class as T
/// </summary>
/// <typeparam name="T">
/// Class name that will be the singleton.
/// T will be a MonoBehaviour.
/// </typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (!Application.isPlaying)
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

            return _instance as T;
        }
    }

    public bool dontDestroyOnLoad;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject.GetComponent<T>());
        }
        else
            Destroy(gameObject.GetComponent<T>());
    }
}
