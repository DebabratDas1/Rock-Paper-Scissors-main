using UnityEngine;
using static GameManager;

public class AIController : MonoBehaviour
{
    private GameManager.Choice[] choices = { GameManager.Choice.Rock, GameManager.Choice.Paper, GameManager.Choice.Scissors };
    private GameManager.Choice aiChoice = GameManager.Choice.None;

    public int aiPoint = 50;

    public void MakeChoice(Choice playerChoice) {

        aiChoice = choices[Random.Range(0, choices.Length)];
        while (aiChoice == playerChoice)
            aiChoice = choices[Random.Range(0, choices.Length)];
    }

    public GameManager.Choice GetChoice() {
        return aiChoice;
    }
}
