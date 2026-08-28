using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T _instance;
    [SerializeField]
    private bool IsDontDestroyOnLoad = false;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;

        if (IsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
    }
}