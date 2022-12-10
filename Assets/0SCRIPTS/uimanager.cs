using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class uimanager : MonoBehaviour
{
    private enum MainMenuTabs
    {
        MainTab, DeckTab, LeaderboardTab
    }
    private enum State
    {
        MainMenu, Play, RunTimeCutscene
    }

    private State state;
    private MainMenuTabs mainMenuTabs;

    [Header("Menus")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject runTimeUi;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private RectTransform runTimeDownPanel;
    [SerializeField] private RectTransform runTimeUpPanel;

    [Header("Main Menu")]
    [SerializeField] private RectTransform mainTab;
    [SerializeField] private RectTransform deckTab;
    [SerializeField] private RectTransform leaderboardTab;
    public GameObject[] leaderboardPlates;
    public GameObject[] difficultyButtons;
    [SerializeField] private Button sleepyButton;   // turhia?
    [SerializeField] private Button mightyButton;   // turhia?
    [SerializeField] private Button legendaryButton;   // turhia?
    public TMP_Text mainMenuHighScoreText;
    public TMP_Text deckTabMoneyText;
    [SerializeField] private Image cannotPlayPanel;
    public InfoPanel infoPanel;
    [SerializeField] private Animator sceneFaderAnim;
    public GameObject[] deckTabCards;
    [SerializeField] private Button resetProgressButton;

    [Header("Score")]
    public TMP_Text scoreText;
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
    [SerializeField] private GameObject runTimeMenuFade;

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
    [SerializeField] private List<GameObject> cardsOnDeck;

    private float timer1, timer2, timer3, timer4;

    private GameObject card1, card2, card3, card4;

    private int prevCard1, prevCard2, prevCard3, prevCard4;

    [HideInInspector] public float timeBtwScoreIncrease;
    [HideInInspector] public bool isTiming;

    [HideInInspector] public Animator anim;
    [HideInInspector] public float score;
    private float scoreInterval = 2;
    private Vector3 originalScoreTextScale;

    private bool newHighScoreAchieved;
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            InitializeRuntime();
        }
        else MainMenuState();
    }
    void InitializeRuntime()
    {
        state = State.RunTimeCutscene;

        if (MultiScene.multiScene.cardsOnDeck.Count == 0) cardsOnDeck.AddRange(MultiScene.multiScene.purchasedCards);
        else cardsOnDeck.AddRange(MultiScene.multiScene.cardsOnDeck);

        anim = GetComponent<Animator>();
        resourceSlider.maxValue = 10;
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        resourceSlider.value = startResources;
        currentResources = resourceSlider.value;
        resourceNumber.text = resourceSlider.value.ToString("0");
        moneyText.text = MultiScene.multiScene.money.ToString();
        originalMoneyTextScale = moneyText.rectTransform.localScale;
        originalScoreTextScale = scoreText.rectTransform.localScale;
        SoundManager.Instance.PlayMusicSound(gamemanager.assetBank.FindSound(AssetBank.Sound.GameStartedJingle));
        runTimeUpPanel.anchoredPosition = new Vector3(0, 690, 0);
        runTimeDownPanel.anchoredPosition = new Vector3(0, -240, 0);

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
        StartCoroutine("StartText");
        Invoke("StartPlayState", 2f);
    }
    public void StartPlayState()
    {
        state = State.Play;

        isTiming = true;
        timeBtwScoreIncrease = 0;
        score = 0;

        StartCoroutine("StartRunTimeUI");
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

                if (isTiming == true && gamemanager.enemyManager.stage != 5)
                {                   
                    if (timeBtwScoreIncrease >= scoreInterval)
                    {
                        timeBtwScoreIncrease = 0;
                        score += 5;
                        UpdateScoreText();
                    }
                    else timeBtwScoreIncrease += Time.deltaTime;
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
                if (timeBtwScoreIncrease > MultiScene.multiScene.highScore && newHighScoreAchieved == false)
                {
                    newHighScoreAchieved = true;
                    NewHighScore();
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
        mainMenuHighScoreText.text = "High Score: " + MultiScene.multiScene.highScore.ToString();
        resetProgressButton.onClick.AddListener(MultiScene.multiScene.ResetSave);
    }
    public void UpdateScoreText()
    {
        scoreText.text = "SCORE: " + score.ToString();
        StartCoroutine("ScoreTextPop");
    }
    IEnumerator ScoreTextPop()
    {
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 5, 1f);
        yield return new WaitForSeconds(.25f);
        if (scoreText.rectTransform.localScale != originalScoreTextScale) scoreText.rectTransform.DOScale(originalScoreTextScale, 0.2f).SetEase(Ease.OutSine);
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
        currentRunScore.text = "Your score: " + score.ToString();
        if (MultiScene.multiScene.highScore < score) MultiScene.multiScene.highScore = score;
        if (score == MultiScene.multiScene.highScore) newHighScoreText.enabled = true;
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
        int randomCard = Random.Range(0, cardsOnDeck.Count);
        if (cardsOnDeck.Count == 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        else while (randomCard == prevCard1) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard = Instantiate(cardsOnDeck[randomCard], oneCardPlace.position, Quaternion.identity);
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
        int randomCard = Random.Range(0, cardsOnDeck.Count);
        if (cardsOnDeck.Count == 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        else while (randomCard == prevCard2) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard = Instantiate(cardsOnDeck[randomCard], twoCardPlace.position, Quaternion.identity);
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
        int randomCard = Random.Range(0, cardsOnDeck.Count);
        if (cardsOnDeck.Count == 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        else while (randomCard == prevCard3) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard = Instantiate(cardsOnDeck[randomCard], threeCardPlace.position, Quaternion.identity);
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
        int randomCard = Random.Range(0, cardsOnDeck.Count);
        if (cardsOnDeck.Count == 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        else while (randomCard == prevCard4) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard = Instantiate(cardsOnDeck[randomCard], fourCardPlace.position, Quaternion.identity);
        newCard.transform.SetParent(fourCardPlace.gameObject.transform);
        fourCardPlace.transform.localScale = Vector3.zero;
        fourCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card4 = newCard;
        prevCard4 = randomCard;
        timer4 = refreshCooldown;
        SpawnCardAudio();
    }
    IEnumerator StartRunTimeUI()
    {
        runTimeUpPanel.DOAnchorPosY(-910, .5f).SetEase(Ease.OutSine);
        runTimeDownPanel.DOAnchorPosY(0, .5f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(.3f);
        SpawnCardOne();
        yield return new WaitForSeconds(.2f);
        SpawnCardTwo();
        yield return new WaitForSeconds(.2f);
        SpawnCardThree();
        yield return new WaitForSeconds(.2f);
        SpawnCardFour();
    }
    public void SpawnCardOne()
    {
        int randomCard = 0;
        if (cardsOnDeck.Count != 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard1 = Instantiate(cardsOnDeck[randomCard], oneCardPlace.position, Quaternion.identity);
        newCard1.transform.SetParent(oneCardPlace.gameObject.transform);
        oneCardPlace.transform.localScale = Vector3.zero;
        oneCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card1 = newCard1;
        prevCard1 = randomCard;
        SpawnCardAudio();
    }
    public void SpawnCardTwo()
    {
        int randomCard = 0;
        if (cardsOnDeck.Count != 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard2 = Instantiate(cardsOnDeck[randomCard], twoCardPlace.position, Quaternion.identity);
        newCard2.transform.SetParent(twoCardPlace.gameObject.transform);
        twoCardPlace.transform.localScale = Vector3.zero;
        twoCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card2 = newCard2;
        prevCard2 = randomCard;
        SpawnCardAudio();
    }
    public void SpawnCardThree()
    {
        int randomCard = 0;
        if (cardsOnDeck.Count != 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard3 = Instantiate(cardsOnDeck[randomCard], threeCardPlace.position, Quaternion.identity);
        newCard3.transform.SetParent(threeCardPlace.gameObject.transform);
        threeCardPlace.transform.localScale = Vector3.zero;
        threeCardPlace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        card3 = newCard3;
        prevCard3 = randomCard;
        SpawnCardAudio();
    }
    public void SpawnCardFour()
    {
        int randomCard = 0;
        if (cardsOnDeck.Count != 1) randomCard = Random.Range(0, cardsOnDeck.Count);
        GameObject newCard4 = Instantiate(cardsOnDeck[randomCard], fourCardPlace.position, Quaternion.identity);
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

    IEnumerator StartText()
    {
        startText.transform.localPosition = new Vector3(-900f, startText.transform.localPosition.y, startText.transform.localPosition.z);
        yield return new WaitForSeconds(.1f);
        startText.DOAnchorPosX(-45, .5f, false).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(.5f);
        startText.DOAnchorPosX(5, 1.5f);
        yield return new WaitForSeconds(1.3f);
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
        ButtonClickAudio();
        if (mainMenuTabs == MainMenuTabs.MainTab)
        {
            mainTab.DOLocalMoveX(-1080, .5f, true);
            deckTab.DOLocalMoveX(0, .5f, true);
            mainMenuTabs = MainMenuTabs.DeckTab;
            StartCoroutine("DeckTabCardPops");
        }
        else
        {
            mainTab.DOLocalMoveX(0, .5f, true);
            deckTab.DOLocalMoveX(1080, .5f, true);
            mainMenuTabs = MainMenuTabs.MainTab;
        }
    }
    public void LeaderboardTabOnOff()
    {
        ButtonClickAudio();
        if (mainMenuTabs == MainMenuTabs.MainTab)
        {
            mainTab.DOLocalMoveX(1080, .5f, true);
            leaderboardTab.DOLocalMoveX(0, .5f, true);
            mainMenuTabs = MainMenuTabs.LeaderboardTab;
            StartCoroutine("ScoreBoardPlatePops");
        }
        else
        {
            mainTab.DOLocalMoveX(0, .5f, true);
            leaderboardTab.DOLocalMoveX(-1080, .5f, true);
            mainMenuTabs = MainMenuTabs.MainTab;
        }
    }
    public IEnumerator CannotStartGame()
    {
        cannotPlayPanel.rectTransform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(2f);

        cannotPlayPanel.rectTransform.DOScale(Vector3.zero, .3f).SetEase(Ease.OutSine);
    }
    IEnumerator DeckTabCardPops()
    {
        foreach (GameObject card in deckTabCards)
        {
            card.transform.localScale = Vector3.zero;
        }

        yield return new WaitForSeconds(.1f);

        foreach (GameObject card in deckTabCards)
        {
            SpawnCardAudio();
            card.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(.1f);
        }
    }
    IEnumerator ScoreBoardPlatePops()
    {
        foreach (GameObject plate in leaderboardPlates)
        {
            plate.transform.localScale = Vector3.zero;
        }

        yield return new WaitForSeconds(.1f);

        foreach (GameObject plate in leaderboardPlates)
        {
            SpawnCardAudio();
            plate.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(.1f);
        }
    }
    public void MainMenuSceneFade()
    {
        ButtonClickAudio();

        if (MultiScene.multiScene.purchasedCards.Count != 0)
        {
            sceneFaderAnim.Play("SceneChange1");
            SoundManager.Instance.FadeMusic(0.75f, false, 0);
            Invoke("StartGame", .75f);
        }
        else StartCoroutine(gamemanager.userInterface.CannotStartGame());
    }
    void StartGame()
    {
        gamemanager.sceneManagement.StartGame();
    }
    void NewHighScore()
    {
        SoundManager.Instance.PlayMusicSound(gamemanager.assetBank.FindSound(AssetBank.Sound.NewHighScoreJingle));
        // tänne myöhemmin jotain juicea
    }
    public void BackToMainMenuFade()
    {
        StartCoroutine(StartMenuFade());
    }
    IEnumerator StartMenuFade()
    {
        ButtonClickAudio();
        runTimeMenuFade.GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitForSecondsRealtime(1);
        gamemanager.sceneManagement.MainMenu();
    }
}
