using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;   //mahdollistaa binary tiedostojen tekemisen eli datan varastoinnin
using System.IO;                                        //(input output)

public class SaveManager : MonoBehaviour
{
    /*public static SaveManager Instance;

    public bool gameStartedFirstTime;
    public float highScore;             // SIIRRÄ MULTISCENEN HIGHSCORE-REFERENSSI TÄHÄN
    public float money;                 // SIIRRÄ MULTISCENEN MONEY-REFERENSSI TÄHÄN
    public string purchasedCards;

    private void Awake()        //Awake lataa ennen starttia
    {
        if (Instance == null)     //jos control ei ole assignattu
        {
            DontDestroyOnLoad(gameObject);  //ei poista kyseistä objektia
            Instance = this;     //control on tämä
        }
        else if (Instance != this)    //muuten jos control ei ole tämä mutta on olemassa
        {
            Destroy(gameObject);    //poista tämä gameobject
        }
        Load();
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
        data.purchasedCards = purchasedCards;

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
            purchasedCards = data.purchasedCards;
            Debug.Log("game loaded");
        }
    }
    public void ResetSave()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        }
        MultiScene.multiScene.money = 300;
        MultiScene.multiScene.highScore = 0;
        gameStartedFirstTime = false;
        gamemanager.userInterface.mainMenuHighScoreText.text = "High Score: " + MultiScene.multiScene.highScore.ToString();
        gamemanager.userInterface.deckTabMoneyText.text = MultiScene.multiScene.money.ToString();
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.ButtonClicked));
        Debug.Log("progress reset");
    }
}

[Serializable]      //mahdollistaa asioiden tallentamisen tiedostoon
class PlayerData    //asiat joita tallennetaan ja lueataan save filestä, pohja näille
{
    //public bool nightCircuitGodlike;
    public float highScore;
    public float money;
    public bool gameStartedFirstTime;
    public string purchasedCards;*/
}
