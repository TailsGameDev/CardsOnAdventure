using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : IncidentAction
{
    [SerializeField]
    private MapScroller mapScroller = null;

    [SerializeField]
    private float targetLeft = 0.0f;

    public override void Execute()
    {
        mapScroller.InterpolateLeftTo(targetLeft);
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
    }
}
