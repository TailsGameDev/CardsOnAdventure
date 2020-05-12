using UnityEngine;

public class ShakeOnAwake : MonoBehaviour
{
    [SerializeField]
    private Shakeable shakeable = null;

    private void Awake()
    {
        shakeable.Shake();
    }
}
