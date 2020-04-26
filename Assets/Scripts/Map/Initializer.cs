using UnityEngine;
using System.Collections;

public class Initializer : MonoBehaviour
{

    private MapsRuntimeCache mapsRuntimeCache;

    private void Awake()
    {
        mapsRuntimeCache = new MapsRuntimeCache();
    }
}
