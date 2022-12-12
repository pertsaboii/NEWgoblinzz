using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class LBManager : MonoBehaviour
{
    string key = "globalHighscore";
    public LootLockerLeaderboardMember[] members;
    void Start()
    {
        StartCoroutine("LoginRoutine");       
    }
    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("player was logged in");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("could not start session");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        if (gamemanager.state == gamemanager.State.MainMenu) StartCoroutine(FetchTopHighscoresRoutine());
    }
    public void SetPlayerName()
    {
        LootLockerSDKManager.SetPlayerName(gamemanager.userInterface.submitScoreName.text, (response) =>
        {
            if (response.success) Debug.Log("player name changed");
            else Debug.Log("could not change player name");
        });
    }
    public IEnumerator SubmitScoreRoutine(float score/*, string playerName*/)
    {
        int scoreToUpload = ((int)score);
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, key, (response) =>
        {
            if (response.success)
            {
                Debug.Log("succesfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed " + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        PlayerPrefs.DeleteAll();
    }
    IEnumerator FetchTopHighscoresRoutine()
    {
        bool done = false;

        LootLockerSDKManager.GetScoreList(key, 10, 0, (response) =>
        {
            if (response.success)
            {
                members = response.items;
                done = true;

                List<LBtemplate> plateInfos = new List<LBtemplate>();
                foreach (GameObject plateInfo in gamemanager.userInterface.leaderboardPlates)
                {
                    plateInfos.Add(plateInfo.transform.GetChild(0).GetComponent<LBtemplate>());
                }

                for (int i = 0; i < members.Length; i++)
                {
                    LBtemplate plateInfo = gamemanager.userInterface.leaderboardPlates[i].transform.GetChild(0).GetComponent<LBtemplate>(); // voi säästää hiukan muistia jos tekee tän paremmin
                    plateInfo.scoreText.text = members[i].score.ToString();
                    plateInfo.nameText.text = members[i].player.name.ToString();
                    plateInfo.hasInfo = true;
                }
                foreach (LBtemplate plateInfo in plateInfos)
                {
                    if (plateInfo.hasInfo == false) plateInfo.gameObject.SetActive(false);
                }
            }
            else Debug.Log("Failed " + response.Error);
        });
        yield return new WaitWhile(() => done == false);
    }
}
