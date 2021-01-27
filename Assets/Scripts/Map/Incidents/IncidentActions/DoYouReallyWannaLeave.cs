using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoYouReallyWannaLeave : IncidentAction
{
    [SerializeField]
    private IncidentAction earnCard = null;
    [SerializeField]
    private IncidentAction justLeave = null;

    public override void Execute()
    {
        customPopUpOpener.Open(
            title: "Are You Sure?",
            warningMessage: "If you just leave, you won't get any card.",
            confirmBtnMessage: "Get a Card",
            cancelBtnMessage: "Just Leave",
            onConfirm: earnCard.Execute,
            onCancel: justLeave.Execute
            );
    }
}
