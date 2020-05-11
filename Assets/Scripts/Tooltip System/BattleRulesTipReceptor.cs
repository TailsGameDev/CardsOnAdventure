using UnityEngine;
using System.Collections;

public class BattleRulesTipReceptor : TipReceptor
{
    public override void OnDroppedInReceptor()
    {
        ((BattleRulesPopUpOpener)popUpOpener).OpenBattleRulesPopUp();
    }
}
