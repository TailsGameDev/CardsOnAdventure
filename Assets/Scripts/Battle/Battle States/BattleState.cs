using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BattleState : CurrentBattleInfo
{
    public abstract void ExecuteAction();
    public abstract BattleState GetNextState();

    protected static BattleStatesFactory currentBattleStatesFactory;

    protected static BattleStatesFactory playerBattleStatesFactory;
    protected static BattleStatesFactory enemyBattleStatesFactory;

    public bool IsPlayerTurn()
    {
        return currentBattleStatesFactory == playerBattleStatesFactory;
    }
}

