using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class olduimanager : MonoBehaviour
{
    private enum MainMenuTabs
    {
        MainTab, DeckTab
    }
    private enum State
    {
        MainMenu, Play
    }

    private State state;
    private MainMenuTabs mainMenuTabs;

    [Header("Menus")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject runTimeUi;
    [SerializeField] private GameObject audioMenu;

    [Header("Main Menu")]
    [SerializeField] private RectTransform mainTab;
    [SerializeField] private RectTransform deckTab;
    [SerializeField] private Button sleepyButton;
    [SerializeField] private Button mightyButton;
    [SerializeField] private Button legendaryButton;
    [SerializeField] private TMP_Text mainMenuHighScoreText;
    public TMP_Text deckTabMoneyText;

    [Header("Score")]
    public TMP_Text timerText;
    [SerializeField] private TMP_Text currentRunScore;
    [SerializeField] private TMP_Text newHighScoreText;

    [Header("Money")]
    [SerializeField] private TMP_Text moneyText;
    private Vector3 originalMoneyTextScale;

    [Header("Resources")]
    public Slider resourceSlider;
    [SerializeField] private float refreshCooldown;
    [SerializeField] private float resourcesPerS;
    [SerializeField] private int startResources;
    [SerializeField] private TMP_Text resourceNumber;
    [SerializeField] private GameObject resourceCircle;
    [SerializeField] private GameObject resourceBar;
    [HideInInspector] public float currentResources;

    [Header("Other")]
    [SerializeField] private RectTransform startText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private Image difficultyPanel;

    [Header("Card Places")]
    [SerializeField] private Button oneRefreshButton;
    [SerializeField] private Image oneRefButtonCD;
    [SerializeField] private Transform oneCardPlace;

    [SerializeField] private Button twoRefreshButton;
    [SerializeField] private Image twoRefButtonCD;
    [SerializeField] private Transform twoCardPlace;

    [SerializeField] private Button threeRefreshButton;
    [SerializeField] private Image threeRefButtonCD;
    [SerializeField] private Transform threeCardPlace;

    [SerializeField] private Button fourRefreshButton;
    [SerializeField] private Image fourRefButtonCD;
    [SerializeField] private Transform fourCardPlace;

    [Header("Cards")]
    [SerializeField] private GameObject[] emptyDeckCards;
    [SerializeField] private GameObject[] cards;

    private float timer1, timer2, timer3, timer4;

    private GameObject card1, card2, card3, card4;

    private int prevCard1, prevCard2, prevCard3, prevCard4;

    [HideInInspector] public float currentTime;
    [HideInInspector] public bool isTiming;

    [HideInInspector] public Animator anim;
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            state = State.Play;

            isTiming = true;
            currentTime = 0;

            //if (MultiScene.multiScene.cardsOnDeck.Count == 0)

            resourceSlider.maxValue = 10;
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            resourceSlider.value = startResources;
            currentResources = resourceSlider.value;
            resourceNumber.text = resourceSlider.value.ToString("0");
            moneyText.text = MultiScene.multiScene.money.ToString();
            originalMoneyTextScale = moneyText.rectTransform.localScale;
            SoundManager.Instance.PlayMusicSound(gamemanager.assetBank.FindSound(AssetBank.Sound.GameStartedJingle));

            if (MultiScene.multiScene.difficulty == 0)
            {
                difficultyText.text = "Sleepy Goblin";
                difficultyPanel.color = Color.green;
            }
            else if (MultiScene.multiScene.difficulty == 1)
            {
                difficultyText.text = "Mighty Goblin";
                difficultyPanel.color = Color.yellow;
            }
            else if (MultiScene.multiScene.difficulty == 2)
            {
                difficultyText.text = "Legendary Goblin";
                difficultyPanel.color = Color.red;
            }
            StartCoroutine("StartCards");
            StartCoroutine("StartText");

            anim = GetComponent<Animator>();
        }
        else MainMenuState();
    }
    void Update()
    {
        switch (state)
        {
            default:
            case State.Play:
                if (resourceSlider.value <= 10)
                {
                    resourceSlider.value += Time.deltaTime * resourcesPerS;
                }

                Timers();

                if (isTiming == true)
                {
                    currentTime += Time.deltaTime;
                    SetTimerText();
                }
                if (resourceSlider.value >= currentResources + 1)
                {
                    currentResources = Mathf.Floor(resourceSlider.value);
                    resourceNumber.text = currentResources.ToString("0");
                    ResourceCirclePop();
                }
                else if (resourceSlider.value <= currentResources)
                {
                    currentResources = Mathf.Floor(resourceSlider.value);
                    resourceNumber.text = currentResources.ToString("0");
                }
                break;
            case State.MainMenu:
                break;
        }
    }
    void MainMenuState()
    {
        state = State.MainMenu;
        mainMenuTabs = MainMenuTabs.MainTab;
        deckTabMoneyText.text = MultiScene.multiScene.money.ToString();
        mainMenuHighScoreText.text = "High Score: " + System.TimeSpan.FromSeconds(MultiScene.multiScene.highScore).ToString("mm\\:ss\\.f");
        if (MultiScene.multiScene.difficulty == 0) sleepyButton.Select();
        else if (MultiScene.multiScene.difficulty == 1) mightyButton.Select();
        else if (MultiScene.multiScene.difficulty == 2) legendaryButton.Select();
    }
    void SetTimerText()
    {
        timerText.text = System.TimeSpan.FromSeconds(currentTime).ToString("mm\\:ss\\.f");
    }

    public void AudioMenuOnOff()
    {
        if (audioMenu.activeSelf == false) audioMenu.SetActive(true);
        else audioMenu.SetActive(false);
        ButtonClickAudio();
    }
    public void PauseMenuOnOff()
    {
        if (pauseMenu.activeSelf == false) pauseMenu.SetActive(true);
        else pauseMenu.SetActive(false);
        ButtonClickAudio();
    }
    public void GameOverMenu()
    {
        isTiming = false;
        currentRunScore.text = "Your time: " + System.TimeSpan.FromSeconds(currentTime).ToString("mm\\:ss\\.f");
        if (MultiScene.multiScene.highScore < gamemanager.userInterface.timeBtwScoreIncrease) MultiScene.multiScene.highScore = gamemanager.userInterface.timeBtwScoreIncrease;
        if (currentTime == MultiScene.multiScene.highScore) newHighScoreText.enabled = true;
        gameOverMenu.SetActive(true);
    }
    public void DisableRunTimeUI()
    {
        runTimeUi.SetActive(false);
    }
    public void RefreshOne()
    {
        if (gamemanager.playercards.selectedCard == card1) gamemanager.playercards.ResetSelectedCard();
        Destroy(card1);
        anim.SetInteger("CardSelected", 0);
        oneRefreshButton.enabled = false;
        int randomCard = Random.Range(0, cards.Length);
        while (randomCard == prevCard1) randomCard = Random.Range(0, cards.Length);
        GameObject newCard = Instantiate(cards[randomCard], oneCardPlace.position, Quaternion.identity);
        newCard.transform.SetParent(oneCardPlace.gameObject.transform);
        oneCardPlace.transform.localScale = Vector3.zero;
        oneCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card1 = newCard;
        prevCard1 = randomCard;
        timer1 = refreshCooldown;
        SpawnCardAudio();
    }
    public void RefreshTwo()
    {
        if (gamemanager.playercards.selectedCard == card2) gamemanager.playercards.ResetSelectedCard();
        Destroy(card2);
        anim.SetInteger("CardSelected", 0);
        twoRefreshButton.enabled = false;
        int randomCard = Random.Range(0, cards.Length);
        while (randomCard == prevCard2) randomCard = Random.Range(0, cards.Length);
        GameObject newCard = Instantiate(cards[randomCard], twoCardPlace.position, Quaternion.identity);
        newCard.transform.SetParent(twoCardPlace.gameObject.transform);
        twoCardPlace.transform.localScale = Vector3.zero;
        twoCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card2 = newCard;
        prevCard2 = randomCard;
        timer2 = refreshCooldown;
        SpawnCardAudio();
    }
    public void RefreshThree()
    {
        if (gamemanager.playercards.selectedCard == card3) gamemanager.playercards.ResetSelectedCard();
        Destroy(card3);
        anim.SetInteger("CardSelected", 0);
        threeRefreshButton.enabled = false;
        int randomCard = Random.Range(0, cards.Length);
        while (randomCard == prevCard3) randomCard = Random.Range(0, cards.Length);
        GameObject newCard = Instantiate(cards[randomCard], threeCardPlace.position, Quaternion.identity);
        newCard.transform.SetParent(threeCardPlace.gameObject.transform);
        threeCardPlace.transform.localScale = Vector3.zero;
        threeCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card3 = newCard;
        prevCard3 = randomCard;
        timer3 = refreshCooldown;
        SpawnCardAudio();
    }
    public void RefreshFour()
    {
        if (gamemanager.playercards.selectedCard == card4) gamemanager.playercards.ResetSelectedCard();
        Destroy(card4);
        anim.SetInteger("CardSelected", 0);
        fourRefreshButton.enabled = false;
        int randomCard = Random.Range(0, cards.Length);
        while (randomCard == prevCard4) randomCard = Random.Range(0, cards.Length);
        GameObject newCard = Instantiate(cards[randomCard], fourCardPlace.position, Quaternion.identity);
        newCard.transform.SetParent(fourCardPlace.gameObject.transform);
        fourCardPlace.transform.localScale = Vector3.zero;
        fourCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card4 = newCard;
        prevCard4 = randomCard;
        timer4 = refreshCooldown;
        SpawnCardAudio();
    }
    IEnumerator StartCards()
    {
        SpawnCardOne();
        yield return new WaitForSeconds(.3f);
        SpawnCardTwo();
        yield return new WaitForSeconds(.3f);
        SpawnCardThree();
        yield return new WaitForSeconds(.3f);
        SpawnCardFour();
    }
    public void SpawnCardOne()
    {
        int randomCard = Random.Range(0, cards.Length);
        GameObject newCard1 = Instantiate(cards[randomCard], oneCardPlace.position, Quaternion.identity);
        newCard1.transform.SetParent(oneCardPlace.gameObject.transform);
        oneCardPlace.transform.localScale = Vector3.zero;
        oneCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card1 = newCard1;
        prevCard1 = randomCard;
        SpawnCardAudio();
    }
    public void SpawnCardTwo()
    {
        int randomCard = Random.Range(0, cards.Length);
        GameObject newCard2 = Instantiate(cards[randomCard], twoCardPlace.position, Quaternion.identity);
        newCard2.transform.SetParent(twoCardPlace.gameObject.transform);
        twoCardPlace.transform.localScale = Vector3.zero;
        twoCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card2 = newCard2;
        prevCard2 = randomCard;
        SpawnCardAudio();
    }
    public void SpawnCardThree()
    {
        int randomCard = Random.Range(0, cards.Length);
        GameObject newCard3 = Instantiate(cards[randomCard], threeCardPlace.position, Quaternion.identity);
        newCard3.transform.SetParent(threeCardPlace.gameObject.transform);
        threeCardPlace.transform.localScale = Vector3.zero;
        threeCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card3 = newCard3;
        prevCard3 = randomCard;
        SpawnCardAudio();
    }
    public void SpawnCardFour()
    {
        int randomCard = Random.Range(0, cards.Length);
        GameObject newCard4 = Instantiate(cards[randomCard], fourCardPlace.position, Quaternion.identity);
        newCard4.transform.SetParent(fourCardPlace.gameObject.transform);
        fourCardPlace.transform.localScale = Vector3.zero;
        fourCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card4 = newCard4;
        prevCard4 = randomCard;
        SpawnCardAudio();
    }
    void Timers()
    {
        if (timer1 >= 0)
        {
            timer1 -= Time.deltaTime;
            oneRefButtonCD.fillAmount = timer1 / refreshCooldown;
        }
        if (timer1 <= 0) oneRefreshButton.enabled = true;

        if (timer2 >= 0)
        {
            timer2 -= Time.deltaTime;
            twoRefButtonCD.fillAmount = timer2 / refreshCooldown;
        }
        if (timer2 <= 0) twoRefreshButton.enabled = true;

        if (timer3 >= 0)
        {
            timer3 -= Time.deltaTime;
            threeRefButtonCD.fillAmount = timer3 / refreshCooldown;
        }
        if (timer3 <= 0) threeRefreshButton.enabled = true;

        if (timer4 >= 0)
        {
            timer4 -= Time.deltaTime;
            fourRefButtonCD.fillAmount = timer4 / refreshCooldown;
        }
        if (timer4 <= 0) fourRefreshButton.enabled = true;
    }
    void ResourceCirclePop()
    {
        resourceCircle.transform.DOPunchScale(Vector3.one * 0.4f, 0.35f, 5, 1f);
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.ResourcePlusOne));
    }
    public void InsufficientResourcesShake()
    {
        resourceBar.transform.DOShakePosition(.3f, Vector3.right * 15, 10, 0, false, false);
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.InsufficientFunds));
    }

    IEnumerator StartText()
    {
        startText.transform.localPosition = new Vector3(-900f, startText.transform.localPosition.y, startText.transform.localPosition.z);
        startText.DOAnchorPosX(-45, .5f, false).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(.5f);
        startText.DOAnchorPosX(5, 1.5f);
        yield return new WaitForSeconds(1.4f);
        startText.DOAnchorPosX(900, .5f, false).SetEase(Ease.InCubic);
    }
    public void UpdateMoneyText()
    {
        moneyText.text = MultiScene.multiScene.money.ToString();
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.MoneyGained));
        StartCoroutine(MoneyTextPop());
    }
    IEnumerator MoneyTextPop()
    {
        moneyText.rectTransform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 5, 1f);
        yield return new WaitForSeconds(.25f);
        if (moneyText.rectTransform.localScale != originalMoneyTextScale) moneyText.rectTransform.DOScale(originalMoneyTextScale, 0.2f).SetEase(Ease.OutSine);
    }
    public void CardSelectedAudio()
    {
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.CardSelected));
        Debug.Log("soundplayed");
    }
    void SpawnCardAudio()
    {
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.NewCardDrawn));
    }
    public void ButtonClickAudio()
    {
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.ButtonClicked));
    }
    public void DeckTabOnOff()
    {
        if (mainMenuTabs == MainMenuTabs.MainTab)
        {
            mainTab.DOLocalMoveX(-1080, .5f, true);
            deckTab.DOLocalMoveX(0, .5f, true);
            mainMenuTabs = MainMenuTabs.DeckTab;
        }
        else
        {
            mainTab.DOLocalMoveX(0, .5f, true);
            deckTab.DOLocalMoveX(1080, .5f, true);
            mainMenuTabs = MainMenuTabs.MainTab;
        }
    }
}
