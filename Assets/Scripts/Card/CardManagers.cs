using UnityEngine;

// Its just for not destroying it's children, ant not permit duplicates of them
public class CardManagers : MonoBehaviour
{
    private static CardManagers instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GetComponent<RectTransform>().SetParent(null, false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
