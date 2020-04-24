using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class BattleFSM : MonoBehaviour
{
    [SerializeField]
    private BattleStatesFactory firstOneToPlayStatesFactory = null;

    private BattleState oldState;

    [SerializeField]
    private TextThatExpandsOnUpdated feedbackText = null;
    [SerializeField]
    private string prefixToFeedbackText = "";
    [SerializeField]
    private string sufixToFeedbackText = "";

    private bool safeToUpdate = false;

    void Start()
    {
        oldState = firstOneToPlayStatesFactory.CreateGameStartState();
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
            oldState.ExecuteAction();

            BattleState newState = oldState.GetNextState();
            
            oldState = newState;
            
            string turnInfo = " (Enemy's Turn)";
            if (newState.IsPlayerTurn())
            {
                turnInfo = " (Your Turn)";
            }

            string formattedStateName = Regex.Replace(newState.GetType().Name, "([A-Z])", " $1").Trim();

            feedbackText.DisplayText( prefixToFeedbackText + formattedStateName + turnInfo + sufixToFeedbackText );
        }
    }
}
