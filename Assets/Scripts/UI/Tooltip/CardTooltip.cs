using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTooltip : TooltipSetter
{
    [SerializeField]
    private Card card = null;

    public override void OnPointerClicked()
    {
        base.tip = card.GetSkillFullName();

        base.OnPointerClicked();
    }
}
