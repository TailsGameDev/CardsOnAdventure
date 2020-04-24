using UnityEngine;
using System.Collections;

public class BattleRulesTipReceptor : TipsDragAndDropReceptor
{
    public override void OnDroppedInReceptor()
    {
        ((BattleRulesPopUpOpener)popUpOpener).OpenBattleRulesPopUp();
    }
}
