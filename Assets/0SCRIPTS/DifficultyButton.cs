using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private int difficulty;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        DifficultyCheck();
    }
    public void SetDifficulty()
    {
        MultiScene.multiScene.difficulty = difficulty;
        MultiScene.multiScene.UpdateDifficulty(difficulty);
        gamemanager.userInterface.ButtonClickAudio();
        foreach (GameObject difficultyButton in gamemanager.userInterface.difficultyButtons)
        {
            if (difficultyButton != this.gameObject) difficultyButton.GetComponent<Animator>().SetInteger("State", 0);
            else anim.SetInteger("State", 1);
        }
    }
    public void DifficultyCheck()
    {
        if (MultiScene.multiScene.difficulty == difficulty) anim.SetInteger("State", 1);
        else anim.SetInteger("State", 0);
    }
}
