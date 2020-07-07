using UnityEngine;

public class CurrentBattleInfo
{
    protected static Color deckColor;
    protected static Classes enemyDeckClass;
    protected static AudioClip bgm;
    protected static bool GiveRewardToSameClassOfMasterDeckOnWin;
    protected static Sprite BattleIcon;

    public static void PrepareBattle
        (
            Sprite battleIcon,
            bool giveRewardToSameClassOfMasterDeckOnWin = false,
            Classes masterClassParam = Classes.NOT_A_CLASS,
            AudioClip bgmParam = null
        )
    {
        PrepareBattle(battleIcon, Color.gray, giveRewardToSameClassOfMasterDeckOnWin, masterClassParam, bgmParam);
    }

    public static void PrepareBattle
        (
            Sprite battleIcon,
            Color deckColorParam,
            bool giveRewardToSameClassOfMasterDeckOnWin = false, 
            Classes masterClassParam = Classes.NOT_A_CLASS, 
            AudioClip bgmParam = null
        )
    {
        deckColor = masterClassParam == Classes.NOT_A_CLASS ? deckColorParam : ClassInfo.GetColorOfClass(masterClassParam);
        enemyDeckClass = masterClassParam;
        bgm = bgmParam;
        GiveRewardToSameClassOfMasterDeckOnWin = giveRewardToSameClassOfMasterDeckOnWin;
        BattleIcon = battleIcon;
    }
}