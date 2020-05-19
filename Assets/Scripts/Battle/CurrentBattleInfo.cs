using UnityEngine;

public class CurrentBattleInfo
{
    protected static Color backgroundColor;
    protected static Classes enemyDeckClass;
    protected static AudioClip bgm;

    public static void PrepareBattle(Classes masterClassParam = Classes.NOT_A_CLASS, AudioClip bgmParam = null)
    {
        PrepareBattle(Color.gray, masterClassParam, bgmParam);
    }

    public static void PrepareBattle(Color backgroundColorParam, Classes masterClassParam = Classes.NOT_A_CLASS, AudioClip bgmParam = null)
    {
        backgroundColor = masterClassParam == Classes.NOT_A_CLASS ? backgroundColorParam : ClassInfo.GetColorOfClass(masterClassParam);
        enemyDeckClass = masterClassParam;
        bgm = bgmParam;
    }

    public static bool IsMasterBattle()
    {
        return enemyDeckClass != Classes.NOT_A_CLASS && enemyDeckClass != Classes.MONSTER;
    }

}