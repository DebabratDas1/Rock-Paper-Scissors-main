using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameManager.Choice playerChoice = GameManager.Choice.None;

    public int playerPoint = 50;

    [SerializeField] private Button rockButton;
    [SerializeField] private Button paperButton;
    [SerializeField] private Button scissorsButton;

    private void Start() {
        rockButton.onClick.AddListener(() =>
        {
            ChooseRock();
        });

        paperButton.onClick.AddListener(() =>
        {
            ChoosePaper();
        });

        scissorsButton.onClick.AddListener(() =>
        {
            ChooseScissors();
        });
    }


    public void ChooseRock() {
        playerChoice = GameManager.Choice.Rock;
        SFXManager.Instance.onSelectRPS(0);
        GameManager.Instance.PlayerMadeChoice();
    }

    public void ChoosePaper() {
        playerChoice = GameManager.Choice.Paper;
        SFXManager.Instance.onSelectRPS(1);
        GameManager.Instance.PlayerMadeChoice();
    }

    public void ChooseScissors() {
        playerChoice = GameManager.Choice.Scissors;
        SFXManager.Instance.onSelectRPS(2);
        GameManager.Instance.PlayerMadeChoice();
    }

    public GameManager.Choice GetChoice() {
        return playerChoice;
    }
}
