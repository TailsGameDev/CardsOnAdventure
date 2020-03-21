using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFSM : MonoBehaviour
{
    [SerializeField]
    private BattleStatesFactory firstOneToPlayStatesFactory = null;

    private BattleState currentState;

    [SerializeField]
    private string currentStateName;

    void Awake()
    {
        currentState = firstOneToPlayStatesFactory.CreateGameStartState();
    }

    void Update()
    {
        currentState.ExecuteAction();
        currentState = currentState.GetNextState();

        currentStateName = currentState.GetType().Name;
    }
}
