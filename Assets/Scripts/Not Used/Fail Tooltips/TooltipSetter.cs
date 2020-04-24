using UnityEngine;

namespace NotUsed
{
    public class TooltipSetter : MonoBehaviour
    {
        protected TooltipDisplayer tooltipDisplayer;

        [SerializeField]
        protected string tip = "";

        [SerializeField]
        protected float duration = 1.0f;

        [SerializeField]
        protected int width = 60;

        [SerializeField]
        protected int height = 17;

        protected void Start()
        {
            tooltipDisplayer = TooltipDisplayer.instance;
        }

        public virtual void OnPointerClicked()
        {
            Debug.LogError("WTF called");
            if (tooltipDisplayer != null)
            {
                tooltipDisplayer.SetMessageAndTimerAndSize(tip, duration, width, height);
            }
            else
            {
                Debug.LogError("tooltipDisplayer is null, and I don't know why. TODO: find it out and avoid it");
            }
        }
    }
}