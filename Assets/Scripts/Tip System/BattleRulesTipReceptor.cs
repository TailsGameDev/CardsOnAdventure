using UnityEngine;
using System.Collections;

public class BattleRulesTipReceptor : TipReceptor
{
    [SerializeField]
    private BattleRulesPopUpOpener battleRulesPopUpOpener = null;

    public override void OpenTip(TipPopUpOpener _)
    {
        battleRulesPopUpOpener.OpenBattleRulesPopUp();
    }
}
