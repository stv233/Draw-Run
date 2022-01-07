using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<T>();
                if (!_instance)
                {
                    Debug.LogError($"Singleton of type {typeof(T)} not contains in scene");
                    return null;
                }

                _instance.AwakeSingletone();
            }

            return _instance;
        }
    }

    public static bool InstanceIsNotNull => _instance;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = GetComponent<T>();
            AwakeSingletone();
        }
        else if (_instance != this)
            Debug.LogError($"Dublicated singleton instance {nameof(T)}", this);
    }

    protected virtual void AwakeSingletone() { }
}
