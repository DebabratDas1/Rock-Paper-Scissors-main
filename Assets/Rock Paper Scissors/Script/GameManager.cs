using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using static GameManager;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public enum Choice { None, Rock, Paper, Scissors }
	public enum Turn { AI, Player }
	public enum Equation { Plus, Minus, Multiplication, Division, PlusPer, Steal }

	public List<GameCardEquation> _GameCardEquations;
	public List<GameCardEquation> gameCardEquations;

	public Sprite cardDefaultImage;

	[SerializeField] private Animator centerTextAnim;

	[Space(2f)]
	[Header("Image")]
	[SerializeField] private Image playerHandImage;
	[SerializeField] private Image aiHandImage;
	[SerializeField] private GameObject winImageKey;

	[Space(2f)]
	[Header("List")]
	[SerializeField] private List<Sprite> decisionImageRPS;
	[SerializeField] private List<Button> playerChoiceButton;
	[SerializeField] private List<Card> allCardScript;
	[SerializeField] private List<RectTransform> allCardRefe;
	[SerializeField] private List<GameObject> pickedCards = new List<GameObject>();
	[SerializeField] private List<GameObject> selectedButtonPlayer;
	[SerializeField] private List<GameObject> selectedButtonAI;
	[SerializeField] private List<RoundWinData> roundWinDataList = new List<RoundWinData>();
	[SerializeField] private List<Sprite> outerLineSprite;
	[SerializeField] private List<Sprite> keySprite;
	[SerializeField] private RectTransform cardPlayerRef, cardAiRef;

	[Space(2f)]
	[Header("GameObjects")]
	[SerializeField] private GameObject cardPrefab;

	[SerializeField] private GameObject PlayerAskSendReceive;
	[SerializeField] private GameObject cardPanel;
	[SerializeField] private GameObject middleAnimatedText;
	[SerializeField] private GameObject playerNoneChoicePanel;
	[SerializeField] private GameObject winLostShowCountDown;
	[SerializeField] private Image middleResultImage;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button ClaimBtn;
	[SerializeField] private Button ClaimBtn2;
	[SerializeField] private RectTransform playerRPSObject, playerRPSRefe, playerRPSMdl;
	[SerializeField] private RectTransform aiRPSObject, aiRPSRefe, aiRPSMdl;


	[Space(2f)]
	[Header("Text")]
	public Text playername;
	public Text playerScoreText;
	public Text aiScoreText;
	public Text statusText;
	public Text cardPickStatus;
	public List<Text> coinTexts;

	[SerializeField] private Text winLostShowCountDownText;
	[SerializeField] private PlayerController player;
	[SerializeField] private AIController ai;

	private int defaultScore = 50;
	public int playerScore;
	private int aiScore;
	private int playerRoundWins = 0;
	private int aiRoundWins = 0;

	private int roundId = 1;
	private int remainingCards = 8;
	[SerializeField] private Turn currentTurn;

	[Space(2f)]
	[Header("Round Info Panel")]
	[SerializeField] private GameObject roundInfoPanel;
	[SerializeField] private Text roundText;
	[SerializeField] private Text roundUpperText;

	[Space(2f)]
	[Header("Win Panel")]
	[SerializeField] private GameObject winPanel;
	[SerializeField] private GameObject winPanelAnimPar;
	[SerializeField] private Button nextRoundButtonWin;
	[SerializeField] private Button homeButtonWin;
	[SerializeField] private Image middleWinImage;
	[SerializeField] private Sprite[] middleWinSprite;
	[SerializeField] private Text nextButtonWinText;
	[SerializeField] private Text homeButtonWinText;
	[SerializeField] private Text finalWinMessageText;
	[SerializeField] private List<Image> playerOuterLineImage;
	[SerializeField] private List<Image> playerKeyImage;
	[SerializeField] private List<Image> aiOuterLineImage;
	[SerializeField] private List<Image> aiKeyImage;


	[Space(2f)]
	[Header("Lose Panel")]
	[SerializeField] private GameObject losePanel;
	[SerializeField] private GameObject losePanelAnimPar;
	[SerializeField] private Button nextRoundButtonLose;
	[SerializeField] private Button homeButtonLose;
	[SerializeField] private Image middleLoseImage;
	[SerializeField] private Sprite[] middleLoseSprite;
	[SerializeField] private Text nextButtonLoseText;
	[SerializeField] private Text homeButtonLoseText;
	[SerializeField] private Text finalLoseMessageText;
	[SerializeField] private List<Image> playerOuterLineImageLose;
	[SerializeField] private List<Image> playerKeyImageLose;
	[SerializeField] private List<Image> aiOuterLineImageLose;
	[SerializeField] private List<Image> aiKeyImageLose;


	[Header("URL")]
	[SerializeField] private string url;

	[Header("Main Screen:")]
	[SerializeField] private GameObject homeUI;
	[SerializeField] private GameObject playUI;

	[SerializeField] private Card[][] cardAry = new Card[4][];
	[SerializeField] private int[][] cardAryIndex = new int[4][];

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		//homeUI.SetActive(true);
		//playUI.SetActive(false);
	}

	private void Start()
	{
		aiScore = playerScore = defaultScore;
		restartButton.onClick.RemoveAllListeners();
		restartButton.onClick.AddListener(() =>
		{
			RestartGameButton();
		});

		SetCardInArray();
		ClearGamePlayButton();
		UpdateScoreText();
		PrintUpperRoundText();
		//ClearGamePlayButton();
		//UpdateScoreText();
		//StartCoroutine(StartGame());
	}

	private void SetCardInArray()
	{
		cardAry[0] = new Card[1];
		cardAry[1] = new Card[2];
		cardAry[2] = new Card[3];
		cardAry[3] = new Card[3];

		cardAryIndex[0] = new int[1];
		cardAryIndex[1] = new int[2];
		cardAryIndex[2] = new int[3];
		cardAryIndex[3] = new int[3];

		int cnt = 0;
		for (int i = 0; i < cardAry.Length; i++)
		{
			for (int j = 0; j < cardAry[i].Length; j++)
			{
				cardAry[i][j] = allCardScript[cnt];
				cardAryIndex[i][j] = cnt;
				cnt++;
			}
		}
	}

	public void GameStart()
	{
		centerTextAnim.enabled = true;
		StartCoroutine(StartGame());
		
		
        gameCardEquations = ShuffleList(_GameCardEquations);
	}

	List<GameCardEquation> ShuffleList(List<GameCardEquation> temp)
	{

		List<GameCardEquation> random = new List<GameCardEquation>();
		for (int i = 0; i < 10; i++)
		{
			GameCardEquation game = _GameCardEquations[Random.Range(0, _GameCardEquations.Count)];
			while (random.Contains(game))
				game = _GameCardEquations[Random.Range(0, _GameCardEquations.Count)];
			temp.Add(game);
		}
		Debug.Log($"temp.Count = {temp.Count}");

		List<GameCardEquation> shuffleList = new List<GameCardEquation>(temp);
		for (int i = shuffleList.Count - 1; i > 0; i--)
		{
			int randomIndex = Random.Range(0, i + 1);
			(shuffleList[i], shuffleList[randomIndex]) = (shuffleList[randomIndex], shuffleList[i]);
		}
		Debug.Log($"shuffleList.Count = {shuffleList.Count}");

		return shuffleList;
	}

	private void PrintUpperRoundText()
	{
		roundUpperText.text = "ROUND " + roundId;
	}


	bool isPlayerChoice = false;
	IEnumerator StartGame()
	{
		Debug.Log("StartGame");
        playername.text = PlayerPrefs.GetString("Address", "null");
        print("player name is -- " + playername.text);
        playerRPSObject.DOMove(playerRPSMdl.position, .5f);
		aiRPSObject.DOMove(aiRPSMdl.position, .5f);

		//OpenRoundInfoPanel();
		//yield return new WaitForSeconds(1.5f);
		roundInfoPanel.SetActive(false);
		ClearGamePlayButton();

		player.playerChoice = Choice.None;

		statusText.text = $"Round {roundId} - Ready";

		statusText.text = "Steady";

		statusText.text = "Go!";
		//yield return new WaitForSeconds(0.5f);
		playerChoiceButton.ForEach(x => x.interactable = true);
		//yield return new WaitForSeconds(0.5f);

		yield return new WaitUntil(() => isPlayerChoice);
		isPlayerChoice = false;
		ai.MakeChoice(player.playerChoice);

		//selectedButtonAI[(int)ai.GetChoice() - 1].SetActive(true);
		selectedButtonAI[(int)ai.GetChoice() - 1].GetComponent<Image>().enabled = true;

		if (player.GetChoice() == Choice.None)
		{
			//OpenPlayerNoneChoice();
		}
		else
		{
			ShowResult();
		}
	}

	public void PlayerMadeChoice()
	{
		SelectGamePlayButton(player.playerChoice);
		isPlayerChoice = true;
	}

	private void ShowResult()
	{
		Debug.Log("ShowResult");
		Choice playerChoice = player.GetChoice();
		Choice aiChoice = ai.GetChoice();

		playerRPSObject.DOMove(playerRPSRefe.position, .5f);
		aiRPSObject.DOMove(aiRPSRefe.position, .5f);

		statusText.text = $"Player: {playerChoice} | AI: {aiChoice}\n";

		if (playerChoice == aiChoice)
		{
			statusText.text += "It's a Draw!";
			StartCoroutine(RestartGame());
			return;
		}

		bool isPlayerWin = false;
		if ((playerChoice == Choice.Rock && aiChoice == Choice.Scissors) ||
			(playerChoice == Choice.Paper && aiChoice == Choice.Rock) ||
			(playerChoice == Choice.Scissors && aiChoice == Choice.Paper))
		{
			statusText.text += "Player Wins!";
			currentTurn = Turn.Player;
			isPlayerWin = true;
			middleResultImage.sprite = decisionImageRPS[(int)playerChoice];
		}
		else
		{
			middleResultImage.sprite = decisionImageRPS[(int)aiChoice];
			statusText.text += "AI Wins!";
			currentTurn = Turn.AI;
		}
		middleResultImage.gameObject.SetActive(true);
		middleAnimatedText.gameObject.SetActive(false);
		middleResultImage.transform.DOScale(Vector3.one, 1f).From(.3f);

		DOTween.Sequence().AppendInterval(2f).OnComplete(() =>
		{
			if (isPlayerWin)
				OpenWinPanel(true);
			else
				OpenLosePanel(true);
			//StartCoroutine(StartCardPickingPhase());
		});
	}

	IEnumerator StartCardPickingPhase()
	{
		cardPickStatus.text = $"Round {roundId}: Card Picking Phase!";
		for (int i = 0; i < allCardScript.Count; i++)
		{
			allCardScript[i].transform.position = allCardRefe[i].position;
			allCardScript[i].transform.localScale = allCardRefe[i].localScale;
		}
		yield return new WaitForSeconds(.5f);
		GetReadyCardPicking();

		if (currentTurn == Turn.Player)
		{
			cardPickStatus.text = "Player's Turn! Pick the first card.";
		}
		else
		{
			StartCoroutine(AIPickCard());
		}
	}

	public int Index = 0;

	private void GetReadyCardPicking()
	{
		//Debug.Log
		cardPanel.SetActive(true);

		Index = 0;
		if (roundId == 2)
			Index = 9;
		else if (roundId == 3)
			Index = 18;
		Debug.Log("INdsex >> " + Index);
		allCardScript.ForEach(card => card.SetValue(Index++));

		while (CheckDraw())
		{
			Index = 0;
			if (roundId == 2)
				Index = 9;
			else if (roundId == 3)
				Index = 18;
			Debug.Log("INdsex >> " + Index);
			allCardScript.ForEach(card => card.SetValue(Index++));
		}
		Debug.Log("Check Draw --- " + CheckDraw());

		allCardScript.ForEach(card => card.SetCardValue(card.GetCardValue(), card.GetEquation()));

		pickedCards.Clear();

		remainingCards = 9;

		UpdateCardButtons();
	}

	public void PlayerPickedCard(int index)
	{
		if (pickedCards.Count == allCardScript.Count) return;
		if (currentTurn != Turn.Player) return;
		selectedCardIndex = index;
		PickCard(Turn.Player, index);
	}

	IEnumerator AIPickCard()
	{
		float randomTime = Random.Range(2f, 4f);
		randomTime = 2f;

		yield return new WaitForSeconds(randomTime);

		if (pickedCards.Count == allCardScript.Count) yield break;

		PickCard(Turn.AI, 0);

		UpdateScoreText();
		remainingCards--;

		if (remainingCards > 0)
		{
			currentTurn = Turn.Player;
			cardPickStatus.text = "Player's Turn! Pick the card.";
		}
		else
		{
			StartCoroutine(EndRound());
		}

		UpdateCardButtons();
	}

	IEnumerator EndRound()
	{
		Debug.Log("EndRound");

		middleResultImage.gameObject.SetActive(false);
		middleAnimatedText.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);
		cardPanel.SetActive(false);

		bool isPlayerWin = roundWinDataList.FindAll(x => x.playerRoundDetail == 1).Count > roundWinDataList.FindAll(x => x.aiRoundDetail == 1).Count;

		if (playerScore > aiScore)
		{
			playerRoundWins++;
		}
		else if (playerScore < aiScore)
		{
			aiRoundWins++;
		}
		Debug.Log("playerScore >> " + playerScore);
		Debug.Log("aiScore >> " + aiScore);

		if (roundId == 3)
		{
			if (playerRoundWins > aiRoundWins)
			{
				Debug.Log("Player wins the round!");
				statusText.text = "Player wins the round!";
				bool player = playerScore > aiScore;
				bool ai = playerScore < aiScore;
				SetWinningData(roundId, player, ai);
				//OpenWinPanel();
				StartCoroutine(OpenWinLoseCountDown(true));
				//playerRoundWins++;
			}
			else if (playerRoundWins < aiRoundWins)
			{
				Debug.Log("AI wins the round!");
				statusText.text = "AI wins the round!";
				bool player = playerScore > aiScore;
				bool ai = playerScore < aiScore;
				SetWinningData(roundId, player, ai);
				//OpenLosePanel();
				StartCoroutine(OpenWinLoseCountDown(false));
				//aiRoundWins++;
			}
		}
		else
		{
			if (playerScore > aiScore)
			{
				Debug.Log("Player wins the round!");
				statusText.text = "Player wins the round!";
				SetWinningData(roundId, true, false);
				//OpenWinPanel();
				StartCoroutine(OpenWinLoseCountDown(true));
				//playerRoundWins++;
			}
			else if (aiScore > playerScore)
			{
				Debug.Log("AI wins the round!");
				statusText.text = "AI wins the round!";
				SetWinningData(roundId, false, true);
				//OpenLosePanel();
				StartCoroutine(OpenWinLoseCountDown(false));
				//aiRoundWins++;
			}
			else
			{
				Debug.Log("Round is a draw!");
				SetWinningData(roundId, false, false);
				statusText.text = "Round is a draw!";
			}
		}

		/* yield return new WaitForSeconds(0.5f);

        if (roundId == 3) {
            StartCoroutine(EndGame());
        }
        else {
            roundId++;
            ResetRound();
            StartCoroutine(StartGame());
        } */
	}

	IEnumerator EndGame()
	{
		Debug.Log("EndGame");
		yield return new WaitForSeconds(1f);

		if (playerRoundWins > aiRoundWins)
		{
			statusText.text = "Player Wins the Game!";
		}
		else if (aiRoundWins > playerRoundWins)
		{
			statusText.text = "AI Wins the Game!";
		}
		else
		{
			statusText.text = "It's a Tie!";
		}
	}

	private void ResetRound()
	{
		Debug.Log("ResetRound");
		playerScore = aiScore = defaultScore;
		//aiScore;;
		UpdateScoreText(true);
	}

	public Text coinAmtText;
	public Text playerRefe;
	public Text aiRefe;
	private void UpdateScoreText(bool isReset = false)
	{
		int ai = int.Parse(aiScoreText.text.Replace("+", ""));
		int plyr = int.Parse(playerScoreText.text.Replace("+", ""));
		coinAmtText.gameObject.SetActive(false);

		if (!isReset)
		{
			if (ai != aiScore)
			{
				coinAmtText.transform.position = aiScoreText.transform.position;

				int inc = aiScore - ai;
				coinAmtText.text = inc > 0 ? $"+{inc}" : inc < 0 ? $"{inc}" : $"{inc}";
				coinAmtText.color = inc < 0 ? Color.red : inc == 0 ? Color.white : Color.green;

				coinAmtText.gameObject.SetActive(true);
				coinAmtText.transform.DOMove(aiRefe.transform.position, .4f).OnComplete(() => coinAmtText.gameObject.SetActive(false));
			}
			if (plyr != playerScore)
			{
				coinAmtText.transform.position = playerScoreText.transform.position;

				int inc = playerScore - plyr;
				coinAmtText.text = inc > 0 ? $"+{inc}" : inc < 0 ? $"{inc}" : $"{inc}";
				coinAmtText.color = inc < 0 ? Color.red : inc == 0 ? Color.white : Color.green;

				coinAmtText.gameObject.SetActive(true);
				coinAmtText.transform.DOMove(playerRefe.transform.position, .4f).OnComplete(() => coinAmtText.gameObject.SetActive(false));
			}
		}

		playerScoreText.color = playerScore < 0 ? Color.red : (playerScore > 0 ? Color.green : new Color(1f, 0.5f, 0f));
		aiScoreText.color = aiScore < 0 ? Color.red : (aiScore > 0 ? Color.green : new Color(1f, 0.5f, 0f));

		playerScoreText.text = playerScore > 0 ? $"+{playerScore}" : playerScore < 0 ? $"{playerScore}" : $"{playerScore}";
		aiScoreText.text = aiScore > 0 ? $"+{aiScore}" : aiScore < 0 ? $"{aiScore}" : $"{aiScore}";
	}

	IEnumerator RestartGame()
	{
		Debug.Log("RestartGame");
		yield return new WaitForSeconds(.8f);

		player.playerChoice = Choice.None;
		StartCoroutine(StartGame());
	}

	int selectedCardIndex = 0;

	public List<Card> cards = new List<Card>();
	public int prevFlip = -1;
	private void UpdateCardButtons()
	{
		Debug.Log("UpdateCardButtons");
		int index = allCardScript.Count - remainingCards;
		Debug.Log("UpdateCardButtons 11 " + index);
		Debug.Log("UpdateCardButtons 99 " + remainingCards);

		int aryIdx = (index == 0) ? 0 : (index >= 1 && index <= 2) ? 1 : (index >= 3 && index <= 5) ? 2 : (index >= 6 && index <= 7) ? 3 : -1;
		Debug.Log("UpdateCardButtons 22 " + aryIdx);
		Debug.Log("UpdateCardButtons 33 " + allCardScript.Count);


		if (index == allCardScript.Count)
		{
			return;
		}
		Debug.Log("UpdateCardButtons 44 " + cards.Count);
		if (aryIdx == -1) return;
		allCardScript.ForEach(x => x.Interactable(false));
		for (int i = 0; i < cardAry[aryIdx].Length; i++)
		{

			int idx = cardAryIndex[aryIdx][i];
			Debug.Log("XXXXXXXXXX || " + idx);
			allCardScript[idx].Interactable(true);
			if (aryIdx != prevFlip)
			{
				allCardScript[idx].FlipCard();
				cards.Add(allCardScript[idx]);
			}
		}

		prevFlip = aryIdx;
		Debug.Log("UpdateCardButtons 55 " + cards.Count);
	}

	private void PickCard(Turn turn, int index)
	{
		Equation equation;

		int selectedCardIndex = allCardScript.Count - remainingCards;
		selectedCardIndex = this.selectedCardIndex;
		Debug.LogWarning("selectedCard = " + selectedCardIndex + " || pickedCards.Count = " + pickedCards.Count);



		switch (turn)
		{
			case Turn.AI:
				selectedCardIndex = Random.Range(0, cards.Count);
				Debug.LogWarning("selectedCard = " + selectedCardIndex + " || cards.Count = " + cards.Count);
				Card aiCard = cards[selectedCardIndex];
				Debug.LogWarning("selectedCard = " + aiCard.name);
				int aiCardAmount = aiCard.GetCardValue();

				equation = aiCard.GetEquation();

				RectTransform cardAnimRef = cardAiRef;

				if (equation == Equation.Plus)
				{
					aiScore += aiCardAmount;
					cardAnimRef = cardAiRef;
				}
				else if (equation == Equation.Minus)
				{
					if (aiScore < 0)
					{
						aiScore -= ((aiScore * aiCardAmount) / 100);
						cardAnimRef = cardAiRef;
					}
					else
					{
						playerScore -= ((playerScore * aiCardAmount) / 100);
						cardAnimRef = cardPlayerRef;
					}
				}
				else if (equation == Equation.Multiplication)
				{
					if ((aiScore >= 0 && aiCardAmount != 0) || (aiScore <= 0 && aiCardAmount == 0))
					{
						aiScore *= aiCardAmount;
						cardAnimRef = cardAiRef;
					}
					else
					{
						playerScore *= aiCardAmount;
						cardAnimRef = cardPlayerRef;
					}
				}
				else if (equation == Equation.Division)
				{
					if (aiScore <= 0)
					{
						aiScore /= aiCardAmount;
						cardAnimRef = cardAiRef;
					}
					else
					{
						playerScore /= aiCardAmount;
						cardAnimRef = cardPlayerRef;
					}
				}
				else if (equation == Equation.PlusPer)
				{
					if (aiScore <= 0)
					{
						playerScore += ((playerScore * aiCardAmount) / 100);
						cardAnimRef = cardPlayerRef;
					}
					else
					{
						aiScore += ((aiScore * aiCardAmount) / 100);
						cardAnimRef = cardAiRef;
					}
				}
				else if (equation == Equation.Steal)
				{
					if (aiScore > 0)
					{
						playerScore = ((playerScore * aiCardAmount) / 100);
						cardAnimRef = cardPlayerRef;
					}
					else
					{
						aiScore = ((aiScore * aiCardAmount) / 100);
						cardAnimRef = cardAiRef;
					}
				}

				pickedCards.Add(aiCard.gameObject);
				cards.Remove(aiCard);

				aiCard.MoveAnimation(cardAnimRef, aiCard.gameObject);

				statusText.text = $"AI picked a card worth {aiCardAmount}!";

				break;
			case Turn.Player:
				//PlayerAskSendReceive.SetActive(true);
				SendReceive(0);
				break;
		}
	}

	private void SendReceivePlayerDecision()
	{

	}


	public void SendReceive(int n)
	{
		PlayerAskSendReceive.SetActive(false);
		if (n == -1) return;

		Card selectedCard = allCardScript[selectedCardIndex];
		int cardAmount = selectedCard.GetCardValue();
		Equation equation = selectedCard.GetEquation();

		RectTransform cardAnimRef = cardPlayerRef;

		if (equation == Equation.Plus)
		{
			playerScore += cardAmount;
			cardAnimRef = cardPlayerRef;
		}
		else if (equation == Equation.Minus)
		{
			if (playerScore < 0)
			{
				playerScore -= cardAmount;
				cardAnimRef = cardPlayerRef;
			}
			else
			{
				aiScore -= cardAmount;
				cardAnimRef = cardAiRef;
			}
		}
		else if (equation == Equation.Multiplication)
		{
			if ((playerScore >= 0 && cardAmount != 0) || (playerScore <= 0 && cardAmount == 0))
			{
				playerScore *= cardAmount;
				cardAnimRef = cardPlayerRef;
			}
			else
			{
				aiScore *= cardAmount;
				cardAnimRef = cardAiRef;
			}
		}
		else if (equation == Equation.Division)
		{
			if (playerScore <= 0)
			{
				playerScore /= cardAmount;
				cardAnimRef = cardPlayerRef;
			}
			else
			{
				aiScore /= cardAmount;
				cardAnimRef = cardAiRef;
			}
		}
		else if (equation == Equation.PlusPer)
		{
			if (playerScore <= 0)
			{
				aiScore += ((aiScore * cardAmount) / 100);
				cardAnimRef = cardAiRef;
			}
			else
			{
				playerScore += ((playerScore * cardAmount) / 100);
				cardAnimRef = cardPlayerRef;
			}
		}
		else if (equation == Equation.Steal)
		{
			if (playerScore > 0)
			{
				aiScore = ((aiScore * cardAmount) / 100);
				cardAnimRef = cardAiRef;
			}
			else
			{
				playerScore = ((playerScore * cardAmount) / 100);
				cardAnimRef = cardPlayerRef;
			}
		}

		pickedCards.Add(selectedCard.gameObject);
		cards.Remove(selectedCard);

		selectedCard.MoveAnimation(cardAnimRef, selectedCard.gameObject);

		statusText.text = $"Player picked a card worth {cardAmount}!";
		{
			UpdateScoreText();
			remainingCards--;
			Debug.Log("cards.Count ==1 " + cards.Count);
			UpdateCardButtons();
			Debug.Log("cards.Count ==2 " + cards.Count);

			if (remainingCards > 0)
			{
				cardPickStatus.text = "AI's Turn! Pick the card.";
				currentTurn = Turn.AI;
				Debug.Log("cards.Count ==3 " + cards.Count);
				StartCoroutine(AIPickCard());
				Debug.Log("cards.Count ==4 " + cards.Count);
			}
			else
			{
				StartCoroutine(EndRound());
			}
		}
	}

	private void CardMoveAnimation(RectTransform cardSendRef, GameObject moveCard)
	{
		moveCard.transform.DOMove(cardSendRef.position, .8f);
	}

	private void SelectGamePlayButton(Choice choice)
	{
		selectedButtonPlayer[(int)choice - 1].GetComponent<Image>().enabled = true;

		playerChoiceButton.ForEach(x => x.interactable = false);
	}

	private void ClearGamePlayButton()
	{
		foreach (var img in selectedButtonPlayer)
			if (img != null) img.gameObject.GetComponent<Image>().enabled = false;

		foreach (var img in selectedButtonAI)
			if (img != null) img.gameObject.GetComponent<Image>().enabled = false;

		playerChoiceButton.ForEach(x => x.interactable = true);
	}

	public void OpenPopUp()
	{

	}

	private void OpenPlayerNoneChoice()
	{
		playerNoneChoicePanel.SetActive(true);
	}

	public void RestartGameButton()
	{
		playerNoneChoicePanel.SetActive(false);
		StartCoroutine(RestartGame());
	}

	private void OpenRoundInfoPanel()
	{
		PrintUpperRoundText();
		roundText.text = "Round : " + roundId;
		roundInfoPanel.SetActive(true);
	}

	private void SetWinningData(int roundId, bool isPlayer, bool isAI)
	{
		roundWinDataList.Add(new RoundWinData());

		roundWinDataList[roundId - 1].playerRoundDetail = isPlayer ? 1 : 0;
		roundWinDataList[roundId - 1].aiRoundDetail = isAI ? 1 : 0;
	}



	private void OpenCardPanelAfterToss()
	{
		Debug.Log("next round call..");
		RoundCoinDescrease();
		Debug.Log("next round called..");

		losePanel.SetActive(false);
		winPanel.SetActive(false);
		if (roundId > 1)
		{
			//roundId++;
			//OpenRoundInfoPanel();
		}
		DOVirtual.DelayedCall(2f, () =>
		{
			roundInfoPanel.SetActive(false);
			StartCoroutine(StartCardPickingPhase());
		});
		//yield return new WaitForSeconds(1.5f);
	}


	private void OpenWinPanel(bool isToss = false)
	{
        ClaimBtn.gameObject.SetActive(false);
        nextRoundButtonWin.gameObject.SetActive(false);
		homeButtonWin.gameObject.SetActive(false);
		Debug.Log("isToss >>> " + isToss);
		middleWinImage.sprite = middleWinSprite[isToss ? 1 : 0];
		for (int i = 0; i < 3; i++)
		{
			playerOuterLineImage[i].gameObject.SetActive(!isToss);
			playerKeyImage[i].gameObject.SetActive(!isToss);
			aiOuterLineImage[i].gameObject.SetActive(!isToss);
			aiKeyImage[i].gameObject.SetActive(!isToss);
		}

		if (isToss)
		{
			finalWinMessageText.text = "IT'S TOUR TURN.";

			homeButtonWinText.text = "Next";
			homeButtonWin.onClick.RemoveAllListeners();
			homeButtonWin.onClick.AddListener(() =>
			{
				OpenCardPanelAfterToss();
			});
			homeButtonWin.gameObject.SetActive(true);
			winPanelAnimPar.transform.localScale = Vector3.one * .5f;
			winPanel.SetActive(true);
			winPanelAnimPar.transform.DOScale(Vector3.one, .5f);
			return;
		}



		for (int i = 0; i < roundWinDataList.Count; i++)
		{
			int player = roundWinDataList[i].playerRoundDetail;
			int ai = roundWinDataList[i].aiRoundDetail;

			playerOuterLineImage[i].sprite = outerLineSprite[player];
			playerKeyImage[i].sprite = keySprite[player];
			if (i == (roundWinDataList.Count - 1) && player == 1)
			{
				GameObject keyGen = Instantiate(winImageKey, playerOuterLineImage[i].transform);
				keyGen.transform.DOScale(Vector3.one, .2f).SetDelay(.1f).OnComplete(() =>
				{
					Destroy(keyGen);
				});
			}


			aiOuterLineImage[i].sprite = outerLineSprite[ai];
			aiKeyImage[i].sprite = keySprite[ai];
			if (i == (roundWinDataList.Count - 1) & ai == 1)
			{
				GameObject keyGen = Instantiate(winImageKey, aiOuterLineImageLose[i].transform);
				keyGen.transform.DOScale(Vector3.one, .2f).SetDelay(.1f).OnComplete(() =>
				{
					Destroy(keyGen);
				});
			}
		}

		nextRoundButtonWin.gameObject.SetActive(false);
		homeButtonWin.gameObject.SetActive(false);

		if (roundId != 3)
		{
			finalWinMessageText.text = "YOU WON THIS ROUND.";

			nextButtonWinText.text = "Next Round";
			nextRoundButtonWin.onClick.RemoveAllListeners();
			nextRoundButtonWin.onClick.AddListener(() =>
			{
				SFXManager.Instance.onButtonClick();
				NextRoundButton();
			});
			nextRoundButtonWin.gameObject.SetActive(true);
		}
		else
		{
			finalWinMessageText.text = "YOU WON THE GAME.";
			RoundCoinDescrease(true);
			ClaimBtn.gameObject.SetActive(true);
			nextButtonWinText.text = "Play Again";
			nextRoundButtonWin.onClick.RemoveAllListeners();
			nextRoundButtonWin.onClick.AddListener(() =>
			{
				SFXManager.Instance.onButtonClick();
				PlayAgainButton();
			});
			nextRoundButtonWin.gameObject.SetActive(true);

			homeButtonWinText.text = "Home";
			homeButtonWin.onClick.RemoveAllListeners();
			homeButtonWin.onClick.AddListener(() =>
			{
				OpenHomePanel();;
			});
			homeButtonWin.gameObject.SetActive(true);
		}

		winPanelAnimPar.transform.localScale = Vector3.one * .5f;
		winPanel.SetActive(true);
		winPanelAnimPar.transform.DOScale(Vector3.one, .5f);

		SFXManager.Instance.onWinLostSoundPlay(1);
	}

	private void OpenHomePanel()
	{
		SceneManager.LoadScene(0);
	}

	private void RoundCoinDescrease(bool isAdd = false)
	{
		if (isAdd)
		{
			Debug.Log("LocalStorageManager.Instance._RPSUserInfo.currentPoints = " + LocalStorageManager.Instance._RPSUserInfo.currentPoints + " | PlayerScore = " + playerScore);
			int pointSave = LocalStorageManager.Instance._RPSUserInfo.currentPoints + (playerScore > 0 ? playerScore : 0);
			LocalStorageManager.Instance._RPSUserInfo.currentPoints = pointSave;
			Debug.Log("LocalStorageManager.Instance._RPSUserInfo.currentPoints = " + LocalStorageManager.Instance._RPSUserInfo.currentPoints + " | PlayerScore = " + playerScore);
			CoinTextsPrint(LocalStorageManager.Instance._RPSUserInfo.currentPoints);
			RPSUserInfoSend infoSend = new RPSUserInfoSend(LocalStorageManager.Instance._RPSUserInfo.address, LocalStorageManager.Instance._RPSUserInfo.currentPoints, "Staking tokens");
			string jsonData = JsonUtility.ToJson(infoSend);
			Debug.Log(jsonData);
			LocalStorageManager.Instance.SendData(jsonData);
			return;
		}
		Debug.Log("LocalStorageManager.Instance._RPSUserInfo.currentPoints = " + LocalStorageManager.Instance._RPSUserInfo.currentPoints);
		LocalStorageManager.Instance._RPSUserInfo.currentPoints -= defaultScore;
		Debug.Log("LocalStorageManager.Instance._RPSUserInfo.currentPoints = " + LocalStorageManager.Instance._RPSUserInfo.currentPoints);
		CoinTextsPrint(LocalStorageManager.Instance._RPSUserInfo.currentPoints);

	}

	IEnumerator OpenWinLoseCountDown(bool isWin)
	{
		if (isWin)
			OpenWinPanel();
		else
			OpenLosePanel();

		yield return null;
	}


	public Sprite[] keyBG;
	public Sprite[] key;
	private void OpenLosePanel(bool isToss = false)
	{
        ClaimBtn2.gameObject.SetActive(false);
       
        homeButtonLose.gameObject.SetActive(false);
		nextRoundButtonLose.gameObject.SetActive(false);

		for (int i = 0; i < 3; i++)
		{
			playerOuterLineImageLose[i].gameObject.SetActive(!isToss);
			playerKeyImageLose[i].gameObject.SetActive(!isToss);

			aiOuterLineImageLose[i].gameObject.SetActive(!isToss);
			aiKeyImageLose[i].gameObject.SetActive(!isToss);
		}

		if (isToss)
		{
			finalLoseMessageText.text = "IT'S OPPENENT'S TURN.";

			homeButtonLoseText.text = "Next";
			homeButtonLose.onClick.RemoveAllListeners();
			homeButtonLose.onClick.AddListener(() =>
			{
				OpenCardPanelAfterToss();
			});
			homeButtonLose.gameObject.SetActive(true);

			losePanelAnimPar.transform.localScale = Vector3.one * .5f;
			losePanel.SetActive(true);
			losePanelAnimPar.transform.DOScale(Vector3.one, .5f);
			return;
		}


		for (int i = 0; i < roundWinDataList.Count; i++)
		{
			int player = roundWinDataList[i].playerRoundDetail;
			int ai = roundWinDataList[i].aiRoundDetail;

			playerOuterLineImageLose[i].sprite = keyBG[player];
			playerKeyImageLose[i].sprite = key[player];
			if (i == (roundWinDataList.Count - 1) && player == 1)
			{
				GameObject keyGen = Instantiate(winImageKey, playerOuterLineImage[i].transform);
				keyGen.transform.DOScale(Vector3.one, .2f).SetDelay(.1f).OnComplete(() =>
				{
					Destroy(keyGen);
				});
			}


			aiOuterLineImageLose[i].sprite = keyBG[ai];
			aiKeyImageLose[i].sprite = key[ai];
			if (i == (roundWinDataList.Count - 1) & ai == 1)
			{
				GameObject keyGen = Instantiate(winImageKey, aiOuterLineImageLose[i].transform);
				keyGen.transform.DOScale(Vector3.one, .2f).SetDelay(.1f).OnComplete(() =>
				{
					Destroy(keyGen);
				});
			}
		}

		if (roundId != 3)
		{
			finalLoseMessageText.text = "YOU LOSE THIS ROUND.";

			nextButtonLoseText.text = "Next Round";
			nextRoundButtonLose.onClick.RemoveAllListeners();
			nextRoundButtonLose.onClick.AddListener(() =>
			{
				SFXManager.Instance.onButtonClick();
				NextRoundButton();
			});
			nextRoundButtonLose.gameObject.SetActive(true);
		}
		else
		{
            
            ClaimBtn2.gameObject.SetActive(true);
            finalLoseMessageText.text = "YOU LOSE THE GAME.";
			RoundCoinDescrease(true);
			nextButtonLoseText.text = "Play Again";
			nextRoundButtonLose.onClick.RemoveAllListeners();
			nextRoundButtonLose.onClick.AddListener(() =>
			{
				SFXManager.Instance.onButtonClick();
				PlayAgainButton();
			});
			nextRoundButtonLose.gameObject.SetActive(true);

			homeButtonLoseText.text = "Home";
			homeButtonLose.onClick.RemoveAllListeners();
			homeButtonLose.onClick.AddListener(() =>
			{
				OpenHomePanel();;
			});
			homeButtonLose.gameObject.SetActive(true);
		}

		losePanelAnimPar.transform.localScale = Vector3.one * .5f;
		losePanel.SetActive(true);
		losePanelAnimPar.transform.DOScale(Vector3.one, .5f);
		SFXManager.Instance.onWinLostSoundPlay(0);
	}

	public void OnClaimBtnClick()
	{
		Application.OpenURL(url);
	}
	private void NextRoundButton()
	{
		winPanel.SetActive(false);
		losePanel.SetActive(false);
		roundId++;
		PrintUpperRoundText();
		ResetRound();
		//StartCoroutine(StartGame());
		OpenCardPanelAfterToss();
		//ShowResult();
	}

	private void PlayAgainButton()
	{
		PlayerPrefs.SetInt("DirectPlay", 1);
		roundWinDataList.Clear();

		roundId = 1;
		playerRoundWins = aiRoundWins = 0;
		PrintUpperRoundText();
		winPanel.SetActive(false);
		losePanel.SetActive(false);
		ResetWinLostPanel();
		ResetRound();
		ClearGamePlayButton();
		//GameStart();
		OpenHomePanel();
		//StartCoroutine(StartGame());
	}

	private void ResetWinLostPanel()
	{
		for (int i = 0; i < 3; i++)
		{
			playerOuterLineImageLose[i].sprite = keyBG[0];
			playerKeyImageLose[i].sprite = key[0];
			aiOuterLineImageLose[i].sprite = keyBG[0];
			aiKeyImageLose[i].sprite = key[0];

			playerOuterLineImage[i].sprite = keyBG[0];
			playerKeyImage[i].sprite = key[0];
			aiOuterLineImage[i].sprite = keyBG[0];
			aiKeyImage[i].sprite = key[0];
		}
	}

	private bool CheckDraw()
	{
		Equation equation;

		int playerAmount = 0;
		int aiAmount = 0;

		for (int i = 0; i < allCardScript.Count; i++)
		{
			if (i % 2 == 0)
			{
				int amount = allCardScript[i].GetCardValue();

				equation = allCardScript[i].GetEquation();

				aiAmount = equation == Equation.Plus ? aiAmount + amount : equation == Equation.Minus ? aiAmount - amount : equation == Equation.Multiplication ? aiAmount * amount : equation == Equation.Division ? aiAmount / amount : 50;
			}
			else
			{
				int amount = allCardScript[i].GetCardValue();

				equation = allCardScript[i].GetEquation();

				playerAmount = equation == Equation.Plus ? playerAmount + amount : equation == Equation.Minus ? playerAmount - amount : equation == Equation.Multiplication ? playerAmount * amount : equation == Equation.Division ? playerAmount / amount : 50;
			}
		}

		return false;

		//if (playerAmount != aiAmount) {
		//    return false;
		//}
		//else {
		//    return true;
		//}
	}


	private void OnClickCalculate(int n)
	{
		Equation equation = Equation.Minus;
		RectTransform cardAnimRef;
		int cardAmount = 0;

		if (n == 0)
		{
			float ai = equation == Equation.Plus ? aiScore + cardAmount :
							  equation == Equation.Minus ? aiScore - cardAmount :
							  equation == Equation.Multiplication ? aiScore * cardAmount :
							  equation == Equation.Division ? aiScore / cardAmount :
							  equation == Equation.PlusPer ? aiScore + ((aiScore * cardAmount) / 100) :
							  equation == Equation.Steal ? ((aiScore * cardAmount) / 100) : 50;
			aiScore = (int)ai;
			cardAnimRef = cardAiRef;
		}
		else
		{
			float player = equation == Equation.Plus ? playerScore + cardAmount :
							  equation == Equation.Minus ? playerScore - cardAmount :
							  equation == Equation.Multiplication ? playerScore * cardAmount :
							  equation == Equation.Division ? playerScore / cardAmount :
							  equation == Equation.PlusPer ? playerScore + ((playerScore * cardAmount) / 100) :
							  equation == Equation.Steal ? ((playerScore * cardAmount) / 100) : 50;
			playerScore = (int)player;
			cardAnimRef = cardPlayerRef;
		}
	}

	public void CoinTextsPrint(int amount)
	{
		Debug.Log("Print Coin Amount = " + amount);
		coinTexts.ForEach(t => t.text = $"{amount}");
	}
}

//[System.Serializable]
//public class GameEquation
//{
//    public Equation _Equation;
//    public int _Value;
//    public Sprite _CardSprite;
//}
[System.Serializable]
public class GameCardEquation
{
	public Equation _CardEquation;
	public int _CardValue;
	public Sprite _CardSprite;
}

#region Class 

[System.Serializable]
public class RoundWinData
{
	public int playerRoundDetail;
	public int aiRoundDetail;
}
#endregion