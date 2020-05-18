using UnityEngine;
using System.Collections;

public class IncidentPopUp : MonoBehaviour
{

    [SerializeField]
    private string title = null;

    [SerializeField]
    private string message = null;

    [SerializeField]
    private string confirmBtnText = null;
    [SerializeField]
    private string cancelBtnText = null;

    [SerializeField]
    private IncidentAction ConfirmAction = null;
    [SerializeField]
    private IncidentAction CancelAction = null;

    public void Open(CustomPopUp incidentPopUpOpener)
    {
        incidentPopUpOpener.OpenAndMakeUncloseable
            (
                title,
                message.Replace("<br>","\n"),
                confirmBtnText,
                cancelBtnText,
                ConfirmAction.Execute,
                CancelAction.Execute
            );
    }
}
