using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Video;

public class ShopCard : MonoBehaviour
{
    private enum State
    {
        NotPurchased, Purchased
    }

    private State state;
    private Card card;

    [Header("Name Panel")]
    [SerializeField] private string unitName;
    [SerializeField] private Button infoPanelButton;
    [SerializeField] private string infoPanelText;
    [SerializeField] private VideoClip infoPanelVideo;

    [Header("Purchased")]
    [SerializeField] private TMP_Text purResCostText;
    [SerializeField] private Image purchasedImage;
    [SerializeField] private Button toDeckButton;
    [SerializeField] private Color32 notInDeckColor;
    [SerializeField] private Color32 inDeckColor;
    [SerializeField] private TMP_Text deckButtonText;
    [SerializeField] private Image resCircleColor;

    [Header("Not Purchased")]
    [SerializeField] private GameObject notPurchasedCard;
    [SerializeField] private TMP_Text notPurResCostText;
    [SerializeField] private TMP_Text purchaseCostText;
    [SerializeField] private Image notPurchasedImage;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private float cardCost;
    [SerializeField] private AudioClip buySound;

    [Header("Card Prefab")]
    [SerializeField] private GameObject cardPrefab;
    public string cardID;
    void Start()
    {
        card = cardPrefab.GetComponent<Card>();
        resCircleColor.color = card.costCircle.color;
        purchasedImage.sprite = card.mainImage.sprite;
        notPurchasedImage.sprite = card.insufFundsImage.sprite;
        infoPanelButton.onClick.AddListener(OpenInfoPanel);
        if (cardCost > MultiScene.multiScene.money)
        {
            purchaseButton.interactable = false;
            purchaseButton.image.color = new Color32(85, 85, 85, 255);
        }
        if (MultiScene.multiScene.purchasedCards.Contains(cardPrefab) || MultiScene.multiScene.cardIDs.Contains(cardID)) PurchasedState();
        else NotPurchasedState();
    }
    public void NotPurchasedState()
    {
        state = State.NotPurchased;
        transform.localScale = Vector3.one;
        purResCostText.text = card.cost.ToString();
        notPurResCostText.text = card.cost.ToString();
        purchaseCostText.text = cardCost.ToString();
        notPurchasedCard.SetActive(true);
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(PurchaseCard);
    }
    void PurchasedState()
    {
        if (MultiScene.multiScene.purchasedCards.Contains(cardPrefab) == false) MultiScene.multiScene.purchasedCards.Add(cardPrefab);
        if (MultiScene.multiScene.deckCards.Contains(cardID) && MultiScene.multiScene.cardsOnDeck.Contains(cardPrefab) == false) MultiScene.multiScene.cardsOnDeck.Add(cardPrefab);

        if (MultiScene.multiScene.cardsOnDeck.Contains(cardPrefab))
        {
            toDeckButton.image.color = inDeckColor;
            deckButtonText.text = "Remove";
        }
        else
        {
            toDeckButton.image.color = notInDeckColor;
            deckButtonText.text = "Add To Deck";
        }
        purResCostText.text = card.cost.ToString();
        toDeckButton.onClick.RemoveAllListeners();
        toDeckButton.onClick.AddListener(CardOnOffDeck);
        notPurchasedCard.SetActive(false);
        state = State.Purchased;
        if (MultiScene.multiScene.cardsOnDeck.Contains(cardPrefab) && transform.localScale == Vector3.one) transform.DOScale(Vector3.one * 1.05f, .25f);
        else transform.localScale = Vector3.one;
    }
    public void PurchaseCard()
    {
        if (MultiScene.multiScene.money >= cardCost)
        {
            CardPop();
            MultiScene.multiScene.money -= cardCost;
            SoundManager.Instance.PlayUISound(buySound);
            SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.MoneyGained));
            gamemanager.userInterface.deckTabMoneyText.text = MultiScene.multiScene.money.ToString();
            gamemanager.userInterface.deckTabMoneyText.rectTransform.DOPunchScale(Vector3.one * -0.3f, 0.25f, 5, 1f);           
            MultiScene.multiScene.cardIDs += cardID;
            PurchasedState();
        }
        else CantBuyCard();
    }
    public void CardOnOffDeck()
    {
        CardPop();
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.CardSelected));

        if (MultiScene.multiScene.cardsOnDeck.Contains(cardPrefab))
        {
            transform.DOScale(Vector3.one, .25f).SetEase(Ease.InSine);
            MultiScene.multiScene.cardsOnDeck.Remove(cardPrefab);
            MultiScene.multiScene.deckCards = MultiScene.multiScene.deckCards.Replace(cardID, "");
            toDeckButton.image.color = notInDeckColor;
            deckButtonText.text = "Add To Deck";
        }
        else
        {
            SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.CardSelected));
            transform.DOScale(Vector3.one * 1.05f, .25f);
            MultiScene.multiScene.cardsOnDeck.Add(cardPrefab);
            MultiScene.multiScene.deckCards += cardID;
            toDeckButton.image.color = inDeckColor;
            deckButtonText.text = "Remove";
        }
    }
    public void OpenInfoPanel()
    {
        CardPop();
        gamemanager.userInterface.infoPanel.InfoPanelOn(unitName, infoPanelText, infoPanelVideo);
    }
    void CardPop()
    {
        gameObject.transform.DOPunchScale(transform.localScale * .1f, .15f, 5, 0.1f);
    }
    void CantBuyCard()
    {
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.InsufficientFunds));
        // tänne myöhemmin jotain juicea
    }
}
