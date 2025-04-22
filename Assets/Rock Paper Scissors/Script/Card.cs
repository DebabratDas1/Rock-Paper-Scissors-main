using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class Card : MonoBehaviour
{
    private int cardValue;

    [SerializeField] private Equation equation;

    [SerializeField] private TextMeshProUGUI cardText;
    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private Button cardButton;

    /*[SerializeField] */
    private Sprite cardValueSprite;

    public void OnEnable() {
        //SetValue();
        cardText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        cardButton.image.sprite = GameManager.Instance.cardDefaultImage;
    }

    private void onOpenText() {
        cardText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
        cardButton.image.sprite = cardValueSprite;
    }

    public void Interactable(bool isTrue) {
        cardButton.interactable = isTrue;
    }

    public void SetValue(int index) {
        Debug.Log("INdsex >> |||| " + index);
        GameCardEquation game = GameManager.Instance.gameCardEquations[index];
        equation = game._CardEquation;
        cardValue = game._CardValue;
        cardValueSprite = game._CardSprite;

        SetCardValue(cardValue, equation);
    }


    public void FlipCard() {
        transform.DORotate(new Vector3(0, 90, 0), 0.15f)
                 .OnComplete(() =>
                 {
                     onOpenText();
                     transform.DORotate(new Vector3(0, 0, 0), 0.15f);
                 });
    }

    public void MoveAnimation(RectTransform cardSendRef, GameObject moveCard) {
        moveCard.transform.DOMove(cardSendRef.position, .8f);
        moveCard.transform.DOScale(Vector3.one * .3f, .8f);
    }

    public void SetCardValue(int value, Equation equation) {


        string equationSymbol = equation switch
        {
            Equation.Plus => $"+ {cardValue}",
            Equation.Minus => $"- {cardValue}",
            Equation.Multiplication => $"× {cardValue}",
            Equation.Division => $"÷ {cardValue}",
            Equation.PlusPer => $"+ {cardValue}%",
            Equation.Steal => $"Steal\n{cardValue}%"
        };
        cardText.text = $"{equationSymbol}"; // Card UI update
    }

    public int GetCardValue() {
        return cardValue;
    }

    public Equation GetEquation() {
        return equation;
    }
}
