using UnityEngine.UI;

public class UICustomBtn : UIBtn
{
    public CustomPopUp.OnBtnClicked onClicked;

    public Text GetTextComponent()
    {
        return GetComponentInChildren<Text>();
    }

    public void ChangeText(string newText)
    {
        GetComponentInChildren<Text>().text = newText;
    }

    public override void OnPointerUp()
    {
        ConfigureBtnLooks(normalSprite, originalRectTransfmOffsetMaxDotY, originalRectTransformOffsetMinDotY, BtnUpTextColor);
        onClicked();
    }
}
