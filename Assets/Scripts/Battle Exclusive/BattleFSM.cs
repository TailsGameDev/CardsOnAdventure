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

    private bool safeToUpdate = false;

    void Start()
    {
        currentState = firstOneToPlayStatesFactory.CreateGameStartState();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        yield return null;
        safeToUpdate = true;
    }

    void Update()
    {
        if (safeToUpdate)
        {
            currentState.ExecuteAction();
            currentState = currentState.GetNextState();

            currentStateName = currentState.GetType().Name;

            string somebodiesTurn = "Enemy's Turn";

            if (currentState.IsPlayerTurn())
            {
                somebodiesTurn = "Your Turn";
            }
            
            feedbackText.text = "* "+currentStateName + ". It's " +somebodiesTurn;
            
        }
    }
}
