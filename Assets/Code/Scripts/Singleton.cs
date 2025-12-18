using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected virtual void OnAwake() { } // Awake() 대신 사용. 자기 자신만의 초기화가 필요할 때만 사용.
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    Debug.LogWarning(typeof(T) + " Singleton was not found in the scene.");
                }
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        OnAwake();
        // DontDestroyOnLoad(gameObject);
    }

    protected void OnApplicationQuit()
    {
        // 에디터에서 유령 객체가 남는 것을 방지
        _instance = null;
    }
}