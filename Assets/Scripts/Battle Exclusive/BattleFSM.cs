using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFSM : MonoBehaviour
{
    [SerializeField]
    private BattleStatesFactory firstOneToPlayStatesFactory = null;

    private BattleState currentState;

    [SerializeField]
    private string currentStateName;

    [SerializeField]
    private Text feedbackText = null;

    void Awake()
    {
        currentState = firstOneToPlayStatesFactory.CreateGameStartState();
    }

    void Update()
    {
        currentState.ExecuteAction();
        currentState = currentState.GetNextState();

        currentStateName = currentState.GetType().Name;

        string somebodiesTurn = "Enemy's Turn";

        if (currentState.IsPlayerTurn())
        {
            somebodiesTurn = "Your Turn";
        }

        feedbackText.text = currentStateName + ". It's " +somebodiesTurn;
    }
}
