using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static int score;

    void Start()
    {
        
    }

    public void scoreUpdate(int InputScore)
    {
        if (score == 0)
        {
            if (InputScore > 0)
            {
                score += InputScore;
            }
        }
        else
        {
            score += InputScore;
        }
    }
}
