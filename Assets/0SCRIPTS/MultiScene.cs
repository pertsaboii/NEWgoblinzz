using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;   //mahdollistaa binary tiedostojen tekemisen eli datan varastoinnin
using System.IO;                                        //(input output)

public class MultiScene : MonoBehaviour
{
    public static MultiScene multiScene;

    // savedata
    public float highScore;
    public float money;
    public string cardIDs;
    public string deckCards;
    [SerializeField] private bool gameStartedFirstTime;
    public int difficulty;

    [SerializeField] private float easyModeMoneyMult;
    [SerializeField] private float hardModeMoneyMult;
    [HideInInspector] public float moneyMult;
    private bool difficultyUpdatedFirstTime;

    public List<GameObject> purchasedCards;
    public List<GameObject> cardsOnDeck;

    private void Awake()
    {
        if (multiScene == null)
        {
            DontDestroyOnLoad(gameObject);
            multiScene = this;
            Load();
        }
        else if (multiScene != this)
        {
            Destroy(gameObject);
        }
        /*if (difficultyUpdatedFirstTime == false)
        {
            difficulty = 1;
            moneyMult = 1;
        }*/

    }
    public void UpdateDifficulty(int difficultyLevel)
    {
        difficultyUpdatedFirstTime = true;
        if (difficultyLevel == 0) moneyMult = easyModeMoneyMult;
        else if (difficultyLevel == 2) moneyMult = hardModeMoneyMult;
        else moneyMult = 1;
    }
    private void OnApplicationQuit()
    {
        Save();
    }

    public void Save()  //tiedon tallennus
    {
        BinaryFormatter bf = new BinaryFormatter(); //tekee uuden binaryformatterin eli sen joka kirjoittaa datan
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat"); //tekee tiedoston, xxx.dat <- luotavan tiedoston nimi

        PlayerData data = new PlayerData(); //hakee tiedostoon tallennettavat tiedot
        data.highScore = highScore;
        data.money = money;
        data.gameStartedFirstTime = gameStartedFirstTime;
        data.cardIDs = cardIDs;
        data.deckCards = deckCards;
        data.difficulty = difficulty;

        data.UIVol = SoundManager.Instance.UIVol;
        data.MasterVol = SoundManager.Instance.MasterVol;
        data.SFXVol = SoundManager.Instance.SFXVol;
        data.MusicVol = SoundManager.Instance.MusicVol;

        bf.Serialize(file, data);   // tallentaa tiedot tiedostoon
        file.Close();   //sulkee tiedoston
        Debug.Log("game saved");
    }

    public void Load()  //avaa olemassaolevan tiedoston
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))     //tarkistaa että tiedosto on olemassa
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file); //otetaan data tiedostosta
            file.Close();   //sulkee tiedoston

            highScore = data.highScore;
            money = data.money;
            gameStartedFirstTime = data.gameStartedFirstTime;
            difficulty = data.difficulty;
            if (data.cardIDs != null) cardIDs = data.cardIDs;
            if (data.deckCards != null) deckCards = data.deckCards;

            SoundManager.Instance.UIVol = data.UIVol;
            SoundManager.Instance.MasterVol = data.MasterVol;
            SoundManager.Instance.SFXVol = data.SFXVol;
            SoundManager.Instance.MusicVol = data.MusicVol;

            Debug.Log("game loaded");
        }
        if (gameStartedFirstTime == false)
        {
            money = 600;
            gameStartedFirstTime = true;
        }
        UpdateDifficulty(difficulty);
    }
    public void ResetSave()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        }
        money = 600;
        highScore = 0;
        gameStartedFirstTime = true;
        foreach (GameObject deckTabCard in gamemanager.userInterface.deckTabCards)
        {
            ShopCard shopCardScript = deckTabCard.transform.GetChild(0).GetComponent<ShopCard>();
            if (cardIDs.Contains(shopCardScript.cardID)) shopCardScript.NotPurchasedState();
        }
        cardIDs = "";
        deckCards = "";
        cardsOnDeck.Clear();
        purchasedCards.Clear();
        difficulty = 1;
        SoundManager.Instance.SFXVol = 1;
        SoundManager.Instance.MasterVol = 1;
        SoundManager.Instance.UIVol = 1;
        SoundManager.Instance.MusicVol = 1;
        SoundManager.Instance.LoadAudioValues();
        foreach (GameObject difficultyButton in gamemanager.userInterface.difficultyButtons)
        {
            difficultyButton.GetComponent<DifficultyButton>().DifficultyCheck();
        }
        gamemanager.userInterface.mainMenuHighScoreText.text = "High Score: " + MultiScene.multiScene.highScore.ToString();
        gamemanager.userInterface.deckTabMoneyText.text = MultiScene.multiScene.money.ToString();
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.ButtonClicked));
        UpdateDifficulty(difficulty);
        Debug.Log("progress reset");
    }
}

[Serializable]      //mahdollistaa asioiden tallentamisen tiedostoon
class PlayerData    //asiat joita tallennetaan ja lueataan save filestä, pohja näille
{
    // general
    public float highScore;
    public float money;
    public bool gameStartedFirstTime;
    public string cardIDs = "";
    public string deckCards = "";
    public int difficulty;

    // audio settings
    public float SFXVol, UIVol, MasterVol, MusicVol;
}
