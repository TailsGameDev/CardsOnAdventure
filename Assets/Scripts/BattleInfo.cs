using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfo
{
    public enum BattleType
    {
        simple,
        difficult,
        master,
        boss
    }

    protected static BattleType typeOfBattle;

    public static BattleType TypeOfBattle { set => typeOfBattle = value; }
}
