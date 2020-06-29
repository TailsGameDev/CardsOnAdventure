using UnityEngine;

public class BuffClass : IncidentAction
{
    [SerializeField]
    private Classes classe = Classes.NOT_A_CLASS;

    [SerializeField]
    private PreMadeAudioFactory preMadeAudioFactory = null;

    public override void Execute()
    {
        customPopUpOpener.OpenDisplayingCardsOfClass(
                                title: "You Bought Equips",
                                   Formater.Paint(" ALL YOUR " + classe + " CARDS WILL BE BUFFED. PLEASE CHOOSE:", ClassInfo.GetColorOfClass(classe)),
                                confirmBtnMessage: "+1 Vitality",
                                cancelBtnMessage: "+1 Attack Power",
                                onConfirm: ImproveVitalityThenSeeMap,
                                onCancel: ImproveAttackPowerThenSeeMap,
                                preMadeAudioFactory.CreateVictoryAudioRequest(assignor: gameObject),
                                classe
                            );
    }

    private void ImproveAttackPowerThenSeeMap()
    {
        ClassInfo.GiveAttackPowerBonusToClass(classe);
        sceneOpener.OpenMapScene();
    }
    private void ImproveVitalityThenSeeMap()
    {
        ClassInfo.GiveVitalityBonusToClass(classe);
        sceneOpener.OpenMapScene();
    }
}
