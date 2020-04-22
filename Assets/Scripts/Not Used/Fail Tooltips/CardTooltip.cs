using UnityEngine;

namespace NotUsed
{
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
}
