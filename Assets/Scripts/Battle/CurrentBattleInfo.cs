using UnityEngine;

public class CurrentBattleInfo
{
    public enum BattleReward
    {
        NONE,
        CARDS_OF_CLASS,
        SPECIFIC_CARD,
    }

    protected static Color deckColor;
    protected static Classes enemyDeckClass;
    protected static AudioClip bgm;
    protected static BattleReward rewardType;
    protected static Sprite BattleIcon;

    public static void PrepareBattle
        (
            Sprite battleIcon,
            BattleReward rewardType,
            Classes masterClassParam = Classes.NOT_A_CLASS,
            AudioClip bgmParam = null
        )
    {
        PrepareBattle(battleIcon, Color.gray, rewardType, masterClassParam, bgmParam);
    }

    public static void PrepareBattle
        (
            Sprite battleIcon,
            Color deckColorParam,
            BattleReward rewardTypeParam,
            Classes masterClassParam = Classes.NOT_A_CLASS, 
            AudioClip bgmParam = null
        )
    {
        deckColor = masterClassParam == Classes.NOT_A_CLASS ? deckColorParam : ClassInfo.GetColorOfClass(masterClassParam);
        enemyDeckClass = masterClassParam;
        bgm = bgmParam;
        BattleIcon = battleIcon;

        rewardType = rewardTypeParam;
    }
}