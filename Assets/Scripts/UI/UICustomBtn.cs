using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UICustomBtn : UIBtn
{
    public CustomPopUp.OnBtnClicked onClicked;

    public override void OnPointerUp()
    {
        ConfigureBtnLooks(normalSprite, originalRectTransfmOffsetMaxDotY, BtnUpTextColor);
        CallOnBtnClickedDelegate();
    }

    public void CallOnBtnClickedDelegate()
    {
        onClicked();
    }
}
