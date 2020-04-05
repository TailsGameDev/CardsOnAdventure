using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineExecutorPrototype : MonoBehaviour
{
    #region Prototype

    private static CoroutineExecutorPrototype prototype;

    private void Awake()
    {
        // this is not a Singleton. There is the prototype obj, and how many copies the game may need.
        if (!prototype)
        {
            prototype = this;
        }
    }

    public static CoroutineExecutorPrototype GetCopy()
    {
        GameObject copy = Instantiate(prototype.gameObject);

        return copy.GetComponent<CoroutineExecutorPrototype>();
    }
    #endregion

    #region Coroutine Executor
    public void ExecuteCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Custom Update Executor
    /*
    private CustomUpdate customUpdate;
    private int frameCounter = 0;
    private int countLimit = 0;

    private void Update()
    {
        if (customUpdate != null && frameCounter < countLimit)
        {
            frameCounter++;
            customUpdate.Execute();
        }
    }

    public void ExecuteCustomUpdateUntillCountLimit(CustomUpdate customUpdate, int countLimit)
    {
        frameCounter = 0;
        this.countLimit = countLimit;
        this.customUpdate = customUpdate;
    }
    */
    #endregion
}
