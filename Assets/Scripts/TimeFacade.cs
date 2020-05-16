using System.Collections;
using UnityEngine;

public class TimeFacade : OpenersSuperclass
{
    private static bool timeIsStopped = false;

    private static TimeFacade timeFacadeOfCurrentScene = null;

    public static bool TimeIsStopped { get => timeIsStopped; set => timeIsStopped = value; }
    public static float DeltaTime 
    {
        get 
        { 
            if (timeIsStopped)
            {
                return 0;
            }
            else
            {
                return Time.deltaTime;
            }
        }
    }

    private void Awake()
    {
        timeFacadeOfCurrentScene = this;
    }

    public static float GetDeltaTimeEvenIfTimeIsStopped()
    {
        return Time. deltaTime;
    }

    public static void RestoreTimeInNextFrameIfAllPopUpsAreClosed()
    {
        timeFacadeOfCurrentScene.StartCoroutine(timeFacadeOfCurrentScene.restoreTime());
    }

    private IEnumerator restoreTime()
    {
        yield return null;
        if (openerOfPopUpsMadeInEditor.AllPopUpsAreClosed())
        {
            timeIsStopped = false;
        }
    }
}
