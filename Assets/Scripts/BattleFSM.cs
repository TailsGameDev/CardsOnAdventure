using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFSM : MonoBehaviour
{
    // change to enemyBattleStatesFactory if you want the enemy to be the first one to play
    [SerializeField]
    private BattleStatesFactory playerBattleStatesFactory = null;

    private BattleState currentState;

    [SerializeField]
    private string currentStateName;

    void Awake()
    {
        currentState = playerBattleStatesFactory.CreateGameStartState();
    }

    void Update()
    {
        currentState.ExecuteAction();
        currentState = currentState.GetNextState();

        currentStateName = currentState.GetType().Name;
    }
}
