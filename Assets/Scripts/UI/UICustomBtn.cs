using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICustomBtn : UIBtn
{
    public ThePopUpOpenerInstance.OnConfirmBtnClicked onClicked;

    [SerializeField]
    private PopUpOpener popUpOpener;

    public void CallOnConfirmBtnClickedDelegate()
    {
        onClicked();
    }
}
