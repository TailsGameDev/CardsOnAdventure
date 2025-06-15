using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Debug.LogWarning("[Singleton] " + this.name + " should have only one instance on scene.", this);
            Destroy(this);
        }
    }
    protected virtual void OnDestroy()
    {
        if (instance != this) // Skip if instance is other than this object.
        {
            return;
        }

        instance = null;
    }
}