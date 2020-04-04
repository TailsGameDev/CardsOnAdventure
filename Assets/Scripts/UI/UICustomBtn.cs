using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICustomBtn : UIBtn
{
    public CustomPopUp.OnBtnClicked onClicked;

    [SerializeField]
    private PopUpOpener popUpOpener;

    public void CallOnBtnClickedDelegate()
    {
        onClicked();
    }
}
