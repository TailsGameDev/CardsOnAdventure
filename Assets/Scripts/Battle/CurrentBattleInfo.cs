using UnityEngine;

public class CurrentBattleInfo
{
    protected static Color backgroundColor;
    protected static Classes enemyDeckClass;
    protected static AudioClip bgm;
    protected static bool IsMasterBattle;

    public static void PrepareBattle(bool isMasterBattle = false, Classes masterClassParam = Classes.NOT_A_CLASS, AudioClip bgmParam = null)
    {
        PrepareBattle(Color.gray, isMasterBattle, masterClassParam, bgmParam);
    }

    public static void PrepareBattle(Color backgroundColorParam, bool isMasterBattle = false, Classes masterClassParam = Classes.NOT_A_CLASS, AudioClip bgmParam = null)
    {
        backgroundColor = masterClassParam == Classes.NOT_A_CLASS ? backgroundColorParam : ClassInfo.GetColorOfClass(masterClassParam);
        enemyDeckClass = masterClassParam;
        bgm = bgmParam;
        IsMasterBattle = isMasterBattle;
    }
}